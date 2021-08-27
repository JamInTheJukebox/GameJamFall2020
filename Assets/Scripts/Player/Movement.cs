using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    // This script is in charge of handling Horizontal Movement and Jumping.
    private Rigidbody2D rb;
    private SpecialCharacterMovement SpecMove;
    public ParticleSystem DustParticle;
    [SerializeField] PlayerState DefaultPlayerState;
    [SerializeField] PlayerState CurrentPlayerState;
    public float LinearDrag = 0.6f;

    [Header("Walking/Running")]

    private float Dir = 0;
    public E_MovementType Movement_Type = E_MovementType.Linear;
    private bool FacingRight = true;

    public enum E_MovementType
    {
        Linear = 0,
        Force = 1,
    }
    [Space(1)]
    [Header("Jumping")]
    [SerializeField] LayerMask GroundLayer = 1 << 8;
    bool onGround = false;
    public float GroundLength = 0.6f;
    [SerializeField] Vector3 RayCastOffset = Vector3.zero;

    [SerializeField] float FallMultiplier = 4;
    [SerializeField] float JumpDelay = 0.25f;

    [SerializeField] int MaxJumps = 1;
    int availableJumps = 1;
    bool multipleJump;

    public float CoyoteTime = 0.2f;
    private bool CoyoteJumpActive;
    private float JumpBufferTimer = 0;
    [Space(1)]
    [Header("DebugTools")]
    [SerializeField] bool DrawGroundCheck = false;

    public static PlayerInputs PlayerInput;

    private void Awake()
    {
        availableJumps = MaxJumps;
        rb = GetComponent<Rigidbody2D>();
        SpecMove = GetComponent<SpecialCharacterMovement>();
        DustParticle.Stop();
        PlayerInput = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        Dir = PlayerInput.Horizontal;
        if (PlayerInput.JumpTriggered() && SpecMove.CurrentSpecialMove == 0)
        {
            JumpBufferTimer = Time.time + JumpDelay;      // A jump timer allows for a treshold for timing jumps. When a player gets on the ground
            // There is a chance that their input will be dropped due to them not being "technically" grounded yet. This timer allows for them to jump without being frame perfect.
        }

        CreateDust();

    }

    private void FixedUpdate()
    {
        GroundCheck();
        // You can easily detach
        if (SpecMove.CurrentSpecialMove == 0 || (int)SpecMove.CurrentSpecialMove == 4) // Applicable ONLY when the player is not climbing or WallJumping
        {
            HorizontalMovement();
            if(SpecMove.CurrentSpecialMove == 0)
                ModifyPhysics();
        }

        HandleJumping();

        if (rb.velocity.y <= -CurrentPlayerState.GetMaxFallSpeed())
        {
            rb.velocity = new Vector2(rb.velocity.x, -CurrentPlayerState.GetMaxFallSpeed());
        }
    }
    #region Jumping
    // jump buffering, coyote jump, multiple jumps.

    void HandleJumping()
    {
        if(JumpBufferTimer > Time.time)
        {
            if (onGround)       // first jump
            {
                multipleJump = true;
                Jump();
            }
            else
            {                   // jumps after first jump or coyote jump.
                if(availableJumps > 0)
                {
                    if (CoyoteJumpActive || multipleJump)
                    {
                        multipleJump = true;
                        Jump();
                    }
                }
            }
        }
    }

    public void Jump()
    {
        availableJumps--;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * CurrentPlayerState.GetJumpImpulse(), ForceMode2D.Impulse);
        JumpBufferTimer = 0;
    }

    private void GroundCheck()
    {
        bool wasGrounded = onGround;
        onGround = false;

        onGround = Physics2D.Raycast(transform.position + RayCastOffset, Vector2.down, GroundLength, GroundLayer)
    || Physics2D.Raycast(transform.position - RayCastOffset, Vector2.down, GroundLength, GroundLayer);

        if(onGround)
        {
            // multiple jumps?
            if(!wasGrounded)
            {
                // reset the jump. You have landed! By the next physics tick, was grounded will be true!
                availableJumps = MaxJumps;
                multipleJump = false;
            }
        }
        else
        {
            if (wasGrounded)
                StartCoroutine(CoyoteJumpDelay());
        }
    
    }

    IEnumerator CoyoteJumpDelay()
    {
        CoyoteJumpActive = true;
        print("Activating Coyote");
        yield return new WaitForSeconds(CoyoteTime);
        CoyoteJumpActive = false;
    } 
    public float GetJumpTimer()
    {
        return JumpBufferTimer;
    }

    #endregion

    #region WillNotBeNeededForNow
    private void OnDisable()        // this was just in case we happen to delete a tile while a character is on it.
    {
        Invoke("EnableAgain", 0.05f);
    }
    private void EnableAgain()
    {
        transform.parent = null;
        gameObject.SetActive(true);
    }

    #endregion

    private void CreateDust()
    {
        bool cond1 = (int)SpecMove.CurrentSpecialMove == 1;
        bool cond2 = rb.velocity.magnitude > CurrentPlayerState.GetMaxWalkSpeed();

        if(cond1 | cond2)
        {
            var main = DustParticle.main;
            var emmision = DustParticle.emission;
            DustParticle.Play();
            if (cond1)
            {
                DustParticle.transform.localPosition = SpecMove.WallCheck.localPosition;

                emmision.rateOverDistanceMultiplier = 180;
                main.startLifetime = 1f;
                // cbhange pos here
            }
            else if (cond2)
            {
                emmision.rateOverDistanceMultiplier = 80;
                DustParticle.transform.localPosition = new Vector3(0, -0.1896f, 0);

                main.startLifetime = 0.5f;
            }
        }
        else
        {
            DustParticle.Stop();
        }
    }

    #region movement
    private void HorizontalMovement()
    {
        float MaxSpeed = ManageMaxSpeed();
        float Acceleration = PlayerInput.Running ? CurrentPlayerState.GetRunAcceleration() : CurrentPlayerState.GetWalkAcceleration();
        if((int)SpecMove.CurrentSpecialMove == 4)
        {
            MaxSpeed = CurrentPlayerState.GetMaxWalkSpeed();
            Acceleration = CurrentPlayerState.GetRunAcceleration();
        }
        if (Movement_Type == E_MovementType.Linear)
        {
            if (PlayerInput.RunTriggered())
            {
                Acceleration = MaxSpeed;
            }
            Vector2 newMove = new Vector2(Dir * Acceleration, rb.velocity.y);
            rb.velocity = newMove;
        }
        else if (Movement_Type == E_MovementType.Force)                                 // Force-Based movement will be necessary for adding hazards that slow or speed you up.
        {
            Vector2 AccumulativeForce = Vector2.right * Acceleration * Dir;
            if (Mathf.Abs(rb.velocity.x) >= MaxSpeed)
            {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * MaxSpeed, rb.velocity.y);
                AccumulativeForce = Vector2.zero;
            }
            rb.AddForce(AccumulativeForce);


            if (Dir > 0 && !FacingRight || Dir < 0 && FacingRight)
            {
                FlipDirection();
            }
        }
    }

    private float ManageMaxSpeed()
    {
        if(PlayerInput.Running)
        {
            return CurrentPlayerState.GetMaxRunSpeed();
        }
        else
        {
            if(Mathf.Abs(rb.velocity.x) > CurrentPlayerState.GetMaxWalkSpeed() && !onGround)
            {
                return CurrentPlayerState.GetMaxRunSpeed();
            }
            else { return CurrentPlayerState.GetMaxWalkSpeed(); }
        }
    }

    private void FlipDirection()
    {
        FacingRight = !FacingRight;
        transform.rotation = Quaternion.Euler(0, FacingRight ? 0 : 180, 0);
    }

    public int GetDirectionFacing()
    {
        return (FacingRight) ? 1 : -1;
    }


    private void ModifyPhysics()
    {
        bool ChangingDirections = (Dir > 0 && rb.velocity.x < 0) || (Dir < 0 && rb.velocity.x > 0);
        if (onGround)
        {
            if (Mathf.Abs(Dir) < 0.4f || ChangingDirections)
            {
                rb.drag = LinearDrag;
            }
            else { rb.drag = 0; }

            rb.gravityScale = 0;

        }
        else
        {
            float gravity = CurrentPlayerState.GetGravityScale();
            rb.gravityScale = gravity;
            rb.drag = LinearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * FallMultiplier;
            }
            else if (rb.velocity.y > 0 && !PlayerInput.Jumping)
            {
                rb.gravityScale = gravity * (FallMultiplier / 2);
            }
        }
    }
    
    public void ChangePlayerState(PlayerState newPlayerState)
    {
        CurrentPlayerState = newPlayerState;
        rb.gravityScale = newPlayerState.GetGravityScale();
    }
    // TODO: change player speed for x seconds?
    public void ResetPlayerState()
    {
        CurrentPlayerState = DefaultPlayerState;
        rb.gravityScale = DefaultPlayerState.GetGravityScale();
    }

    public PlayerState GetDefaultPlayerState()
    {
        return DefaultPlayerState;
    }
    #endregion
    private void OnDrawGizmos()
    {
        if(DrawGroundCheck)
        {
            Gizmos.color = (onGround) ? Color.green : Color.red;
            Vector3 CurrentPos = transform.position;
            Gizmos.DrawLine(CurrentPos + RayCastOffset, CurrentPos + Vector3.down * GroundLength + RayCastOffset);
            Gizmos.DrawLine(CurrentPos - RayCastOffset, CurrentPos + Vector3.down * GroundLength - RayCastOffset);
        }
    }

    public bool CheckGrounded()
    {
        return onGround;
    }

}
