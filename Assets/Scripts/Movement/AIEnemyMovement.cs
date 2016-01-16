using UnityEngine;
using System.Collections;

public class AIEnemyMovement : Movement {

    public enum EnemyState
    {
        Idle,
        Search,
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
        bool flag = false;

        flag |= checkPlayerBallPosition();

        if (!flag)
            flag |= checkDangerZone();

        if (!flag)
            calculateHotPoint();

    }

    private bool checkPlayerBallPosition()
    {
        if (Ball.position.z - ballRadius < _myTransform.position.z)
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
        if (Mathf.Abs(_hotPoint.x) > 29.0f)
        {
            _actionState = EnemyState.Hit;
            return;
        }

        _actionState = EnemyState.Launch;

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
        }
    }

    private void returnToGoal()
    {
        Vector3 dir1 = RedPoint1.position - _myTransform.position;
        Vector3 dir2 = RedPoint2.position - _myTransform.position;
        float dist1 = dir1.magnitude;
        float dist2 = dir2.magnitude;

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
    }

    private void launchBall()
    {
        Vector3 dir = _hotPoint - _myTransform.position;

        turnToDirection(dir.normalized);
        MoveForward(Movement.Move.Forward);
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
