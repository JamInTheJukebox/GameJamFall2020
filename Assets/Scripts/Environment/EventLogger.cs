using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    ITEM_RETRIEVED,
    CHECKPOINT
}

public class EventDetails
{
    public EventType type;
    public GameObject obj;
    
    public EventDetails(EventType t, GameObject o)
    {
        type = t;
        obj = o;
    }
}

public static class EventLogger
{
    public delegate void checkpointEvent();
    public static event checkpointEvent triggeredCheckpoint;

    static Stack<EventDetails> log;

    static EventLogger()
    {
        log = new Stack<EventDetails>();
    }

    // undos all changes up until the first checkpoint encountered which will have its transform returned
    public static Transform undoChanges()
    {
        while (log.Count > 0)
        {
            EventDetails temp = log.Pop();

            switch (temp.type)
            {
                case (EventType.ITEM_RETRIEVED):
                    ItemPickUp tempItem = temp.obj.GetComponent<ItemPickUp>();
                    tempItem.reactivate();
                    ItemTracker.removeItem(tempItem.item);
                    break;
                case (EventType.CHECKPOINT):
                    if (triggeredCheckpoint != null)
                        triggeredCheckpoint();
                    return temp.obj.transform;
            }
        }

        return null;
    }

    public static void addLog(EventType type, GameObject obj)
    {
        log.Push(new EventDetails(type, obj));
    }

    public static void clearLog()
    {
        log.Clear();
    }
}
