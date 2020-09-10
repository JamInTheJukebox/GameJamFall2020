using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCharacterMovement : MonoBehaviour
{
    // This script is in charge of handling movement that the player can only execute by interacting with the environment. Examples include: Climbing, WallJumping, ledgeGrabbing, etc.
    // an ENUM controls the current special move to ensure that the player can only do one at a time.
    private Rigidbody2D rb;
    private float Dir;
    private Movement CharMovement;
    private bool OnGround;
    [SerializeField] LayerMask GroundLayer = 1 << 8;
    private float JumpImpulse = 15f;

    private E_CurrentMode CurrentSpecialMove;

    public enum E_CurrentMode
    {
        Default = 0,
        WallSliding = 1
    }


    [Header("Wall Jumping")]
    public Transform WallCheck;
    bool isTouchingWall;
    bool isBoundedByWall;
    public float SlideBreakTimer = 3f;          // Amount of time you must hold an input on a wall to break free from sliding.
    private float CurrentSlideTimer;
    bool isSlidingOnWall;
    public float WallCheckRadius;
    public float WallSlidingSpeed;
    bool WallJumping;
    public float xWallForce;
    [Tooltip("When players want to break free, they will hit a directional input to break free from the wall slide.")]
    public float WallJumpBreakThreshold;

    [Header("DebugTools")]
    [SerializeField] bool DrawWallCheck = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CharMovement = GetComponent<Movement>();
        JumpImpulse = CharMovement.JumpImpulse;
    }

    private void SetMode(int mode)
    {
        CurrentSpecialMove = (E_CurrentMode)mode;
    }
    public int GetMode()
    {
        return (int)CurrentSpecialMove;
    }

    private void Update()
    {
        Dir = Input.GetAxis("Horizontal");
        OnGround = CharMovement.CheckGrounded();
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpTimer = Time.time + JumpDelay;      // A jump timer allows for a treshold for timing jumps. When a player gets on the ground
            // There is a chance that their input will be dropped due to them not being "technically" grounded yet. This timer allows for them to jump without being frame perfect.
        }
        */
        WallJump();
    }

    private void WallJump()
    {
        isTouchingWall = Physics2D.OverlapCircle(WallCheck.position, WallCheckRadius, GroundLayer);
        if (!isSlidingOnWall && isTouchingWall && !OnGround && Dir != 0 && rb.velocity.y < -0.1f)
        {
            CurrentSpecialMove = E_CurrentMode.WallSliding;
            isSlidingOnWall = true;
        }
        if (isSlidingOnWall)
        {
            float WallJumpDir = (WallCheck.position - transform.position).x;
            int WallJumpDirection = (int)(WallJumpDir / Mathf.Abs(WallJumpDir));
            //print("WallJumpDir" + WallJumpDirection);
            //print("Dir" + Mathf.Sign(Dir));
            if ((int)Dir != Mathf.Sign(WallJumpDirection))      // Character is trying to break free from a walljump
            {
                CurrentSlideTimer += Time.deltaTime;
            }
        }
        if (CurrentSlideTimer >= SlideBreakTimer | OnGround | !isTouchingWall)
        {
            print("Cancelling slide");
            CurrentSlideTimer = 0;
            CurrentSpecialMove = E_CurrentMode.Default;
            isSlidingOnWall = false;
            // make this a seconds time.
        }

        if (isSlidingOnWall)
        {
            float Y_Vel = rb.velocity.y;
            if (Y_Vel > 0)
            {
                // do nothing
            }
            else if (Y_Vel <= 0)
            {
                rb.gravityScale = 1;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallSlidingSpeed, float.MaxValue));
            }
            // Call Jump here;
        }

        if (Input.GetKeyDown(KeyCode.Space) && (isSlidingOnWall || (isTouchingWall && rb.velocity.y > 0.1f)))
        {
            WallJumping = true;
            //invoke
        }


        if (WallJumping)
        {
            WallJumping = false;
            float WallJumpDir = -(WallCheck.position - transform.position).x;
            WallJumpDir /= Mathf.Abs(WallJumpDir);
            print(WallJumpDir);
            rb.AddForce(xWallForce * WallJumpDir * Vector2.right, ForceMode2D.Impulse);
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * JumpImpulse, ForceMode2D.Impulse);
    }

    private void OnDrawGizmos()
    {
        if (DrawWallCheck)
        {
            Gizmos.color = (isSlidingOnWall) ? Color.blue : Color.red;
            Vector3 CurrentPos = WallCheck.position;
            Gizmos.DrawSphere(CurrentPos, WallCheckRadius);
        }
    }
}
