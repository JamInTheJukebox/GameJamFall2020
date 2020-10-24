using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Handle_Collision : MonoBehaviour
{
    //System.Action<InputAction.CallbackContext> ActivateElevator;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string name = collision.name.ToLower();
        if (collision.tag == "Item")
        {
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
        string name = obj.name.ToLower();
        if (obj.tag == "Platform")
        {
            if (name.Contains("bouncy"))
            {
                bool cond = transform.position.y - collision.transform.position.y >= 0.405  && rb.velocity.y >= 0.01f;     // activate only if the player is moving down and they are above the spring. Change this to a collider if this is error prone.
                if (!cond) { return; }
                print(rb.velocity.y);
                obj.GetComponent<Animator>().SetTrigger("Bounce");
            }
        }
    }
}
