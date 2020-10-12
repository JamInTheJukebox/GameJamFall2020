using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    public float speed;
    public float rotateSpeed;
    private Rigidbody2D rb;
    [HideInInspector] public bool Homing;
    [HideInInspector] public Transform Target;

    private void Start()
    {
        Destroy(gameObject,10f);
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Homing && Target != null)
        {
            Vector2 Dir = (Vector2)Target.position - rb.position;
            Dir.Normalize();
            float rotateAmount = Vector3.Cross(Dir, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
        }
        rb.velocity = transform.up * speed;
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
    private void OnDestroy()
    {
        // play particle effect here
    }
}
