using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Vector3 defaultSpawn;

    public void respawnPlayer()
    {
        Transform checkpointPos = EventLogger.undoChanges();

        if (checkpointPos)
        {
            gameObject.transform.position = checkpointPos.position;
            EventLogger.addLog(EventType.CHECKPOINT, checkpointPos.gameObject);
        }
        else
        {
            gameObject.transform.position = defaultSpawn;
        }   
    }
}
