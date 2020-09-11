using System.Collections;
using System.Collections.Generic;
using System;

public enum ItemType
{
    COIN
}

public static class ItemCount
{
    static Dictionary<ItemType, int> count;

    static ItemCount()
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
}
