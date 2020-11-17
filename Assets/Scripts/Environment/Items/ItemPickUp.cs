using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemType item;
    public bool retrieved = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && !retrieved)
        {
            ItemTracker.addItem(item);
            EventLogger.addLog(EventType.ITEM_RETRIEVED, gameObject);
            deactivate();
            //Debug.Log(ItemTracker.getItemCount(item));
        }
    }

    public void reactivate()
    {
        retrieved = false;
        gameObject.SetActive(true);
    }

    public void deactivate()
    {
        retrieved = true;
        gameObject.SetActive(false);
    }
}
