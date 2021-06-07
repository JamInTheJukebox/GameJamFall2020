using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    Vector3 defaultSpawn;
    Movement CharMovement;
    private void Awake()
    {
        defaultSpawn = transform.position;
        CharMovement = GetComponent<Movement>();
    }

    public void respawnPlayer()
    {
        Transform checkpointPos = EventLogger.undoChanges();

        if (checkpointPos)
        {
            gameObject.transform.position = checkpointPos.position;
            CharMovement.ResetSpeed();
            EventLogger.addLog(EventType.CHECKPOINT, checkpointPos.gameObject);
        }
        else
        {
            gameObject.transform.position = defaultSpawn;
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
