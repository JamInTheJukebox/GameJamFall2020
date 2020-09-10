using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCharacterMovement : MonoBehaviour
{
    // This script is in charge of handling movement that the player can only execute by interacting with the environment. Examples include: Climbing, WallJumping, ledgeGrabbing, etc.
    // an ENUM controls the current special move to ensure that the player can only do one at a time.
    private Rigidbody2D rb;
    private float Dir;                          // Raw X input
    private float Y_Dir;                        // Raw Y input
    private Movement CharMovement;              // For retreiving the OnGround bool
    private bool OnGround;                      // Determines a variety of methods to escape from special moves.
    private LayerMask PlayerLayer;              

    [SerializeField] LayerMask GroundLayer = 1 << 8;
    private float JumpImpulse = 15f;

    private E_CurrentMode m_CurrentSpecialMove; // Member variable that stores the current state that the player is in
    public E_CurrentMode CurrentSpecialMove
    {
        get
        {
            return m_CurrentSpecialMove;
        }
        protected set
        {
            m_CurrentSpecialMove = value;
            I_CurrentMoveChanged();
        }
    }
    public enum E_CurrentMode               
    {
        Default = 0,                                        // walking, running, jumping    
        WallSliding = 1,
        Climbing = 2,
        Ledge_Grabbing = 3,
        Swimming = 4                            // Might implement it, maybe not. IDK
    }

    [Header("Wall Jumping")]
    public Transform WallCheck;                 // Tracks the position of a object that checks whether the player is pressing against the wall
    public float WallCheckRadius = 0.01f;       
    bool isTouchingWall;                        // Stores the value of whether the player is pressing against the wall
    public float SlideBreakTimer = 3f;          // Amount of time you must hold an input on a wall to break free from sliding.
    private float CurrentSlideTimer;            // tracks the amount of time the player has been trying to break free from the wall slide.
    bool isSlidingOnWall;                       // If the player decides to actually slide down the wall, this will be set to true.
    public float WallSlidingSpeed;
    public float xWallForce;                    // The force that players will bounce off the wall with.

    // ledge Grabbing here

    // TO DO:
    // IMPLEMENT A WALL JUMP TIMER so players do not have to be frame-perfect if they want to jump off the wall immediately.
    [Header("Climbing")]
    [SerializeField] LayerMask LadderLayer = 1 << 9;        // Used only if the climbable surface has no ceiling. Climbing above the ladder leads to bugs(Player slowly falling.
                                                            // The player should stop when they reach the top.
    [SerializeField] string LadderTag = "Climbable";        
    public Vector2 ClimbSpeed = new Vector2(0.7f,1);
    private bool Climbing_Initialized;
    private bool ReadyToClimb = false;                      // If the player is colliding with a ladder, this will be set to true.
    public Transform LadderCheck;                           // If there is a ladder with no ceiling, something needs to prevent the player from climbing so high up that it leaves the ladder's collider
    private bool HorizontalClimbAllowed = false;            
    private bool Ladder_ReachedTop = false;
    private bool Ladder_HasTop = true;
    private Transform LadderPos;
    // TO DO:
    // JUMPING ON HORIZONTAL CLIMBABLE SURFACES SHOULD BE POSSIBLE? 

    [Header("DebugTools")]
    [SerializeField] bool DrawWallCheck = false;
    [SerializeField] bool DrawLadderCeilCheck = false;

    private void I_CurrentMoveChanged()
    {
        switch((int)CurrentSpecialMove)
        {
            case 1:
                break;
            case 2:
                rb.gravityScale = 0;
                break;
            case 3:
                break;
            default:
                // case 0 or any other unintended case.
                break;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CharMovement = GetComponent<Movement>();
        JumpImpulse = CharMovement.JumpImpulse;
        PlayerLayer = gameObject.layer;
    }
    

    private void Update()
    {
        Dir = Input.GetAxisRaw("Horizontal");
        Y_Dir = Input.GetAxisRaw("Vertical");
        OnGround = CharMovement.CheckGrounded();
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpTimer = Time.time + JumpDelay;      // A jump timer allows for a treshold for timing jumps. When a player gets on the ground
            // There is a chance that their input will be dropped due to them not being "technically" grounded yet. This timer allows for them to jump without being frame perfect.
        }
        */
        int currentMode = (int)CurrentSpecialMove;
        if (currentMode == 0 || currentMode == 1)
        {
            WallJump();
        }
        if (currentMode == 0 || currentMode == 2)
        {
            Climb();
        }
        print(CurrentSpecialMove);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == LadderTag && collision.GetComponent<ClimbableSurface>() != null)
        {
            ClimbableSurface newClimbable = collision.GetComponent<ClimbableSurface>();
            ReadyToClimb = true;
            HorizontalClimbAllowed = newClimbable.Get_HorizAllowed();
            Ladder_HasTop = newClimbable.Get_HasTop();
            LadderPos = (Ladder_HasTop) ? collision.transform : null;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == LadderTag)
        {
            ReadyToClimb = false;
            CurrentSpecialMove = 0;
            Climbing_Initialized = false;
        }
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
            CurrentSpecialMove = 0;
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
            float WallJumpDir = -(WallCheck.position - transform.position).x;
            WallJumpDir /= Mathf.Abs(WallJumpDir);
            print(WallJumpDir);
            rb.AddForce(xWallForce * WallJumpDir * Vector2.right, ForceMode2D.Impulse);
            Jump();
            //invoke
        }
    }
    private void Climb()
    {
        if (!ReadyToClimb) { return; }
        Ladder_ReachedTop = !Physics2D.OverlapCircle(LadderCheck.position, WallCheckRadius, LadderLayer);

        if (Y_Dir != 0 || (Climbing_Initialized && Dir != 0))
        {
            CurrentSpecialMove = E_CurrentMode.Climbing;

            if (!Climbing_Initialized)
            {
                Climbing_Initialized = true;
                if (Ladder_HasTop)
                {
                    transform.position = new Vector3(LadderPos.position.x, transform.position.y);
                }
            }

            Vector2 newVel = new Vector2(Dir * ClimbSpeed.x, Y_Dir * ClimbSpeed.y);
            if (!HorizontalClimbAllowed)
                newVel.x = 0;

            if (Ladder_ReachedTop && Y_Dir > 0 && !Ladder_HasTop)
                newVel.y = 0;

            rb.velocity = newVel;
        }
        else if((int)CurrentSpecialMove == 2 && Y_Dir == 0)
        {
            rb.velocity = Vector2.zero;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if(OnGround)
        {
            CurrentSpecialMove = 0;
            Climbing_Initialized = false;
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
        if(DrawLadderCeilCheck)
        {
            Gizmos.color = (Ladder_ReachedTop) ? Color.blue : Color.red;
            Vector3 CurrentPos = LadderCheck.position;
            Gizmos.DrawSphere(CurrentPos, WallCheckRadius);
        }
    }
}
