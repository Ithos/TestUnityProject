using UnityEngine;
using System.Collections;

public class PlayerMovement : Movement {

    public string advanceButton = string.Empty;
    public string turnButton = string.Empty;
    public string strafeButton = string.Empty;
    public string jumpButton = string.Empty;
    public string boostButton = string.Empty;

    public float energyCostPerSec = 50;

    private float _energy = 0;

    public float Energy
    {
        set { _energy = value; }
        get { return _energy; }
    }

	// Use this for initialization
	IEnumerator Start () {
        return base.Start();
	}
	
	// Update is called once per frame
	void Update () {
        checkAdvance();
        checkTurn();
        checkStrafe();
        checkJump();
        checkRun();
	}

    private void checkAdvance()
    {
        if(advanceButton != string.Empty)
        {
            float advance = Input.GetAxis(advanceButton);

            if (advance > 0)
            {
                MoveForward(Movement.Move.Forward);
            }
            else if (advance < 0)
            {
               MoveForward(Movement.Move.Backwards);
            }
            else
            {
                MoveForward(Movement.Move.None);
            }
        }
    }

    private void checkTurn()
    {
        if (turnButton != string.Empty)
        {
            float turn = Input.GetAxis(turnButton);

            if (turn > 0)
            {
                TurnAround(Movement.Side.Right);
            }
            else if (turn < 0)
            {
                TurnAround(Movement.Side.Left);
            }
            else
            {
                TurnAround(Movement.Side.None);
            }
        }
    }

    private void checkStrafe()
    {
        if (strafeButton != string.Empty)
        {
            float strafe = Input.GetAxis(strafeButton);

            if (strafe > 0)
            {
                MoveToTheSide(Movement.Side.Right);
            }
            else if (strafe < 0)
            {
                MoveToTheSide(Movement.Side.Left);
            }
            else
            {
                MoveToTheSide(Movement.Side.None);
            }
        }
    }

    private void checkJump()
    {
        if (jumpButton != string.Empty)
        {
            if (Input.GetButtonDown(jumpButton))
            {
                SetJump(true);
            }
            else if (Input.GetButtonUp(jumpButton))
            {
                SetJump(false);
            }
        }
    }

    private void checkRun()
    {
        if (boostButton != string.Empty)
        {
            if (Input.GetButtonDown(boostButton) && _energy > 0)
            {
                SetRun(true);
            }
            else if (Input.GetButtonUp(boostButton))
            {
                SetRun(false);
            }

            if (_run)
            {
                if (!_controller.isGrounded)
                {
                    _movementDirection = Vector3.zero;
                    Walk();
                    // Boost keeps momentum while in air if this is uncommented
                    //_lastDirection.x = _movementDirection.x;
                    //_lastDirection.z = _movementDirection.z;
                    _myCollisionFlags = _controller.Move(_movementDirection * Time.deltaTime);
                }

                _energy -= energyCostPerSec * Time.deltaTime;
                if (_energy < 0)
                {
                    _energy = 0;
                    SetRun(false);
                }
            }
        }
    }
}
