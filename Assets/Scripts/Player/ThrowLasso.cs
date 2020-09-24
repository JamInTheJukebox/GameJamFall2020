using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLasso : MonoBehaviour
{
    // handle special item collisions here(perks, lasso, etc)
    [Header("Lasso")]
    public GameObject Lasso;
    private bool ThrewLasso;
    [HideInInspector] public bool HangingOnLasso;
    private DistanceJoint2D CharJoint;
    private Movement CharMovement;
    private SpecialCharacterMovement SpecialMove;
    public float MaxSegmentLength = 10f;
    public float MinSegmentLength = 0f;
    public float RetractVelocity = 0.1f;
    private float Y_Dir;

    private void Awake()
    {
        this.enabled = false;
        CharMovement = GetComponent<Movement>();
        SpecialMove = GetComponent<SpecialCharacterMovement>();
    }

    private void Update()
    {
        Y_Dir = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShootLasso();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ShootLassoDirectionally();
        }
        if(HangingOnLasso)
        {
            if(CharJoint == null)
            {
                if(GetComponent<DistanceJoint2D>() == null) { return; }
                CharJoint = GetComponent<DistanceJoint2D>();
                CharJoint.autoConfigureDistance = false;
                SpecialMove.enabled = false;
            }
            else
            {
                CharJoint.distance = Mathf.Clamp(CharJoint.distance, 0.5f, MaxSegmentLength);
            }
            bool isGrounded = CharMovement.CheckGrounded();
            if (Input.GetKeyDown(KeyCode.Space) && !isGrounded)
            {
                Jump();
            }

            if (Y_Dir != 0 && !isGrounded)
            {
                CharJoint.distance -= Y_Dir*Time.deltaTime;
            }
        }
    }
    private void ShootLasso()
    {
        if (!ThrewLasso)
        {
            Lasso Bullet = Instantiate(Lasso, transform.position, Quaternion.identity).GetComponent<Lasso>();
            Bullet.Parent = transform;
            int ForceDir = CharMovement.GetDirectionFacing();
            Bullet.Shoot(ForceDir);
            ThrewLasso = true;
        }
        else if (HangingOnLasso)
        {
            ResetLasso();
            Destroy(FindObjectOfType<Lasso>().gameObject);
        }
    }

    private void ShootLassoDirectionally()
    {
        if (!ThrewLasso)
        {
            Lasso Bullet = Instantiate(Lasso, transform.position, Quaternion.identity).GetComponent<Lasso>();
            Bullet.Parent = transform;
            Vector2 ForceDir;
            if(Y_Dir > 0)
            {
                ForceDir = Vector2.up * 1.5f;
            }
            else
            {
                ForceDir = new Vector2(CharMovement.GetDirectionFacing(), 0);
            }

            Bullet.Shoot(ForceDir);
            ThrewLasso = true;
        }
    }

    private void Jump()
    {
        ResetLasso();
        if(FindObjectOfType<Lasso>() != null)
            Destroy(FindObjectOfType<Lasso>().gameObject);
        CharMovement.Jump();
    }

    public void ResetLasso()
    {
        SpecialMove.enabled = true;
        ThrewLasso = false;
        HangingOnLasso = false;
        if(GetComponent<DistanceJoint2D>() != null)
            Destroy(GetComponent<DistanceJoint2D>());
    }
}
