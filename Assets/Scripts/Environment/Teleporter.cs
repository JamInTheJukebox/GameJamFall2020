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

    private ParticleSystem Effect1;
    private ParticleSystem Effect2;

    private void Awake()
    {
        try
        {
            Effect1 = transform.Find("Hover").GetComponent<ParticleSystem>();
            Effect2 = transform.Find("Pixels").GetComponent<ParticleSystem>();
        }
        catch
        {
            Debug.LogWarning("Teleporter.cs: An effect was not Initialized");
        }

    }
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
            PlayEffect();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopEffect();
            trackedTime = 0;
            teleporterActive = false;
            previouslyTeleported = false;
        }   
    }

    private void PlayEffect()
    {
        if(Effect2 is ParticleSystem && Effect2 is ParticleSystem)
        {
            Effect1.Play();
            Effect2.Play();
        }
    }

    private void StopEffect()
    {
        if (Effect2 is ParticleSystem && Effect2 is ParticleSystem)
        {
            Effect1.Stop();
            Effect2.Stop();
        }
    }
}
