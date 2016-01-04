using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animation))]
public class Movement : MonoBehaviour {
    public float linearAccelTime = 0.0f;
    public float linearDeccelTime = 0.0f;
	public float moveSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float strafeSpeed = 2.5f;
	public float rotateSpeed = 250.0f;

    public float maxGravity = 50.0f;
    public float gravityAccel = 20.0f;
    public float timeBeforeFall = 0.5f;
    public float jumpVelocity = 3.0f;
    public float jumpWaitTime = 0.3f;

    public AnimationClip idleAnimation;
    public AnimationClip turnAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip runAnimation;
    public AnimationClip strafeAnimation;
    public AnimationClip fallAnimation;
    public AnimationClip jumpAnimation;
    public AnimationClip swimAnimation;
    public AnimationClip runSwimAnimation;
    public AnimationClip swimIdleAnimation;
    public AnimationClip brakingAnimation;

    public enum JumpState
    {
        Recovering,
        Standing,
        Jumping,
        DoubleJumping,
        Disabled
    }

    public enum Side
    {
        Left = -1,
        None = 0,
        Right = 1
    }

    public enum Move
    {
        Backwards = -1,
        None = 0,
        Forward = 1
    }

    protected enum State
    {
        Stop,
        Init,
        Setup,
        Action
    }

    protected Transform _myTransform;
    protected CharacterController _controller;

    protected CollisionFlags _myCollisionFlags;
    protected Vector3 _movementDirection;
    protected Vector3 _lastDirection;
    protected float _verticalVelocity = 0.0f;
    protected JumpState _jumping;
    protected float _acumLinearVel = 0.0f;

    protected Side _turn;
    protected Side _strafe;
    protected Move _forward;
    protected bool _run;
    protected bool _jump;
    protected bool _swimming;

    private float _airTime = 0.0f;
    private int _recoverTime = 0; // milisecods

    private State _state;

	public void Awake(){
		_myTransform = transform;
		_controller = GetComponent<CharacterController> ();
        _state = State.Init;
	}

	// Use this for initialization
	protected IEnumerator Start () {
        while (true)
        {
            // Creamos una sencilla maquina de estados
            switch (_state)
            {
                case State.Init:
                    Init();
                    break;
                case State.Setup:
                    Setup();
                    break;
                case State.Action:
                    move();
                    break;
            }
            yield return null;
        }
	}
	
    void move() 
    {
        _movementDirection = Vector3.zero;

        bool movement = false;

        movement |= Turn();

        if (_controller.isGrounded || _swimming)
        {
            _airTime = 0.0f;

            UpdateJumpState();

            movement |= Strafe();
            movement |= Walk();
            movement |= Jump();

            _lastDirection = _movementDirection;
        }
        else
        {
            if ((_myCollisionFlags & CollisionFlags.CollidedBelow) == 0)
            {
                movement = true;

                _airTime += Time.deltaTime;

                if (_airTime > timeBeforeFall && _verticalVelocity < 0.0f)
                {
                   Fall();
                }
            }

            _movementDirection += new Vector3(_lastDirection.x, 0.0f, _lastDirection.z);
        }

        if (!movement)
        {
            Idle();
        }

        if (_verticalVelocity > -maxGravity)
        {
            float gravityVel = gravityAccel * Time.deltaTime;
            float tmpVel = _verticalVelocity - gravityVel;
            _verticalVelocity = tmpVel > -maxGravity ? tmpVel : -maxGravity;
        }

        _movementDirection.y += _verticalVelocity;

        if (_swimming)
            _movementDirection.y = 0.0f;

        //Utilizamos move para obtener las banderas de colisión y reducimos el movimiento a una única llamada
        _myCollisionFlags = _controller.Move( _movementDirection * Time.deltaTime);

        if ((_myCollisionFlags & CollisionFlags.CollidedSides) == CollisionFlags.Sides)
        {
            Debug.Log("Collision");
            _acumLinearVel = 0;
        }
	}

    private void Init()
    {
        // Realizamos comprobaciones de componentes requeridos
        if (!GetComponent<CharacterController>()) return;
        if (!GetComponent<Animation>()) return;

        _state = State.Setup;
    }

    private void Setup()
    {
        animation.Stop(); // Paramos cualquier animación que pudiera estar ejecutandose 
        animation.wrapMode = WrapMode.Loop;

        if (jumpAnimation != null)
        {
            animation[jumpAnimation.name].layer = 1; //Aumentamos el delay en la ejecución de la animación
            animation[jumpAnimation.name].wrapMode = WrapMode.Once;
        }

        if (idleAnimation != null)
            animation.Play(idleAnimation.name);

        _movementDirection = Vector3.zero;
        _lastDirection = Vector3.zero;
        _myCollisionFlags = CollisionFlags.None;
        _jumping = JumpState.Standing;

        _turn = Side.None;
        _strafe = Side.None;
        _forward = Move.None;
        _run = false;
        _jump = false;
        _swimming = false;

        _state = State.Action;
    }

    private bool Turn()
    {
        int horiz = (int)_turn;

        if (Mathf.Abs(horiz) > 0)
        {
            _myTransform.Rotate(0, horiz * Time.deltaTime * rotateSpeed, 0);
            if (turnAnimation != null)
                animation.CrossFade(turnAnimation.name);

            if (_acumLinearVel != 0)
                return false;

            return true;
        }

        return false;
    }

    protected bool Walk()
    {
        int vert = (int)_forward;

        if (Mathf.Abs(vert) > 0)
        {
            float speed = 0.0f;
            float accel = 0.0f;
            Vector3 auxVec = Vector3.zero;

            if (_run)
            {
                if (linearAccelTime != 0)
                    accel = runSpeed / linearAccelTime;

                speed = runSpeed;
                if (!checkSwimming(runSwimAnimation) && runAnimation != null)
                {
                    animation.CrossFade(runAnimation.name);
                }
            }
            else
            {
                if (linearAccelTime != 0)
                    accel = moveSpeed / linearAccelTime;

                speed = moveSpeed;
                if(!checkSwimming(swimAnimation) && walkAnimation != null)
                    animation.CrossFade(walkAnimation.name);
            }

            if (linearAccelTime != 0)
            {
                _acumLinearVel += accel * vert * Time.deltaTime;
                auxVec = _myTransform.forward * _acumLinearVel;
            }
            else
            {
                auxVec = _myTransform.forward * vert;
                auxVec = auxVec.normalized * speed;
            }

            _movementDirection += auxVec;

            return true;
        }
            
        return false;
    }

    protected bool Strafe()
    {
        int strafe = (int)_strafe;

        if (Mathf.Abs(strafe) > 0)
        {
            Vector3 auxVec = Vector3.zero;

            auxVec = _myTransform.right * strafe;
            auxVec = auxVec.normalized * strafeSpeed;

            _movementDirection += auxVec;
            if (!checkSwimming(swimAnimation) && strafeAnimation != null)
                animation.CrossFade(strafeAnimation.name);
            return true;
        }

        return false;
    }

    protected bool Jump()
    {
        int jump = _jump ? 1 : 0;

        if (Mathf.Abs(jump) > 0 && _jumping == JumpState.Standing)
        {
            _verticalVelocity = jumpVelocity;
            if(jumpAnimation != null)
                animation.CrossFade(jumpAnimation.name);
            _jumping = JumpState.Jumping;
            return true;
        }

        return false;
    }

    protected void Fall()
    {
        if(fallAnimation != null)
            animation.CrossFade(fallAnimation.name);
    }

    protected void Idle()
    {
        if (!checkSwimming(swimAnimation))
        {
            if (linearDeccelTime != 0 && _acumLinearVel != 0)
            {
                float deccel = moveSpeed / linearDeccelTime;
                float aux = deccel * Time.deltaTime;

                if (_acumLinearVel < 0)
                {
                    _acumLinearVel += aux;
                    if (_acumLinearVel > -aux)
                        _acumLinearVel = 0;
                }
                else
                {
                    _acumLinearVel -= aux;
                    if (_acumLinearVel < aux)
                        _acumLinearVel = 0;
                }

                _movementDirection += _myTransform.forward * _acumLinearVel;

                if (brakingAnimation != null)
                    animation.CrossFade(brakingAnimation.name);

                return;
            }

            if(idleAnimation != null)
                animation.CrossFade(idleAnimation.name);
        }
    }

    public void UpdateJumpState()
    {
        if ((_jumping != JumpState.Standing) && _recoverTime < jumpWaitTime * 1000)
        {
            _jumping = JumpState.Recovering;
            _recoverTime += (int)(Time.deltaTime * 1000);
        }
        else
        {
            _recoverTime = 0;
            _jumping = JumpState.Standing;
        }
    }

    protected bool checkSwimming(AnimationClip anim)
    {
        if (_swimming && anim != null)
        {
            animation.CrossFade(anim.name);
            return true;
        }

        return false;
    }

    protected void MoveForward(Move move)
    {
        _forward = move;
    }

    protected void TurnAround(Side side)
    {
        _turn = side;
    }

    protected void MoveToTheSide(Side side)
    {
        _strafe = side;
    }

    protected void SetJump(bool jump)
    {
        _jump = jump;
    }

    protected void SetRun(bool run)
    {
        _run = run;
    }

    private void SetSwimming(bool swimming)
    {
        //Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Swimming<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
        _swimming = swimming;

        if (_swimming)
        {
            _jumping = JumpState.Disabled;
        }
        else
        {
            _jumping = JumpState.Standing;
        }
    }
}


