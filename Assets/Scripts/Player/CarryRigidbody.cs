using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryRigidbody : MonoBehaviour
{
    public PlatformMovement ElevatorMovement;       // script that controls movement
    private bool Colliding;
    private GameObject Player;
    private void Update()
    {
        if (Colliding && Movement.PlayerInput.RunTriggered())
        {
            ToggleElevator();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;

        if (obj.tag == "Player")            // turn on only if player is touching collider
        {
            //ActivateElevator += ctx => ToggleElevator(collision);
            //Movement.PlayerInput.Controls.Player.Run.performed += ActivateElevator;
            //Movement.PlayerInput.Controls.Player.Jump.performed -= ActivateElevator;
            Colliding = true;
            Player = obj;
            obj.transform.parent = transform;
            obj.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.tag == "Player")        // dont turn off again until player touches collider and presses shift
        {
            Colliding = false;
            Player = null;
            obj.transform.parent = null;
            obj.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }


    private void ToggleElevator()
    {   
        if (ElevatorMovement != null && ElevatorMovement.move == null)
        {
            ElevatorMovement.toggleMovement();      // if the elevator is not already stationed to move, move the elevator
        }
    }

}
