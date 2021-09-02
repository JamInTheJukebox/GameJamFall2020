using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    // manages motion for the rocket.
    public float speed;             
    public float rotateSpeed;           // torque exerted on the rocket to rotate.
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    [HideInInspector] public bool Homing;
    [HideInInspector] public Transform Target;
    [HideInInspector] public float lifetime;
    private float WarningTime;
    public Color BaseColor = Color.white;
    public Color WarningColor = Color.red;          // color to show that the rocket is about to blow up.
    private float CollectiveTime = 0;
    private bool ShowWarning = false;
    HazardDamage explo;
    private void Start()
    {
        WarningTime = lifetime * 0.75f;
        StartCoroutine(ShowWarningNow());
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        explo = GetComponentInChildren<HazardDamage>();
        StartCoroutine(timedDestroy());
    }

    IEnumerator timedDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        explo.destroyProjectile();
    }

    private IEnumerator ShowWarningNow()
    {
        yield return new WaitForSeconds(WarningTime);
        while(true)
        {
            yield return new WaitForSeconds(0.3f); sp.color = WarningColor;
            yield return new WaitForSeconds(0.3f); sp.color = Color.white;

        }
    }
    void FixedUpdate()
    {
        if (Homing && Target != null)
        {
            Vector2 Dir = (Vector2)Target.position - rb.position;
            Dir.Normalize();
            float rotateAmount = Vector3.Cross(Dir, transform.right).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
        }
        else if (Homing && Target == null) { rb.angularVelocity = 0; }
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Target = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Target = null;
        }
    }
}
