﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Handle_Collision : MonoBehaviour
{
    //System.Action<InputAction.CallbackContext> ActivateElevator;
    private Rigidbody2D rb;
    private Movement GroundCheck;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GroundCheck = GetComponent<Movement>();
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
                obj.GetComponent<Animator>().SetTrigger("Bounce");
                obj.GetComponent<SpawnVFX_Animator>()?.BurstParticle();
            }
            else if (name.Contains("trap"))
            {

                PlatformPanel PlatPanel = obj.GetComponent<PlatformPanel>();
                if (PlatPanel != null)
                    PlatPanel.OpenPanel();
                    //anim.ResetTrigger("Open");
            }
            else if(name.Contains("tnt"))
            {
                obj.GetComponent<BlastBox>().InitiateDestruction();
            }
        }
    }
}
