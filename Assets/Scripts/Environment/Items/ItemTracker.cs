using System.Collections;
using System.Collections.Generic;
using System;

public enum ItemType
{
    COIN
}

public static class ItemTracker
{
    public delegate void itemCountChange(ItemType item, int counted);
    public static event itemCountChange itemCountEvent;

    static Dictionary<ItemType, int> count;

    static ItemTracker()
    {
        count = new Dictionary<ItemType, int>();
    }

    public static void addItem(ItemType item)
    {
        if (count.ContainsKey(item))
        {
            count[item] += 1;
        }
        else
        {
            count[item] = 1;
        }

        onItemChange(item, count[item]);
    }

    public static void removeItem(ItemType item)
    {
        if (count.ContainsKey(item))
        {
            count[item] -= 1;
            
            if (count[item] < 0)
            {
                count[item] = 0;
            }

            onItemChange(item, count[item]);
        }
    }

    public static int getItemCount(ItemType item)
    {
        if (count.ContainsKey(item))
        {
            return count[item];
        }

        return 0;
    }

    public static void onItemChange(ItemType item, int counted)
    {
        if (itemCountEvent != null)
        {
            itemCountEvent(item, counted);
        }
    }
}
