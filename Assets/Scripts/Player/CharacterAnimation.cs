using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator CharAnim;
    public float IdleTimer = 30f; // Amount of time the user has to enter no input before the idle animation plays
    private float CurrentIdleTime;

    private Movement AirMove;
    private Rigidbody2D rb;

    private void Awake()
    {
        CharAnim = GetComponent<Animator>();
        AirMove = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        SetBasicAnimation();
        AirAnimations();

        if (CurrentIdleTime >= IdleTimer)
        {
            PlayIdleAnim();
        }

        CurrentIdleTime += Time.deltaTime;
    }

    private void AirAnimations()
    {
        float Y_Vel = rb.velocity.y;
        bool grounded = AirMove.CheckGrounded();
        bool Moved = false;
        if (Movement.PlayerInput.JumpTriggered())
        {
            CharAnim.SetTrigger("Jump");
            Moved = true;
        }

        if (!grounded)
        {
            if (Y_Vel < 0)
            {
                CharAnim.SetBool("Falling", true);
                ResetIdleTimer();
                Moved = true;
            }
        }

        else if(grounded || Mathf.RoundToInt(Y_Vel) == 0)
        {
            CharAnim.SetBool("Falling", false);
        }

        if (Moved) { ResetIdleTimer(); }
    }

    private void SetBasicAnimation()            // Includes walking, Running
    {
        Vector2 P_Input = new Vector2(Movement.PlayerInput.Horizontal, Movement.PlayerInput.Vertical);
        CharAnim.SetFloat("Input_X", Mathf.Abs(P_Input.x));
        CharAnim.SetFloat("Input_Y", P_Input.y);

        if (P_Input.x != 0)
        {
            CharAnim.SetFloat("Input_Y", 0);
        }

        if (P_Input.x != 0)
        {
            if (Movement.PlayerInput.Running)
            {
                CharAnim.SetBool("Run", true);
            }
        }

        if (Movement.PlayerInput.RunStopTriggered() || P_Input.x == 0)
        {
            CharAnim.SetBool("Run", false);

        }

        if (P_Input != Vector2.zero)
            ResetIdleTimer();
    }

    private void PlayIdleAnim()
    {
        CharAnim.SetTrigger("IdleTrigger");
        CurrentIdleTime = 0;
    }

    private void ResetIdleTimer()
    {
        CurrentIdleTime = 0;
    }
}
