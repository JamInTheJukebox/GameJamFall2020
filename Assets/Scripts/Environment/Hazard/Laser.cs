using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] Collider2D lazerZone; // collider denotating the zone the lazer should detect the player
    [SerializeField] GameObject beam;
    private Animator LazerAnim;
    float beamLength;
    Vector3 searchDir;
    public float moveTime;
    public float shootDelay;
    public float shootTime;
    GameObject foundPlayer = null;
    float trackedTime = 0;
    bool isShooting = false;
    public Color ChargeColor = new Color(248,114,114);
    ContactFilter2D playerFilter;
    private SpriteRenderer LazerSprite;
    Vector3 debugPos;

    void Awake()
    {
        playerFilter = new ContactFilter2D();
        playerFilter.useTriggers = true;
        playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        EventLogger.triggeredCheckpoint += stopShoot;
        beam.SetActive(false);
        LazerAnim = GetComponent<Animator>();
        LazerSprite = GetComponent<SpriteRenderer>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(debugPos, 0.2f);
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
            LazerAnim.SetBool("Shooting", true);
            foundPlayer = objs[0].gameObject;
            return true;
        }
        else if (val > 1)
        {
            Debug.Log("Something weird happened: Players detected" + val);
        }

        else if(foundPlayer != null && val == 0)                            // reset back to default state
        {
            LazerAnim.SetBool("Shooting", false);
            LazerSprite.color = Color.white;
        }
        return false;
    }

    void lookatPlayer()
    {
        if (foundPlayer == null) return;
        Vector3 dir = foundPlayer.transform.position - transform.position;
        beamLength = dir.magnitude + 1;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, Time.deltaTime * 10);
        searchDir = foundPlayer.transform.position + dir;
        LazerSprite.color = Color.Lerp(LazerSprite.color, ChargeColor, Time.deltaTime/0.2f);
    }

    void shoot()
    {
        // find length beam needs to travel
        RaycastHit2D hit = Physics2D.Linecast(transform.position, searchDir, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            debugPos = hit.point;
            beamLength = (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude;
        }

        isShooting = true;
        Transform parent = beam.transform.parent;
        beam.transform.SetParent(null);
        beam.transform.localScale = new Vector3(beamLength, beam.transform.localScale.y, beam.transform.localScale.z);
        beam.transform.SetParent(parent);
        beam.SetActive(true);
        Invoke("stopShoot", shootTime);
    }

    void stopShoot()
    {
        if (isShooting)
        {
            LazerAnim.SetBool("Shooting", false);
            isShooting = false;
            beam.SetActive(false);
        }
    }

    private void OnDisable()
    {
        LazerAnim.SetBool("Shooting", false);
        LazerSprite.color = Color.white;
    }
}
