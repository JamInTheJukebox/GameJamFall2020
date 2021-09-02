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
    [SerializeField] float refreshTime;
    bool refreshed;
    private bool ThrewLasso;
    [HideInInspector] public bool HangingOnLasso;
    public float MaxSegmentLength = 5f;     // make this variable dependent on a ray that shoots out of the latch>
    private float CurrentMaxSegmentLength;
    
    public float MinSegmentLength = 0.3f;
    public float RetractVelocity = 0.1f;
    private float Y_Dir;
    [HideInInspector] public float LatchDragFactor = 2.5f;
    [SerializeField] GameObject LassoPointerPrefab;
    Transform LassoPointerObj;

    [Header("Descent Check")]
    [Tooltip("Make sure we do not descend onto water.")]
    [SerializeField] Transform FeetPosition;
    [SerializeField] float RadiusCheck;
    [SerializeField] LayerMask LayersToAvoid;
    bool IsApproachingGroundOrWater;

    List<GameObject> DiscoveredLatches = new List<GameObject>();       // manage all the targets on screen.
    GameObject PriorityTarget;  // the object we will latch on to.
    // References
    private DistanceJoint2D CharJoint;
    private Movement CharMovement;
    private SpecialCharacterMovement SpecialMove;
    private Rigidbody2D Char_rb;

    private void Awake()
    {
        this.enabled = false;
        // components
        CharMovement = GetComponentInParent<Movement>();
        SpecialMove = CharMovement.GetComponent<SpecialCharacterMovement>();
        Char_rb = CharMovement.GetComponent<Rigidbody2D>();
        // pointer
        LassoPointerObj = Instantiate(LassoPointerPrefab).transform;
        LassoPointerObj.gameObject.SetActive(false);
        grappleLasso.enabled = false;
        // refresh
        refreshed = true;
    }

    private void Update()
    {
        if (!CanThrowLasso() || !refreshed) { return; }

        if(!grappleLasso.enabled)       // only look for new targets when the grapple isn't enabled.
            SetPrioritizedTarget();

        if (GetPrioritizedTarget() == null)
        {
            if (HangingOnLasso) { ResetLasso(); }
            return;      // do not give the player the ability to press C if we do not have anything to target.
        }
        Y_Dir = Movement.PlayerInput.Vertical;
        if (Movement.PlayerInput.LassoTriggered())
        {
            if (!grappleLasso.enabled)
            {
                grappleLasso.enabled = true;
                HangingOnLasso = true;
                ResetPointerPosition();     // hide animation??
                if (Char_rb.velocity.y > 0)      // player will slow down if they are moving up
                {
                    Char_rb.velocity = new Vector2(Char_rb.velocity.x, -0.1f);
                }

                // Cast a ray straight downwards. Set the max distance.
                SetMaxDistance();
            }
            else
                ResetLasso();
        }
        
        if(HangingOnLasso)
        {
            HangOnLasso();
        }
    }

    private void SetMaxDistance()
    {
        RaycastHit2D hit = Physics2D.Raycast(GetPrioritizedTarget().position, Vector2.down, MaxSegmentLength, 1 << 8);
        float _distance = hit.distance - 0.3710088f;
        if (_distance > 1 && _distance < MaxSegmentLength)
            CurrentMaxSegmentLength = hit.distance - 0.3710088f;
        else { CurrentMaxSegmentLength = MaxSegmentLength; }
    }

    private void HangOnLasso()
    {
        if (CharJoint == null)
        {
            if (CharMovement.GetComponent<DistanceJoint2D>() == null) {
                CharJoint = CharMovement.gameObject.AddComponent<DistanceJoint2D>();
                CharJoint.enableCollision = true;
            }
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
            CharJoint.distance = Mathf.Clamp(CharJoint.distance, MinSegmentLength, CurrentMaxSegmentLength);
        }
        bool isGrounded = CharMovement.CheckGrounded();
        if (Movement.PlayerInput.JumpTriggered() && !isGrounded)
        {
            Jump();
        }
        float DeltaTime = Time.deltaTime;
        IsApproachingGroundOrWater = Physics2D.OverlapCircle(FeetPosition.position, RadiusCheck, LayersToAvoid);

        if (Y_Dir != 0 && !isGrounded)
        {
            if(IsApproachingGroundOrWater && Y_Dir < 0)        // DO NOT KEEP GOING DOWN IF YOU ARE TRYING TO GO INTO WATER OR GROUND.
                CharJoint.distance = CharJoint.distance;
            else
                CharJoint.distance -= Y_Dir * DeltaTime;
        }
        if (IsApproachingGroundOrWater)
        {
            CharJoint.distance -= 2 * DeltaTime;

            //Char_rb.AddForce(new Vector2(0, 3f), ForceMode2D.Impulse);
            ///CharJoint.autoConfigureDistance = true;
            //Invoke("TurnOffAutoConfig", 0.1f);
        }
    }

    private void FixedUpdate()
    {
        if (HangingOnLasso)
        {
            float X_dir = Movement.PlayerInput.Horizontal;
            if (X_dir == 0 && HangingOnLasso)
            {
                Char_rb.drag = LatchDragFactor;
            }
            else if (IsApproachingGroundOrWater && X_dir != 0)
            {
                CharJoint.distance -= Time.deltaTime;
            }
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
        refreshed = false;
        StartCoroutine(StartRefresh());
        if (CharJoint != null)
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
            if (!HangingOnLasso && DiscoveredLatches.Count == 0)
            {
                ResetPriorityTarget();
            }
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
            LatchNullCheck();
            if(DiscoveredLatches.Count == 0) { return; }
            float[] priority = new float[DiscoveredLatches.Count];
            for (int i = 0; i < DiscoveredLatches.Count; i++)
            {
                // check if key item. If key item, return
                Vector2 Distance = DiscoveredLatches[i].transform.position - transform.position;
                Distance = new Vector2(Mathf.Abs(Distance.x), Distance.y);
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
    private void LatchNullCheck()
    {
        for(int i = 0; i < DiscoveredLatches.Count; i++)
        {
            if(DiscoveredLatches[i] == null)
            {
                DiscoveredLatches.RemoveAt(i);
            }
        }
    }
    public Transform GetPrioritizedTarget()
    {
        if(PriorityTarget == null) { return null; }
        return PriorityTarget.transform;
    }

    private void SetPointerPosition(GameObject newLatch)
    {
        LassoPointerObj.gameObject.SetActive(true);
        LassoPointerObj.position = newLatch.transform.position;
    }

    private void ResetPointerPosition()
    {
        LassoPointerObj.gameObject.SetActive(false);
    }

    public void ResetPriorityTarget()
    {
        PriorityTarget = null;
    }
    IEnumerator StartRefresh()
    {
        yield return new WaitForSeconds(refreshTime);
        refreshed = true;
    }
}
