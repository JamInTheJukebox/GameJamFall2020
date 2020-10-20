using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryRigidbody : MonoBehaviour
{
    public List<Rigidbody2D> RigidBodies = new List<Rigidbody2D>();

    public Vector2 LastPos;
    Transform _transform;

    private void Start()
    {
        _transform = transform;
        LastPos = _transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if(rb == null) { return; }
        AddRigidBody(rb);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) { return; }
        RemoveRigidBody(rb);
    }

    private void LateUpdate()
    {
        if(RigidBodies.Count > 0)
        {
            UpdateBody();   
        }

        LastPos = (Vector2)_transform.position;
    }

    private void UpdateBody()
    {
        foreach(Rigidbody2D rb in RigidBodies)
        {
            Vector2 vel = ((Vector2)_transform.position - LastPos);
            rb.transform.Translate(vel, _transform);
        }
    }
    void AddRigidBody(Rigidbody2D rb)
    {
        if(!RigidBodies.Contains(rb))
            RigidBodies.Add(rb);
    }

    void RemoveRigidBody(Rigidbody2D rb)
    {
        if (RigidBodies.Contains(rb))
            RigidBodies.Remove(rb);
    }
}
