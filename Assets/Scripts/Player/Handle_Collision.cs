using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Handle_Collision : MonoBehaviour
{
    //System.Action<InputAction.CallbackContext> ActivateElevator;
    bool InteractTriggered;

    private void Update()
    {
        InteractTriggered = Movement.PlayerInput.RunTriggered();
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
                if (Mathf.Abs(transform.position.y - collision.transform.position.y) < 0.1) return;
                obj.GetComponent<Animator>().SetTrigger("Bounce");
            }
        }
        else if(obj.tag == "Elevator")
        {
            //ActivateElevator += ctx => ToggleElevator(collision);
            //Movement.PlayerInput.Controls.Player.Run.performed += ActivateElevator;
            //Movement.PlayerInput.Controls.Player.Jump.performed -= ActivateElevator;
            transform.parent = collision.transform;
            GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var obj = collision.gameObject;

        if (obj.tag == "Elevator")
        {
            if(InteractTriggered)     // might cause problem
            {
                ToggleElevator(collision);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.tag == "Elevator")
        {
            //Movement.PlayerInput.Controls.Player.Run.performed -= ActivateElevator;
            //ActivateElevator = null;

            transform.parent = null;
            GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }


    private void ToggleElevator(Collision2D collision)
    {
        //print("Activating Elevator");
        if(collision != null)
        {
            PlatformMovement Elevator = collision.gameObject.GetComponent<PlatformMovement>();      // watch out. This is called every time user presses shift on the elevator.
            if (Elevator != null && Elevator.move == null)  
            {
                transform.parent = collision.transform;
                Elevator.toggleMovement();
            }
        }
    }
}
