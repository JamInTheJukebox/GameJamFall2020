using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCountUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI counter;
    public ItemType countedItem;

    private void Awake()
    {
        counter.text = "00";
        ItemTracker.itemCountEvent += updateItemCounter;
    }

    public void updateItemCounter(ItemType item, int counted)
    {
        if (item == countedItem)
        {
            string countText = counted.ToString();
            counter.text = counted < 10 ? "0" + countText : countText;
        }
    }

}
