using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementModifier : MonoBehaviour
{
    // used by powerups, slow platforms, etc to change how the player moves.
    public enum e_propertyModifier
    {
        speed = 0,
        jump = 1,
        jumpCount = 2,
    }

    [SerializeField] propertyModifier[] modifiers = new propertyModifier[1];
    [SerializeField] float ResetTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var targetPlayer = collision.gameObject.GetComponent<Movement>();
            foreach(propertyModifier mod in modifiers)
            {
                switch (mod.modifier)
                {
                    case e_propertyModifier.speed:
                        targetPlayer.ModifySpeed(mod.value, ResetTime);
                        break;
                    case e_propertyModifier.jump:
                        targetPlayer.ModifyJump(mod.value, ResetTime);
                        break;
                    case e_propertyModifier.jumpCount:
                        targetPlayer.ModifyJumpCount(mod.value, ResetTime);
                        break;
                }
            }

            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class propertyModifier
    {
        public e_propertyModifier modifier;
        public float value;
    }
}
