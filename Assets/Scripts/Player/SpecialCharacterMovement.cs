using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCharacterMovement : MonoBehaviour
{
    // This script is in charge of handling movement that the player can only execute by interacting with the environment. Examples include: Climbing, WallJumping, ledgeGrabbing, etc.
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
            PlayerStateChanged();
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
    public Vector2 WallJumpForce;                    // The force that players will bounce off the wall with.
    
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
    private bool Jumping = false;

    [Header("ClimbDown")]                                   // When you want to climb down from a platform
    public Transform BottomLadderCheck;
    [SerializeField] float BottomLadderCheckRadius = 0.3f;
    private bool ReadyToClimbDown = false;

    [Header("LedgeGrabbing")]
    public float LedgeJumpImpulse = 15;
    public float TimeRequiredToGrabLedge = 0.2f;
    private float CurrentGrabTime = 0f;
    private bool IsLedgeGrabbing = false;
    private int LedgeJumpDir;
    private bool CanLedgeGrabAgain = true;
    public float TimeToRefreshLedgegrab;                // To prevent Ledgegrabs from being abused

    private SpriteRenderer SpritePNG;

    [Header("DebugTools")]
    [SerializeField] bool DrawWallCheck = false;
    [SerializeField] bool DrawLadderCeilCheck = false;
    [SerializeField] bool DrawBottomLadderCheck = false;

    private float WallJumpBufferTimer;
    private float JumpDelay = 0.2f;

    private void PlayerStateChanged()
    {
        switch((int)CurrentSpecialMove)
        {
            case 1:
                break;
            case 2:
                rb.gravityScale = 0;
                SpritePNG.sortingOrder = 4;
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
        SpritePNG = GetComponent<SpriteRenderer>();
    }
    

    private void Update()
    {
        //print(CurrentSpecialMove);
        Dir = Movement.PlayerInput.Horizontal;
        Y_Dir = Movement.PlayerInput.Vertical;
        OnGround = CharMovement.CheckGrounded();

        int currentMode = (int)CurrentSpecialMove;
        if (currentMode == 0 || currentMode == 1)                   // walljumping only
        {
            if (Movement.PlayerInput.JumpTriggered() && isSlidingOnWall) { WallJumpBufferTimer = Time.time + JumpDelay; }
            WallJump();
        }
        if (currentMode == 0 || currentMode == 2)                   // climbing up and down only
        {
            if (((Y_Dir > 0 && rb.velocity.y <= -0.5f)) && CurrentSpecialMove == 0 && Jumping)                                                       // stretch goal: Make it so the user has to hit the key again to grab onto the ladder if they jump
            {
                CurrentSpecialMove = E_CurrentMode.Climbing;
                Jumping = false;
            }
            if (OnGround)
            {
                Jumping = false;
            }
            if (Movement.PlayerInput.JumpTriggered() && (int)CurrentSpecialMove == 2)
            {
                Climbing_Initialized = false;
                Jumping = true;
                CurrentSpecialMove = 0;
                SetYVelocity(Vector2.up * JumpImpulse);
            }
            if (!Jumping)
            {
                Climb();
            }
        }

        if (currentMode == 3)                   // LedgeGrabbing
        {
            if (Movement.PlayerInput.JumpTriggered())
            {
                float X_Force = 0;

                if (Dir != 0)
                {
                    if (Dir * LedgeJumpDir > 0)
                    {
                        X_Force = WallJumpForce.x * Dir;
                    }
                    else
                    {
                        X_Force = WallJumpForce.x * Dir * 1 / 2;
                    }
                }
                SetYVelocity(new Vector2(X_Force, LedgeJumpImpulse));
                CurrentSpecialMove = 0;
                CanLedgeGrabAgain = false;
                Invoke("ResetLedgeGrab", TimeToRefreshLedgegrab);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == LadderTag && collision.GetComponent<ClimbableSurface>() != null)
        {
            ReadyToClimb = true;
            Climb_Init(collision);
            SpritePNG.sortingOrder = 4;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)       // LEDGE GRABBING
    {
        if(isTouchingWall && CanLedgeGrabAgain)
        {
            if(collision.tag == "Ledge")
            {
                CurrentGrabTime += Time.deltaTime;
                if(CurrentGrabTime >= TimeRequiredToGrabLedge)
                {
                    CurrentGrabTime = 0;
                    CurrentSpecialMove = E_CurrentMode.Ledge_Grabbing;
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 0;
                    float WallJumpDir = (collision.transform.position - transform.position).x;
                    LedgeJumpDir = -(int)Mathf.Sign(WallJumpDir);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == LadderTag)
        {
            ReadyToClimb = false;
            CurrentSpecialMove = 0;
            Climbing_Initialized = false;
            Jumping = false;
            SpritePNG.sortingOrder = 0;
        }

        if (collision.tag == "Ledge")
        {
            CurrentGrabTime = 0;
        }
    }
    #region Walljump state
    private void WallJump()
    {
        isTouchingWall = Physics2D.OverlapCircle(WallCheck.position, WallCheckRadius, GroundLayer);
        if (!isSlidingOnWall && isTouchingWall && !OnGround && Dir != 0 && rb.velocity.y < -0.1f)
        {
            // if the playeris trying to slide down the wall, set isSlidingOnWall to true.
            CurrentSpecialMove = E_CurrentMode.WallSliding;
            isSlidingOnWall = true;
        }
        if (isSlidingOnWall)
        {
            // if the player is trying to break free from wall.
            float WallJumpDir = (WallCheck.position - transform.position).x;
            int WallJumpDirection = (int)(WallJumpDir / Mathf.Abs(WallJumpDir));
            if ((int)Dir != Mathf.Sign(WallJumpDirection))  
            {
                CurrentSlideTimer += Time.deltaTime;
            }
        }
        if (CurrentSlideTimer >= SlideBreakTimer | OnGround | !isTouchingWall)
        {
            ResetWallSlide();
        }

        if (isSlidingOnWall)        // slide player down
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
        }

        if ((WallJumpBufferTimer > Time.time) && (isSlidingOnWall || (isTouchingWall && rb.velocity.y > 0.1f)))
        {
            // perform the actual jump.
            WallJumpBufferTimer = 0;            
            float WallJumpDir = -(WallCheck.position - transform.position).x;
            WallJumpDir /= Mathf.Abs(WallJumpDir);
            //print(WallJumpDir);
            rb.AddForce(WallJumpForce.x * WallJumpDir * Vector2.right, ForceMode2D.Impulse);
            SetYVelocity(Vector2.up*WallJumpForce.y);
        }
    }

    private void ResetWallSlide()
    {
        CurrentSlideTimer = 0;
        CurrentSpecialMove = 0;
        isSlidingOnWall = false;
    }
    #endregion
    #region climbing state
    private void Climb()
    {
        if (!ReadyToClimb)          // There are two ways to climb: Climbing from the bottom of a ladder of from the top of a platform. This bit of code is in case the user wants to climb from on top of the platform
        {
            ReadyToClimbDown = Physics2D.OverlapCircle(BottomLadderCheck.position, BottomLadderCheckRadius, LadderLayer);
            if (ReadyToClimbDown && Y_Dir < 0)      // if you are on a platform and want to climb down a ladder that is under you.
            {
               
                Collider2D Ladder = Physics2D.OverlapCircle(BottomLadderCheck.position, BottomLadderCheckRadius, LadderLayer);

                if(Ladder.GetComponent<ClimbableSurface>() != null && !HorizontalClimbAllowed)
                {
                    Vector3 LadderTopPos = Ladder.GetComponent<ClimbableSurface>().Get_Climb_Down_Position();
                    transform.position = LadderTopPos;
                    LadderPos = Ladder.transform;
                }
            }
            else
            {
                return;
            }
        }

        Ladder_ReachedTop = !Physics2D.OverlapCircle(LadderCheck.position, WallCheckRadius, LadderLayer);
        if (Y_Dir != 0 || (Climbing_Initialized && Dir != 0))
        {
            if (!Climbing_Initialized)          // initializing when you press up.
            {
                Climbing_Initialized = true;
                CurrentSpecialMove = E_CurrentMode.Climbing;
                if (Ladder_HasTop || !HorizontalClimbAllowed)
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
        if(OnGround)
        {
            ResetClimbing();
        }
    }

    private void Climb_Init(Collider2D collision)
    {
        ClimbableSurface newClimbable = collision.GetComponent<ClimbableSurface>();
        HorizontalClimbAllowed = newClimbable.Get_HorizAllowed();
        Ladder_HasTop = newClimbable.Get_HasTop();
        LadderPos = (Ladder_HasTop || !HorizontalClimbAllowed) ? collision.transform : null;
    }
    public void ResetClimbing()
    {
        CurrentSpecialMove = 0;
        Climbing_Initialized = false;
        Jumping = false;
    }
    #endregion

    private void SetYVelocity(Vector2 InputForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(InputForce, ForceMode2D.Impulse);
    }

    private void ResetLedgeGrab()
    {
        CanLedgeGrabAgain = true;
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
        if(DrawBottomLadderCheck)
        {
            Gizmos.color = (ReadyToClimbDown) ? Color.blue : Color.red;
            Vector3 CurrentPos = BottomLadderCheck.position;
            Gizmos.DrawSphere(CurrentPos, BottomLadderCheckRadius);
        }
    }
}
