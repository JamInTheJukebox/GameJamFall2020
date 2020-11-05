using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public int index;
    public float waitTime;
    public bool previouslyTeleported = false;
    float trackedTime = 0;
    bool teleporterActive = false;
    GameObject player;

    public delegate void teleporting(int currIndex, GameObject player);
    public event teleporting teleportEvent;

    // Update is called once per frame
    void Update()
    {
        if (teleporterActive)
        {
            trackedTime += Time.deltaTime;
            if (trackedTime >= waitTime)
            {
                teleportEvent(index, player);
                trackedTime = 0;
                teleporterActive = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !previouslyTeleported)
        {
            player = collision.gameObject;
            teleporterActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            trackedTime = 0;
            teleporterActive = false;
            previouslyTeleported = false;
        }   
    }
}
