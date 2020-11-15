using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ObjectType
{
    Blue, Red, Green, Yellow,       // blocks
    BlockSpikes,                         // block spikes, maybe spikes individually if I have time.
    Laser, StationaryLazer,          // Two Types of Lazers
    LatchesRed, LatchesYellow, LatchesGreen,    // three types of latches.
    PanelBrown, PanelManual, PanelAuto,         // brown, green, and purple panel.
    Ladder, HorizontalLadder,               // Ladder, and blue ladder
    Bouncy, SlowPlatform,                             // bouncy platform
    TNT, Rocket
}

public class Object_Toggler : MonoBehaviour
{

    [SerializeField] E_ObjectType ObjectType;

    private void Awake()
    {
        EventLogger.ItemToggle += ToggleObject;
    }

    public void ToggleObject(E_ObjectType TargetType)
    {
        if(TargetType == ObjectType)
        {
            gameObject.SetActive(false);        // set Target gameobjects to false.
        }
        else if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);     // set objects back to enabled when they are not the target.
        }
    }

}
