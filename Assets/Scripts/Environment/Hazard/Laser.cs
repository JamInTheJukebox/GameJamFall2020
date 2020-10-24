using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] Collider2D lazerZone; // collider denotating the zone the lazer should detect the player
    [SerializeField] GameObject beam;
    LineRenderer beamLine;
    BoxCollider2D beamCollider;
    float beamLength;
    public float moveTime;
    public float shootDelay;
    public float shootTime;
    GameObject foundPlayer = null;
    float trackedTime = 0;
    bool isShooting = false;
    ContactFilter2D playerFilter;

    void Awake()
    {
        playerFilter = new ContactFilter2D();
        playerFilter.useTriggers = true;
        playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        EventLogger.triggeredCheckpoint += stopShoot;
        beamLine = beam.GetComponent<LineRenderer>();
        beamCollider = beam.GetComponent<BoxCollider2D>();
        beam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isShooting) return;

        if (playerInZone())
        {
            if (trackedTime < moveTime)
                lookatPlayer();

            else if (trackedTime >= moveTime + shootDelay)
            {
                shoot();
                trackedTime = 0;
            }

            trackedTime += Time.deltaTime;
        }
        else
        {
            trackedTime = 0;
            foundPlayer = null;
        }
    }

    bool playerInZone()
    {
        Collider2D[] objs = new Collider2D[1];
        int val = Physics2D.OverlapCollider(lazerZone, playerFilter, objs);
        if (val == 1)
        {
            foundPlayer = objs[0].gameObject;
            return true;
        }
        else if (val > 1)
        {
            Debug.Log("Something weird happened: Players detected" + val);
        }

        return false;
    }

    void lookatPlayer()
    {
        if (foundPlayer == null) return;
        Vector3 dir = foundPlayer.transform.position - transform.position;
        beamLength = dir.magnitude + 1;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, Time.deltaTime * 10);

        // find length beam needs to travel
        RaycastHit2D hit = Physics2D.Linecast(transform.position, foundPlayer.transform.position + dir, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            beamLength = (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude;
        }
    }

    void shoot()
    {
        isShooting = true;
        beam.SetActive(true);
        beamCollider.size = new Vector2(beamLength, beamCollider.size.y);
        beamLine.SetPosition(1, new Vector3(beamLength, 0, 0));
        Invoke("stopShoot", shootTime);
    }

    void stopShoot()
    {
        if (isShooting)
        {
            isShooting = false;
            beam.SetActive(false);
        }
    }
}
