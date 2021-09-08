using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMovementModifier : MonoBehaviour
{
    // area movementModifiers are persisitent. They do not get destroyed. (Ex: Anti-gravity fields, slow platforms)

    [SerializeField] PlayerState newPlayerState;
    List<Movement> targetPlayers = new List<Movement>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = collision.gameObject.GetComponent<Movement>(); 
            if (!targetPlayers.Contains(player))
            {
                targetPlayers.Add(player);
                player.ChangePlayerState(newPlayerState);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = collision.gameObject.GetComponent<Movement>();
            if (targetPlayers.Contains(player))
            {
                targetPlayers.Remove(player);
                player.ResetPlayerState();
            }
        }
    }
}
