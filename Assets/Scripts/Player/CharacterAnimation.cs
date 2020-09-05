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
        print(CurrentIdleTime);
        CurrentIdleTime += Time.deltaTime;
    }

    private void AirAnimations()
    {
        float Y_Vel = rb.velocity.y;
        bool grounded = AirMove.CheckGrounded();
        bool Moved = false;
        if (Input.GetKeyDown(KeyCode.Space))
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
        Vector2 PlayerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        CharAnim.SetFloat("Input_X", Mathf.Abs(PlayerInput.x));
        CharAnim.SetFloat("Input_Y", PlayerInput.y);

        if (PlayerInput.x != 0)
        {
            CharAnim.SetFloat("Input_Y", 0);
        }

        if (PlayerInput.x != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                CharAnim.SetBool("Run", true);
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || PlayerInput.x == 0)
        {
            CharAnim.SetBool("Run", false);

        }

        if (PlayerInput != Vector2.zero)
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
