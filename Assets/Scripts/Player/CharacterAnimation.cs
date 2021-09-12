using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator CharAnim;
    public float IdleTimer = 30f; // Amount of time the user has to enter no input before the idle animation plays
    private float CurrentIdleTime;

    private Movement AirMove;
    private SpecialCharacterMovement SpecialMove;
    private ThrowLasso LassoMove;
    private Rigidbody2D rb;

    private string m_CurrentState;
    public string CurrentState
    {
        get { return m_CurrentState; }
        set
        {
            m_CurrentState = value;
            StateChanged();
        }
    }

    private void StateChanged()
    {
        var ID = Animator.StringToHash(m_CurrentState);
        CharAnim.Play(ID);
    }

    private int m_CurrentMoveState;

    public int CurrentMoveState
    {
        get { return m_CurrentMoveState; }
        set
        {
            if(m_CurrentMoveState != value)
            {
                MoveStateChanged(value);
                m_CurrentMoveState = value;
            }
        }
    }

    private void MoveStateChanged(int newState)
    {
        if (m_CurrentMoveState == 2)
        {
            CharAnim.speed = 1;
        }
        Alt_Anim = false;
        if(newState == 0 && (m_CurrentMoveState == 1 || m_CurrentMoveState == 3) && Movement.PlayerInput.Jumping)
        {
            WallJump();
        }
    }

    private void WallJump()
    {
        CurrentState = "WallJump";
        Alt_Anim = true;
    }

    private bool Alt_Anim;

    bool Grounded = false;
    bool hanging = false;
    bool old_hanging = false;
    private void Awake()
    {
        CharAnim = GetComponent<Animator>();
        AirMove = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
        SpecialMove = GetComponent<SpecialCharacterMovement>();
        LassoMove = GetComponentInChildren<ThrowLasso>();
    }

    private void Update()
    {
        Grounded = AirMove.CheckGrounded();
        if(LassoMove)
            hanging = LassoMove.HangingOnLasso;

        CurrentMoveState = (int)SpecialMove.CurrentSpecialMove;
        if(CurrentMoveState == 0)
        {
            if(hanging && !Grounded)
            {
                SetHangingAnimation();
            }
            if (Grounded)
            {
                SetBasicAnimation();
            }
            else if (!Grounded && !hanging)
            {
                AirAnimations();
            }

            if (CurrentIdleTime >= IdleTimer)
            {
                PlayIdleAnim();
            }
            // challenge, implement move variable here
            CurrentIdleTime += Time.deltaTime;
        }
        else if(CurrentMoveState == 1)          //
        {
            CurrentState = "WallSlide";
        }
        else if(CurrentMoveState == 2)
        {
            Vector2 P_Input = new Vector2(Movement.PlayerInput.Horizontal,Movement.PlayerInput.Vertical);
            if(P_Input.y != 0)
            {
                CharAnim.speed = Mathf.Abs(P_Input.y);                       // set the speed of the animation equal to the y Input.
                CurrentState = "Climbing";
            }
            else if(P_Input.x != 0 && rb.velocity.x != 0)
            {
                CharAnim.speed = Mathf.Abs(P_Input.x);                       // set the speed of the animation equal to the y Input.
                CurrentState = (P_Input.x > 0) ? "SideClimb" : "SideClimb2";
            }
            else
            {
                CharAnim.speed = 0;                       // set the speed of the animation equal to the y Input.
            }
        }
        else if(CurrentMoveState == 3)
        {
            CurrentState = "Hanging";
        }
        else if(CurrentMoveState == 4)
        {
            if (Grounded)
            {
                SetBasicAnimation();
            }
            else
            {
                // swimming animation
            }
        }
    }

    private void AirAnimations()
    {
        if (old_hanging)
        {
            old_hanging = false; WallJump(); return;
        }

        float Y_Vel = rb.velocity.y;
        if (Y_Vel > -2 && !Alt_Anim)
        {
            CurrentState = "Jump";
        }

        if(Y_Vel <= -2f)
        {
            Alt_Anim = false;
            CurrentState = "Falling";
        }

    }

    private void SetBasicAnimation()            // Includes walking, Running
    {
        Vector2 P_Input = new Vector2(Movement.PlayerInput.Horizontal, Movement.PlayerInput.Vertical);

        if (P_Input.x != 0)
        {
            bool running = Movement.PlayerInput.Running;
            if (running)
                CurrentState = "Run";
            else
                CurrentState = "Walk";
        }
        else
        {
            if(hanging)
            {
                CurrentState = "Lasso_Idle";
            }
            else
                CurrentState = "Idle";
        }

        if (P_Input != Vector2.zero)
            ResetIdleTimer();
    }

    private void SetHangingAnimation()
    {
        Vector2 P_Input = new Vector2(Movement.PlayerInput.Horizontal, Movement.PlayerInput.Vertical);
        old_hanging = true;
        CurrentState = "Lasso_Hang";
        /*
        if (P_Input.x != 0)
        {
            CurrentState = "Lasso_Hang";
        }
        else
        {
            float vel = rb.velocity.x;
            if (Mathf.Abs(vel) < 0.01f)
            {
                //CurrentState = "Hanging";
                CurrentState = "Lasso_Hang";        // default hanging animation
            }
            else
            {
                CurrentState = "Lasso_Hang";        // default hanging animation
            }
        }
        */
    }

    private void PlayIdleAnim()
    {

        CurrentIdleTime = 0;
    }

    private void ResetIdleTimer()
    {
        CurrentIdleTime = 0;
    }
}
