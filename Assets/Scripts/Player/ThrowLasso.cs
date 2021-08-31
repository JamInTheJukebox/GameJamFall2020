using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLasso : MonoBehaviour
{
    // handle special item collisions here(perks, lasso, etc)
    // you can only shoot a lasso when you are climbing or when you are in the default movement state.
    [Header("Scripts:")]
    public Lasso grappleLasso;

    [Header("Lasso")]
    private bool ThrewLasso;
    [HideInInspector] public bool HangingOnLasso;
    public float MaxSegmentLength = 10f;
    public float MinSegmentLength = 0f;
    public float RetractVelocity = 0.1f;
    private float Y_Dir;
    [HideInInspector] public float LatchDragFactor = 2.5f;
    [SerializeField] GameObject LassoPointerPrefab;
    Transform LassoPointerObj;

    [Header("Descent Check")]
    [Tooltip("Make sure we do not descend onto water.")]
    [SerializeField] Transform FeetPosition;
    [SerializeField] float RadiusCheck;
    bool ApproachingWater;
    bool ApproachingLand;
    List<GameObject> DiscoveredLatches = new List<GameObject>();       // manage all the targets on screen.
    GameObject PriorityTarget;  // the object we will latch on to.
    // References
    private DistanceJoint2D CharJoint;
    private Movement CharMovement;
    private SpecialCharacterMovement SpecialMove;
    private Rigidbody2D Char_rb;

    
    /*    
     *    
    [Header("Main Camera")]
    public Camera m_camera;

    [Header("Transform Refrences:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 80)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = true;
    [SerializeField] private float maxDistance = 4;

    [Header("Launching")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType Launch_Type = LaunchType.Transform_Launch;
    [Range(0, 5)] [SerializeField] private float launchSpeed = 5;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoCongifureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequency = 3;*/
    private void Awake()
    {
        this.enabled = false;
        CharMovement = GetComponentInParent<Movement>();
        SpecialMove = CharMovement.GetComponent<SpecialCharacterMovement>();
        Char_rb = CharMovement.GetComponent<Rigidbody2D>();
        LassoPointerObj = Instantiate(LassoPointerPrefab).transform;
        LassoPointerObj.gameObject.SetActive(false);
        grappleLasso.enabled = false;
    }

    private void Update()
    {
        if (!CanThrowLasso()) { return; }

        if(!grappleLasso.enabled)
            SetPrioritizedTarget();

        if(GetPrioritizedTarget() == null) { return; }

        Y_Dir = Movement.PlayerInput.Vertical;
        if (Movement.PlayerInput.LassoTriggered())
        {
            if (!grappleLasso.enabled)
            {
                grappleLasso.enabled = true;
                HangingOnLasso = true;
            }
            else
                ResetLasso();
        }
        
        if(HangingOnLasso)
        {
            HangOnLasso();
        }
    }

    private void HangOnLasso()
    {
        if (CharJoint == null)
        {
            if (CharMovement.GetComponent<DistanceJoint2D>() == null) { CharJoint = CharMovement.gameObject.AddComponent<DistanceJoint2D>(); }
            else
                CharJoint = CharMovement.GetComponent<DistanceJoint2D>();
            CharJoint.autoConfigureDistance = true;
            Invoke("TurnOffAutoConfig", 0.1f);
            SpecialMove.enabled = false;
            LatchDragFactor = 2.5f;
        }
        else
        {
            CharJoint.connectedAnchor = GetPrioritizedTarget().position;
            CharJoint.distance = Mathf.Clamp(CharJoint.distance, MinSegmentLength, MaxSegmentLength);
        }
        bool isGrounded = CharMovement.CheckGrounded();
        if (Movement.PlayerInput.JumpTriggered() && !isGrounded)
        {
            Jump();
        }

        if (Y_Dir != 0 && !isGrounded)
        {
            CharJoint.distance -= Y_Dir * Time.deltaTime;
        }
        else if (Y_Dir > 0 && isGrounded)
        {
            Char_rb.AddForce(new Vector2(0, 1f), ForceMode2D.Impulse);
            CharJoint.autoConfigureDistance = true;
            Invoke("TurnOffAutoConfig", 0.1f);
        }
    }

    private void FixedUpdate()
    {
        float X_dir = Movement.PlayerInput.Horizontal;

        if (X_dir == 0 && HangingOnLasso)
        {
            Char_rb.drag = LatchDragFactor;
        }
    }

    private void TurnOffAutoConfig()
    {
        if(CharJoint == null)
        {
            ResetLasso();
            return;
        }
            CharJoint.autoConfigureDistance = false;
    }
    private bool CanThrowLasso()
    {
        if((int)SpecialMove.CurrentSpecialMove == 2 || (int)SpecialMove.CurrentSpecialMove == 0)
        {
            return true;
        }
        ThrewLasso = false;
        return false;
    }
    public bool VerifyLasso()
    {
        if ((int)SpecialMove.CurrentSpecialMove == 2 || (int)SpecialMove.CurrentSpecialMove == 0)
        {
            SpecialMove.ResetClimbing();
            return true;
        }
        return false;
    }

    private void SpawnLasso(out Lasso Bullet)
    {
        grappleLasso.gameObject.SetActive(true);
        Bullet = grappleLasso;
    }

    private void Jump()
    {
        ResetLasso();
        CharMovement.Jump();
    }

    public void ResetLasso()
    {
        SpecialMove.enabled = true;
        HangingOnLasso = false;
        PriorityTarget = null;
        grappleLasso.enabled = false;
        if(CharJoint != null)
            Destroy(CharJoint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Lasso_Latch")
        {
            if(!DiscoveredLatches.Contains(collision.gameObject))
            {
                DiscoveredLatches.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Lasso_Latch")
        {
            if (DiscoveredLatches.Contains(collision.gameObject))
                DiscoveredLatches.Remove(collision.gameObject);
        }
    }

    public void SetPrioritizedTarget()
    {
        //rules for max priority:
        // if you are above a latch, DO NOT PRIORITIZE.
        // Target a Latch with a value of one if you are closer to it on the x axis.
        // Target a Latch with a value of two if the above is true and you are farther away from it on the y axis.
        // Target a Latch with a value of three if the above is true and you are targetting a "key item"(something you can pull, etc).
        // Overall, prioritize (1) x: close, (2) y: far, (3) key items
        if (DiscoveredLatches.Count > 0)
        {
            float[] priority = new float[DiscoveredLatches.Count];
            for (int i = 0; i < DiscoveredLatches.Count; i++)
            {
                // check if key item. If key item, return
                Vector2 Distance = DiscoveredLatches[i].transform.position - transform.position;
                Distance = new Vector2(Mathf.Abs(Distance.x), Distance.y);
                //Distance = Distance.normalized;
                print(Distance);
                float _priority_x = (Distance.x != 0) ? Mathf.Clamp(1/Distance.x, 0, 100) : 100;
                float _priority_y = (Distance.y > 0) ? Distance.y*2000 : -2000;      // DO NOT PRIORITIZE LATCHES UNDER YOU.
                float _priority = _priority_x + _priority_y;
                priority[i] = _priority;
            }

            PriorityTarget = DiscoveredLatches[0];
            int currentTargetIndex = 0;
            for (int i = 1; i < DiscoveredLatches.Count; i++)
            {
                if (priority[currentTargetIndex] < priority[i])
                {
                    currentTargetIndex = i;
                }
            }
            if(priority[currentTargetIndex] < 0) { return; }        // DO NOT TARGET A LATCH UNDER YOU.
            PriorityTarget = DiscoveredLatches[currentTargetIndex];
        }
        else
        {
            ResetPointerPosition();
            return;
        }
        SetPointerPosition(PriorityTarget);
    }
    public Transform GetPrioritizedTarget()
    {
        if(PriorityTarget == null) { return null; }
        return PriorityTarget.transform;
    }

    private void SetPointerPosition(GameObject newLatch)
    {
        if (LassoPointerObj.parent != newLatch)
        {
            LassoPointerObj.gameObject.SetActive(true);
            LassoPointerObj.parent = newLatch.transform;
            LassoPointerObj.localPosition = Vector3.zero;
        }
    }

    private void ResetPointerPosition()
    {
        LassoPointerObj.parent = null;
        LassoPointerObj.gameObject.SetActive(false);
    }

    public void Grapple()
    {
        /*
        if (!launchToPoint && !autoCongifureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequency;
        }

        if (!launchToPoint)
        {
            if (autoCongifureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }
            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }

        else
        {
            if (Launch_Type == LaunchType.Transform_Launch)
            {
                ballRigidbody.gravityScale = 0;
                ballRigidbody.velocity = Vector2.zero;
            }
            if (Launch_Type == LaunchType.Physics_Launch)
            {
                m_springJoint2D.connectedAnchor = grapplePoint;
                m_springJoint2D.distance = 0;
                m_springJoint2D.frequency = launchSpeed;
                m_springJoint2D.enabled = true;
            }
        }*/
    }
}
