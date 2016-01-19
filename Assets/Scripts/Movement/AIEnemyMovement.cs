using UnityEngine;
using System.Collections;

public class AIEnemyMovement : Movement {

    public enum EnemyState
    {
        Idle,
        AvoidRight,
        AvoidLeft,
        AvoidBack,
        AvoidFront,
        Hit,
        Return,
        Launch
    }

    public float maxEnergy = 100.0f;
    public GameObject boostParticles;

    public float ballRadius = 0.5f;
    public float hotPointDistance = 0.4f;
    public float redGoalDangerZone = 10.0f;
    public float rotationDamp = 0.15f;

    public float avoidMarginX = 28.5f;
    public float avoidMarginZ = 48.0f;

    public float forceParameter = 0.1f;
    public float velocityDivider = 3.0f;

    public float minJumParameter = 1.0f;
    public float maxJumParameter = 2.0f;
    public float jumpDistance = 3.0f;

    public Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
    public float ballHeightSearchLimit = 4.0f;

    public Transform Ball;
    public Transform BlueGoal;
    public Transform RedGoal;

    public Transform RedPoint1;
    public Transform RedPoint2;

    public string BallTag;
    public string BlueGoalTag;
    public string RedGoalTag;

    public string RedPoint1Tag;
    public string RedPoint2Tag;

    private Vector3 _hotPoint = Vector3.zero;
    private float _energy = 0;
    private EnemyState _actionState = EnemyState.Idle;

    public float Energy
    {
        set { _energy = value; if (_energy > maxEnergy)_energy = maxEnergy; }
        get { return _energy; }
    }

    void OnValidate()
    {
        if (hotPointDistance > ballRadius)
        {
            hotPointDistance = ballRadius;
        }

        if (minJumParameter > maxJumParameter)
        {
            minJumParameter = maxJumParameter;
        }
    }

	// Use this for initialization
    IEnumerator Start()
    {
        setParticlesActive(false);
        checkGameObjects();
        return base.Start();
	}
	
	// Update is called once per frame
	void Update () {
        clearKeyStates();
        checkGameObjects();
        checkState();
        executeBehaviour();
	}

    private void setParticlesActive(bool active)
    {
        if (boostParticles != null)
        {
            boostParticles.SetActive(active);
        }
    }

    private void checkState()
    {
        if (!checkZMargin() && !checkXMargin())
        {
            bool flag = false;

            flag |= checkPlayerBallPosition();

            if (!flag)
                flag |= checkDangerZone();

            if (!flag)
                calculateHotPoint();
        }
    }

    private bool checkPlayerBallPosition()
    {
        if (Ball.position.z - ballRadius < _myTransform.position.z + _controller.radius)
        {
            _actionState = EnemyState.Return;
            return true;
        }
        return false;
    }

    private bool checkDangerZone()
    {
        if (Ball.position.z < RedGoal.position.z + redGoalDangerZone)
        {
            _actionState = EnemyState.Hit;
            return true;
        }

        return false;
    }

    private void calculateHotPoint()
    {
        Vector3 dir = (Ball.position - BlueGoal.position).normalized;
        _hotPoint = Ball.position + hotPointDistance * dir;
        if (Mathf.Abs(_hotPoint.x) > avoidMarginX)
        {
            _actionState = EnemyState.Hit;
            return;
        }

        _actionState = EnemyState.Launch;

    }

    private bool checkXMargin()
    {
        if (_myTransform.position.x < -avoidMarginX)
        {
            _actionState = EnemyState.AvoidLeft;
            return true;
        }
        else if (_myTransform.position.x > avoidMarginX)
        {
            _actionState = EnemyState.AvoidRight;
            return true;
        }

        return false;
    }

    private bool checkZMargin()
    {
        if (_myTransform.position.z < -avoidMarginZ )
        {
            _actionState = EnemyState.AvoidBack;
            return true;
        }
        else if (_myTransform.position.z > avoidMarginZ)
        {
            _actionState = EnemyState.AvoidFront;
            return true;
        }

        return false;
    }

    private void executeBehaviour()
    {
        switch (_actionState)
        {
            case EnemyState.Return:
                returnToGoal();
            break;
            case EnemyState.Hit:
                hitBall();
            break;
            case EnemyState.Launch:
                launchBall();
            break;
            case EnemyState.AvoidLeft:
                avoidLeft();
            break;
            case EnemyState.AvoidRight:
                avoidRight();
            break;
            case EnemyState.AvoidFront:
                avoidFront();
            break;
            case EnemyState.AvoidBack:
                avoidBack();
            break;
        }
    }

    private void returnToGoal()
    {
        Vector3 dir1 = RedPoint1.position - _myTransform.position;
        Vector3 dir2 = RedPoint2.position - _myTransform.position;
        float dist1 = (RedPoint1.position - Ball.position).magnitude;
        float dist2 = (RedPoint2.position - Ball.position).magnitude;

        if (dist1 < dist2)
        {
            turnToDirection(dir1.normalized);
        }
        else
        {
            turnToDirection(dir2.normalized);
        }

        MoveForward(Movement.Move.Forward);
    }

    private void hitBall()
    {
        Vector3 dir = Ball.position - _myTransform.position;

        turnToDirection(dir.normalized);
        MoveForward(Movement.Move.Forward);
        checkJump();
    }

    private void launchBall()
    {
        Vector3 dir = _hotPoint - _myTransform.position;

        turnToDirection(dir.normalized);

        checkBallHeightAndAdvance();
        checkJump();
    }

    private void turnToDirection(Vector3 dir)
    {
        float direction = Vector3.Dot(dir, _myTransform.right);
        float forward = Vector3.Dot(dir, _myTransform.forward);

        if (forward <= 0)
        {

            if (direction <= 0)
            {
                TurnAround(Movement.Side.Left);
            }
            else
            {
                TurnAround(Movement.Side.Right);
            }
        }
        else
        {
            if (direction > rotationDamp)
            {
                TurnAround(Movement.Side.Right);
            }
            else if (direction < -rotationDamp)
            {
                TurnAround(Movement.Side.Left);
            }
            else
            {
                TurnAround(Movement.Side.None);
            }
        }
    }

    private void avoidRight()
    {
        Vector3 myDir = _myTransform.forward;

        float xProj = Vector3.Dot(myDir, new Vector3(1.0f, 0.0f, 0.0f));

        if (xProj > -0.5)
        {
            float zProj = Vector3.Dot(myDir, new Vector3(0.0f, 0.0f, 1.0f));

            if (zProj < 0)
            {
                TurnAround(Movement.Side.Right);
            }
            else
            {
                TurnAround(Movement.Side.Left);
            }
        }

        checkVelAndAdvance();
        
    }

    private void avoidLeft()
    {
        Vector3 myDir = _myTransform.forward;

        float xProj = Vector3.Dot(myDir, new Vector3(1.0f, 0.0f, 0.0f));

        if (xProj < 0.5)
        {
            float zProj = Vector3.Dot(myDir, new Vector3(0.0f, 0.0f, 1.0f));

            if (zProj < 0)
            {
                TurnAround(Movement.Side.Left);
            }
            else
            {
                TurnAround(Movement.Side.Right);
            }
        }

        checkVelAndAdvance();
    }

    private void avoidBack()
    {

        goToCenter();
    }

    private void avoidFront()
    {
        goToCenter();
    }

    private void goToCenter()
    {
        Vector3 dir = center - _myTransform.position;

        turnToDirection(dir);

        checkVelAndAdvance();
    }

    private void checkVelAndAdvance()
    {
        if (_acumLinearVel < moveSpeed / velocityDivider)
            MoveForward(Movement.Move.Forward);
        else
            MoveForward(Movement.Move.None);
    }

    private void checkBallHeightAndAdvance()
    {
        Vector3 distVec = BlueGoal.position - Ball.position;
        distVec.y = 0.0f;
        float ballDist = distVec.magnitude;

        if (Ball.position.y > ballHeightSearchLimit)
        {
            checkVelAndAdvance();
        }
        else if (_acumLinearVel < ballDist * forceParameter)
            MoveForward(Movement.Move.Forward);
        else
            MoveForward(Movement.Move.None);
    }

    private void checkJump()
    {
        Vector3 distVec = Ball.position - _myTransform.position;
        float  dist = new Vector2(distVec.x, distVec.z).magnitude;

        if (dist <= jumpDistance && dist > ballRadius + _controller.radius &&
            distVec.y > jumpVelocity * minJumParameter && distVec.y < jumpVelocity * maxJumParameter)
        {
            SetJump(true);
        }
    }

    private void clearKeyStates()
    {
        SetJump(false);
        SetRun(false);
    }

    private void checkGameObjects()
    {
        if( Ball == null)
        {
            Ball = GameObject.FindGameObjectWithTag(BallTag).transform;
        }

        if( BlueGoal == null )
        {
            BlueGoal = GameObject.FindGameObjectWithTag(BlueGoalTag).transform;
        }

        if( RedGoal == null)
        {
            RedGoal = GameObject.FindGameObjectWithTag(RedGoalTag).transform;
        }

        if( RedPoint1 == null)
        {
            RedPoint1 = GameObject.FindGameObjectWithTag(RedPoint1Tag).transform;
        }

        if (RedPoint2 == null)
        {
            RedPoint2 = GameObject.FindGameObjectWithTag(RedPoint2Tag).transform;
        }
    }
}
