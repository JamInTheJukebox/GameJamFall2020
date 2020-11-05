using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterSystem : MonoBehaviour
{
    [Tooltip("Game objects need to have a collider")]
    public GameObject[] teleporters;
    public float waitTime;
    Teleporter[] tSettings;

    private void Awake()
    {
        tSettings = new Teleporter[teleporters.Length];
        for (int i = 0; i < teleporters.Length; i++)
        {
            Teleporter temp = teleporters[i].AddComponent<Teleporter>();
            temp.waitTime = waitTime;
            temp.index = i;
            temp.teleportEvent += teleporting;
            tSettings[i] = temp;
        }
    }

    void teleporting(int currIndex, GameObject player)
    {
        int nextIndex = (currIndex + 1) % teleporters.Length;
        tSettings[nextIndex].previouslyTeleported = true;
        player.transform.position = teleporters[nextIndex].transform.position;
    }
}
