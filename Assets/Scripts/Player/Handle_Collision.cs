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
    public bool DrawCrushCheck = false;
    public float CrushCheckRadius = 1f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GroundCheck = GetComponent<Movement>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            string name = collision.name.ToLower();
            if (name.Contains("lasso"))
            {
                Destroy(collision.gameObject);
                GetComponent<ThrowLasso>().enabled = true;
                // only slot machine disables this.s
            }
        }
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
            }
            else if(name.Contains("tnt"))
            {
                obj.GetComponent<BlastBox>().InitiateDestruction();
            }
            return;
        }
        /*
        if(obj.layer == 8)
        {
            bool closest = collision.collider.bounds.Contains(transform.position);
            int LayermaskG = 1 << 8;
            var inside = collision.collider.bounds.ClosestPoint(transform.position);
            print(inside);
            //bool ActuallyInside = Physics2D.Raycast(transform.position, Vector2.right,CrushCheckRadius, LayermaskG);
            //print(ActuallyInside);
            CrushCheck = ActuallyInside;

            // Because closest=point if point is inside - not clear from docs I feel
            if (closest)
            {
                print("I'm being crushed!");            // if this piece of code runs, kill the player. THey are stuck inside a block.
            }
        }
        */
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.tag == "Platform")
        {
            string name = obj.name.ToLower();
            if (name.Contains("trap"))
            {
                print(rb.velocity.y);
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
}
