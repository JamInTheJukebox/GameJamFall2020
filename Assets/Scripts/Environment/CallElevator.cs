using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallElevator : MonoBehaviour
{
    public PlatformMovement ElevatorMovement;       // script that controls movement
    private bool Colliding;
    private Animator ButtonAnim;
    int SuccessHash;
    int FailHash;
    private void Awake()
    {
        ButtonAnim = GetComponent<Animator>();
        if(ButtonAnim != null)
        {
            SuccessHash = Animator.StringToHash("PressedSuccess");
            FailHash = Animator.StringToHash("Pressed");

        }
    }
    private void Update()
    {
        if (Colliding && Movement.PlayerInput.RunTriggered())
        {
            ToggleElevator();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;

        if (obj.tag == "Player")            // turn on only if player is touching collider
        {
            Colliding = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var obj = collision.gameObject;

        if (obj.tag == "Player")            // turn on only if player is touching collider
        {
            Colliding = false;
        }
    }

    private void ToggleElevator()
    {
        if (ElevatorMovement != null && ElevatorMovement.move == null && ElevatorMovement.gameObject.activeInHierarchy)
        {
            ButtonAnim.Play(SuccessHash);
            ElevatorMovement.toggleMovement();      // if the elevator is not already stationed to move, move the elevator
        }
        else
            ButtonAnim.Play(FailHash);
    }
}
