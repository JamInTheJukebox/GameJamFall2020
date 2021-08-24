using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Handle_Collision : MonoBehaviour
{
    //System.Action<InputAction.CallbackContext> ActivateElevator;
    private Rigidbody2D rb;
    private Movement GroundCheck;
    private bool TrapCol = false;
    public GameObject LassoUI;
    public bool DrawCrushCheck = false;
    public float CrushCheckRadius = 1f;
    private float Jump_Delay = 0.25f;
    public float AdditiveSpringJumpForce;
    public Transform CrushCheck;

    public float DisplacementFactor = 0.1f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GroundCheck = GetComponent<Movement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.name);
        if (collision.tag == "Item")
        {
            string name = collision.name.ToLower();
            if (name.Contains("lasso"))
            {
                Destroy(collision.gameObject);
                GetComponent<ThrowLasso>().enabled = true;
                TurnOnLasso();
                // only slot machine disables this.s
            }
        }
        if(collision.name.ToLower().Contains("tnt"))
            if(collision.GetComponent<BlastBox>())
                collision.GetComponent<BlastBox>().InitiateDestruction();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.tag == "Platform")
        {
            string name = obj.name.ToLower();
            if (name.Contains("bouncy"))
            {
                bool cond = transform.position.y - collision.transform.position.y >= 0.405  && rb.velocity.y >= 0.01f;     // activate only if the player is moving down and they are above the spring. Change this to a collider if this is error prone.
                if (!cond) { return; }
                obj.GetComponent<Animator>().SetTrigger("Bounce");
                obj.GetComponent<SpawnVFX_Animator>()?.BurstParticle();

                if(GroundCheck.GetJumpTimer() > Time.time)
                {
                    rb.AddForce(Vector2.up * AdditiveSpringJumpForce, ForceMode2D.Impulse);
                }
            }
            else if(name.Contains("tnt"))
            {
                obj.GetComponent<BlastBox>().InitiateDestruction();
            }
            return;
        }
        
        if(obj.layer == 8)      // CRUSH CHECK
        {
            bool closest = collision.collider.bounds.Contains(transform.position);/*
            Debug.LogWarning(collision.gameObject.name);
            
            // if the collider is a composite, get any colliders underneath that tile
            if (collision.collider is CompositeCollider2D)
            {

                List<Collider2D> hitColliders = new List<Collider2D>();
                Vector3 hitPosition = Vector3.zero;

                print(collision.collider.bounds.Contains(transform.position));
                foreach (ContactPoint2D hit in collision.contacts)
                {
                    print("Normal Y:" + hit.normal.y);
                    print("Normal X:" + hit.normal.x);
                    // move the hit location inside the collider a bit

                    hitPosition.x = hit.point.x - DisplacementFactor * hit.normal.x;
                    hitPosition.y = hit.point.y - DisplacementFactor * hit.normal.y;
                    hitColliders.AddRange(Physics2D.OverlapPointAll(hitPosition,1<<8));
                }
                
                foreach(var hit in hitColliders)
                {
                    Debug.LogWarning(hit.gameObject.name);
                    Debug.LogWarning(transform.position);
                    Debug.LogWarning(hit.transform.position);
                    Debug.LogWarning(Vector2.Distance(transform.position, hit.transform.position));
                }
                
                // use hitColliders as a list of all colliders under the hit location
            }
            */
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.tag == "Platform")
        {
            string name = obj.name.ToLower();
            if (name.Contains("trap"))
            {
                if (TrapCol | (!GroundCheck.CheckGrounded() | Mathf.Abs(rb.velocity.y) > 0.01f)) { return; }
                print(GroundCheck.CheckGrounded());
                PlatformPanel PlatPanel = obj.GetComponent<PlatformPanel>();
                if (PlatPanel != null)
                {
                    TrapCol = true;                         // means we are on a platforms
                    PlatPanel.OpenPanel();
                }
                //anim.ResetTrigger("Open");
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.tag == "Platform")
        {
            string name = obj.name.ToLower();
            if (name.Contains("trap"))
            {
                TrapCol = false;
            }
        }
    }

    public void TurnOnLasso()
    {
        if (LassoUI == null) return;
        LassoUI.GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }
}
