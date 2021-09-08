using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_keyItems
{
    Nothing = 0,
    Lasso = 1,
    // add future key items here.
}

public class KeyItem : MonoBehaviour
{
    [SerializeField] e_keyItems typeOfItem;
    public e_keyItems getTypeOfItem()
    {
        return typeOfItem;
    }

    public virtual void EnableItem()
    {
        // enable Key Item on player
        gameObject.SetActive(true);
    }

    public virtual void DisableItem()
    {
        gameObject.SetActive(false);
    }

    public virtual void ResetItem()
    {
        // each item has a reset state.
    }

    public virtual bool UsingItem()
    {
        // hanging on lasso? this returns true.
        return false;
    }
}
