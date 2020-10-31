using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLasso : MonoBehaviour
{
    // handle special item collisions here(perks, lasso, etc)
    // you can only shoot a lasso when you are climbing or when you are in the default movement state.
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
        if (!CheckLasso()) { return; }

        Y_Dir = Movement.PlayerInput.Vertical;
        if (Movement.PlayerInput.LassoTriggered())
        {
            ShootLasso();
        }
        else if (Movement.PlayerInput.LassoDirTriggered())
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
            if (Movement.PlayerInput.JumpTriggered() && !isGrounded)
            {
                Jump();
            }

            if (Y_Dir != 0 && !isGrounded)
            {
                CharJoint.distance -= Y_Dir*Time.deltaTime;
            }
            else if(Y_Dir > 0 && isGrounded)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1f), ForceMode2D.Impulse);
                CharJoint.autoConfigureDistance = true;
                Invoke("TurnOffAutoConfig", 0.1f);
            }
        }
    }

    private void FixedUpdate()
    {
        float X_dir = Movement.PlayerInput.Horizontal;

        if (X_dir == 0 && HangingOnLasso)
        {
            GetComponent<Rigidbody2D>().drag = 2.5f;
        }
    }

    private void TurnOffAutoConfig()
    {
        CharJoint.autoConfigureDistance = false;
    }
    private bool CheckLasso()
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
