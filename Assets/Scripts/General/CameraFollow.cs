using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    //private Rigidbody2D rb_Target;
    [SerializeField] Camera Follower;

    private Vector3 velocity = Vector3.zero;
    public float TimeToReach = 0.3f;
    private float Z_Value;                      // Keep Z-Value constant!

    /*
     * The threshold mode only
     */
    private Vector3 EndPos;
    public Vector2 FollowOffset;
    private Vector2 threshold;

    [SerializeField] bool DrawBorder = true;

    [SerializeField] E_mode CurrentMode;
    public enum E_mode
    {
        AlwaysTarget = 0,
        AllowThreshold = 1
    };

    private void Awake()
    {
        if(Follower == null)
           Follower = Camera.main;
        if (CurrentMode == E_mode.AllowThreshold)
        {
            /*
            if (Target.GetComponent<Rigidbody2D>() == null)
            {
                rb_Target = Target.gameObject.AddComponent<Rigidbody2D>();
                Debug.LogWarning("CameraFollower.cs: There is not a rigidbody attached to the Target.");
            }
            rb_Target = Target.GetComponent<Rigidbody2D>();
            */
        }

        Z_Value = transform.position.z;

    }

    private void Start()
    {
        threshold = RetrieveThreshold();
    }
    private void Update()
    {
        if(CurrentMode == E_mode.AllowThreshold)
            RetreiveTargetPos();
    }

    private void RetreiveTargetPos()
    {
        Vector2 TargetPos = Target.position;
        Vector3 FollowerPos = transform.position;
        float X_DeltaPos = Vector2.Distance(Vector2.right * FollowerPos.x, Vector2.right * TargetPos.x);
        float Y_DeltaPos = Vector2.Distance(Vector2.up * FollowerPos.y, Vector2.up * TargetPos.y);
        if (Mathf.Abs(X_DeltaPos) >= threshold.x)
        {
            FollowerPos.x = TargetPos.x;
        }
        if (Mathf.Abs(Y_DeltaPos) >= threshold.y)
        {
            FollowerPos.y = TargetPos.y;
        }

        EndPos = FollowerPos;
    }

    private Vector3 RetrieveThreshold()
    {
        Rect Aspect = Follower.pixelRect;
        Vector2 t = new Vector2(Follower.orthographicSize * Aspect.width / Aspect.height, Follower.orthographicSize);
        t.x -= FollowOffset.x;
        t.y -= FollowOffset.y;
        return t;
    }

    private void OnDrawGizmos()
    {
        if(CurrentMode == E_mode.AllowThreshold && DrawBorder)
        {
            Gizmos.color = Color.blue;
            Vector2 border = RetrieveThreshold();
            Gizmos.DrawWireCube(transform.position, new Vector3(2 * border.x, 2 * border.y, 1));
        }
    }

    void LateUpdate()
    {
        if (Target == null) { return; }
        Vector3 Start_vec = new Vector3(transform.position.x, transform.position.y, Z_Value);
        Vector3 End_vec = Vector3.zero;
        if (CurrentMode == E_mode.AlwaysTarget)
            End_vec = new Vector3(Target.position.x, Target.position.y, Z_Value);
        else if (CurrentMode == E_mode.AllowThreshold)
            End_vec = new Vector3(EndPos.x,EndPos.y,Z_Value);
        SmoothDamp(Start_vec, End_vec);
    }
    private void SmoothDamp(Vector3 Start, Vector3 End)
    {
        transform.position = Vector3.SmoothDamp(Start, End, ref velocity, TimeToReach, 50f);
    }
}
