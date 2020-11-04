using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    public Vector2 Force;
    public float MaximumDistance;
    private float TotalDistanceTraveled;
    [HideInInspector] public Transform Parent;
    [HideInInspector] public Transform Latch;
    private Vector2 LastPos;
    private LineRenderer Lasso_Segment;
    private DistanceJoint2D ParentJoint;
    private bool TargetHit = false;

    private void Awake()
    {
        LastPos = transform.position;
        Lasso_Segment = GetComponent<LineRenderer>();
        
    }

    private void Update()
    {
        if (!TargetHit)
        {
            RecordDistance();

            if (TotalDistanceTraveled > MaximumDistance)
            {
                if(Parent != null)
                    Parent.GetComponent<ThrowLasso>().ResetLasso();
                Destroy(gameObject);
            }
        }
        else
        {
            SetPostions();
            if(ParentJoint != null && Latch != null)
            {
                ParentJoint.connectedAnchor = Latch.position;
            }
            else if(ParentJoint != null && Latch == null)       // a rocket
            {
                DestroyLasso();
            }
        }
    }

    public void Shoot(float Dir)
    {
        Vector2 ForceDir = new Vector2(Force.x * Dir, Force.y);
        GetComponent<Rigidbody2D>().AddForce(ForceDir, ForceMode2D.Impulse);
    }

    public void Shoot(Vector2 Dir)
    {
        Vector2 ForceDir = new Vector2(Force.x * Dir.x, Force.y * Dir.y);
        GetComponent<Rigidbody2D>().AddForce(ForceDir, ForceMode2D.Impulse);
    }
    private void RecordDistance()
    {
        TotalDistanceTraveled += Vector2.Distance(LastPos, transform.position);
        LastPos = transform.position;
    }

    public void DestroyLasso()
    {
        Parent.GetComponent<ThrowLasso>().ResetLasso();
        Destroy(gameObject);
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Lasso_Latch")
        {
            var ParentComp = Parent.GetComponent<ThrowLasso>();
            if (!ParentComp.VerifyLasso()) { Destroy(gameObject); return; }
            //if(Vector2.Distance(Parent.position,collision.transform.position) > 20) { DestroyLasso(); }
            float DragMagnitude = 2.5f;
            Destroy(GetComponent<TrailRenderer>());
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<CircleCollider2D>());
            TargetHit = true;
            ParentComp.HangingOnLasso = true;
            if (collision.transform.name.ToLower().Contains("rocket"))
            {
                DragMagnitude = 1;
            }
            ParentComp.LatchDragFactor = DragMagnitude;              // drag in a rocket looks wierd!

            ParentJoint = Parent.gameObject.AddComponent<DistanceJoint2D>();
            ParentJoint.maxDistanceOnly = true;
            ParentJoint.enableCollision = true;
            ParentJoint.breakForce = 600f;
            Latch = collision.transform;
            ParentJoint.connectedAnchor = Latch.position;
            SetPostions();
        }
        else
        {
            Destroy(gameObject);            // failed to hit target.
        }
    }

    private void SetPostions()
    {
        if(Parent == null | Latch == null)
        {
            Debug.LogWarning("Lasso.cs: Error. Either parent or latch are null");
            return;
        }
        Vector3[] Pos = new Vector3[2]
        {
                new Vector3(Parent.position.x,Parent.position.y,0),
                new Vector3(Latch.position.x, Latch.position.y,0)
        };
        Lasso_Segment.SetPositions(Pos);
    }
}
