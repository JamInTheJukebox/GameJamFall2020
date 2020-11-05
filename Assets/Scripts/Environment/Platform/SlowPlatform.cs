using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPlatform : MonoBehaviour
{
    public float slowDownBy = 2;
    Movement targetPlayer;
    float origMaxWalkSpeed;
    float origMaxRunSpeed;

    private void Awake()
    {
        if (slowDownBy < 0)
        {
            slowDownBy *= -1;
        }

        if (slowDownBy == 0)
        {
            slowDownBy = 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            targetPlayer = collision.gameObject.GetComponent<Movement>();
            origMaxRunSpeed = targetPlayer.MaxRunSpeed;
            origMaxWalkSpeed = targetPlayer.MaxWalkSpeed;
            targetPlayer.MaxRunSpeed /= slowDownBy;
            targetPlayer.MaxWalkSpeed /= slowDownBy;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            targetPlayer.MaxRunSpeed = origMaxRunSpeed;
            targetPlayer.MaxWalkSpeed = origMaxWalkSpeed;
        }
    }
}
