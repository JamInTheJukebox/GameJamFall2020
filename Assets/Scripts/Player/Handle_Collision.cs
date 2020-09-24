using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle_Collision : MonoBehaviour
{

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
                obj.GetComponent<Animator>().SetTrigger("Bounce");
            }
        }
    }
}
