using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityWheel : MonoBehaviour
{

    KeyItem m_currentItem;
    public KeyItem CurrentItem
    {
        get
        {
            return m_currentItem;
        }
        set
        {
            if(value != m_currentItem)
            {
                if(m_currentItem != null)
                    m_currentItem.DisableItem();
                m_currentItem = value;
                if(m_currentItem != null)
                    m_currentItem.EnableItem();
                // enable new item here.
            }
        }
    }

    e_keyItems m_currentIndex;
    public e_keyItems CurrentIndex
    {
        get { return m_currentIndex; }
        set
        { 
            if(value != m_currentIndex)
            {
                m_currentIndex = value;
                CurrentItem = AllKeyItems[m_currentIndex];
            }
        }
    }

    Dictionary<e_keyItems, KeyItem> AllKeyItems = new Dictionary<e_keyItems, KeyItem>();
    private void Awake()
    {
        AllKeyItems.Add(e_keyItems.Nothing, null);
        AllKeyItems.Add(e_keyItems.Lasso, GetComponentInChildren<ThrowLasso>());    
    }
    int debugCounter = 0;
    private void Update()
    {
        /*
        // change items here.
        if(Movement.PlayerInput.JumpTriggered())
        {
            debugCounter++;
            if(debugCounter >= AllKeyItems.Count) { debugCounter = 0; }
            else if(debugCounter < 0) { debugCounter = AllKeyItems.Count - 1; }

            e_keyItems it = (e_keyItems)debugCounter;
            CurrentIndex = it;
        }*/
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            KeyItem newItem = collision.GetComponent<KeyItem>();
            if(newItem != null)
            {
                switch (newItem.getTypeOfItem())
                {
                    case e_keyItems.Nothing:
                        break;
                    case e_keyItems.Lasso:
                        if(AllKeyItems[e_keyItems.Lasso])
                        {
                            AllKeyItems[e_keyItems.Lasso].EnableItem();
                            CurrentIndex = e_keyItems.Lasso;
                        }
                        break;
                }
            }
            Destroy(collision.gameObject);
        }
    }

    public bool UsingItem()
    {
        if(CurrentItem != null)
        {
            if (CurrentItem.UsingItem())
                return true;
        }

        return false;
    }
}
