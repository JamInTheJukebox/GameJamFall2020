using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    // This script is in charge of handling Horizontal Movement and Jumping.
    private Rigidbody2D rb;
    private SpecialCharacterMovement SpecMove;
    public ParticleSystem DustParticle;

    public float LinearDrag = 0.6f;
    
    [Header("Walking/Running")]
    public float WalkAcceleration = 5f;
    public float MaxWalkSpeed = 4f;

    public float RunAcceleration = 10f;
    public float MaxRunSpeed = 10;

    private float Dir = 0;
    public E_MovementType Movement_Type = E_MovementType.Linear;
    private bool FacingRight = true;

    public enum E_MovementType
    {
        Linear = 0,
        Force = 1,
    }

    [Header("Jumping")]
    [SerializeField] LayerMask GroundLayer = 1 << 8;
    [SerializeField] bool onGround = false;
    public float GroundLength = 0.6f;
    [SerializeField] Vector3 RayCastOffset = Vector3.zero;
    public float JumpImpulse = 15f;
    [SerializeField] float gravity = 1;
    [SerializeField] float FallMultiplier = 4;
    [SerializeField] float JumpDelay = 0.25f;
    [SerializeField] float MaxFallSpeed = 10f;

    private float JumpTimer = 0;

    [Header("DebugTools")]
    [SerializeField] bool DrawGroundCheck = false;

    public static PlayerInputs PlayerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SpecMove = GetComponent<SpecialCharacterMovement>();
        DustParticle.Stop();
        PlayerInput = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        Dir = PlayerInput.Horizontal;
        onGround = Physics2D.Raycast(transform.position + RayCastOffset, Vector2.down, GroundLength, GroundLayer)
            || Physics2D.Raycast(transform.position - RayCastOffset, Vector2.down, GroundLength, GroundLayer);
        if (PlayerInput.JumpTriggered() && SpecMove.CurrentSpecialMove == 0)
        {
            JumpTimer = Time.time + JumpDelay;      // A jump timer allows for a treshold for timing jumps. When a player gets on the ground
            // There is a chance that their input will be dropped due to them not being "technically" grounded yet. This timer allows for them to jump without being frame perfect.
        }

        CreateDust();
        
    }

    private void FixedUpdate()
    {
        // You can easily detach
        if(SpecMove.CurrentSpecialMove == 0) // Applicable ONLY when the player is not climbing or WallJumping
        {
            HorizontalMovement();
            ModifyPhysics();
        }

        if (JumpTimer > Time.time && onGround)
        {
            Jump();
        }
        
        if(rb.velocity.y <= -MaxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -MaxFallSpeed);
        }
    }

    public float GetJumpTimer()
    {
        return JumpTimer;
    }

    private void OnDisable()
    {
        Invoke("EnableAgain", 0.05f);
    }
    private void EnableAgain()
    {
        transform.parent = null;
        gameObject.SetActive(true);
    }

    private void CreateDust()
    {
        bool cond1 = (int)SpecMove.CurrentSpecialMove == 1;
        bool cond2 = rb.velocity.magnitude > MaxWalkSpeed;

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


    private void HorizontalMovement()
    {
        float MaxSpeed = ManageMaxSpeed();

        float MoveSpeed = PlayerInput.Running ? RunAcceleration : WalkAcceleration;
        if (Movement_Type == E_MovementType.Linear)
        {
            if (PlayerInput.RunTriggered())
            {
                MoveSpeed = MaxSpeed;
            }
            Vector2 newMove = new Vector2(Dir * MoveSpeed, rb.velocity.y);
            rb.velocity = newMove;
        }
        else if (Movement_Type == E_MovementType.Force)                                 // Force-Based movement will be necessary for adding hazards that slow or speed you up.
        {
            Vector2 AccumulativeForce = Vector2.right * MoveSpeed * Dir;
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
            return MaxRunSpeed;
        }
        else
        {
            if(Mathf.Abs(rb.velocity.x) > MaxWalkSpeed && !onGround)
            {
                return MaxRunSpeed;
            }
            else { return MaxWalkSpeed; }
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
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * JumpImpulse, ForceMode2D.Impulse);
        JumpTimer = 0;
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
