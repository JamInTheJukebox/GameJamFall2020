using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardDamage : MonoBehaviour
{
    [SerializeField] bool Projectile;           // projectiles are destroyed upon impact, other types of hazards are not.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<Respawn>().respawnPlayer();
            destroyProjectile();
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            destroyProjectile();
        }
    }

    void destroyProjectile()
    {
        if (Projectile)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
