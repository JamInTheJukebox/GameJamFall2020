using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using minigameEnumerators;
using TMPro;

public class PlayerPanel : MonoBehaviour
{
    // Player background and color.
    [SerializeField] GameObject ObjectiveItemUI;
    [SerializeField] TextMeshProUGUI ObjectiveItemText;
    private int m_ObjectiveItemCount;
    public int ObjectiveItemCount
    {
        get { return m_ObjectiveItemCount; }
        set
        {
            if(value != m_ObjectiveItemCount)
            {
                m_ObjectiveItemCount = value;
                ObjectiveItemText.text = "x " + m_ObjectiveItemCount.ToString();
            }
        }
    }
    public void SetItemUI(bool active)
    {
        ObjectiveItemUI.SetActive(active);
    }

    public void SetObjectiveItemInfo(ObjectiveItem _itemInfo)
    {
        if (_itemInfo == ObjectiveItem.off)
            ObjectiveItemUI.SetActive(false);
        // set PNG here.
    }
}
