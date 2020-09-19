using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static GameObject currentCheckpoint = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && (!currentCheckpoint || !currentCheckpoint.Equals(gameObject)))
        {
            EventLogger.clearLog();
            EventLogger.addLog(EventType.CHECKPOINT, gameObject);
            currentCheckpoint = gameObject;
        }
    }
}
