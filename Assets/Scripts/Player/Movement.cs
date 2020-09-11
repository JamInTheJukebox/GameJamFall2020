using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    // This script is in charge of handling Horizontal Movement and Jumping.
    private Rigidbody2D rb;
    private SpecialCharacterMovement SpecMove;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SpecMove = GetComponent<SpecialCharacterMovement>();
    }

    private void Update()
    {
        Dir = Input.GetAxis("Horizontal");
        onGround = Physics2D.Raycast(transform.position + RayCastOffset, Vector2.down, GroundLength, GroundLayer)
            || Physics2D.Raycast(transform.position - RayCastOffset, Vector2.down, GroundLength, GroundLayer);
        if (Input.GetKeyDown(KeyCode.Space) && SpecMove.CurrentSpecialMove == 0)
        {
            JumpTimer = Time.time + JumpDelay;      // A jump timer allows for a treshold for timing jumps. When a player gets on the ground
            // There is a chance that their input will be dropped due to them not being "technically" grounded yet. This timer allows for them to jump without being frame perfect.
        }
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

    private void HorizontalMovement()
    {
        float MaxSpeed = Input.GetKey(KeyCode.LeftShift) ? MaxRunSpeed : MaxWalkSpeed; 
        float MoveSpeed = Input.GetKey(KeyCode.LeftShift) ? RunAcceleration : WalkAcceleration;
        if (Movement_Type == E_MovementType.Linear)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) { MoveSpeed = MaxSpeed;


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

    private void FlipDirection()
    {
        FacingRight = !FacingRight;
        transform.rotation = Quaternion.Euler(0, FacingRight ? 0 : 180, 0);
    }

    private void Jump()
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
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
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
