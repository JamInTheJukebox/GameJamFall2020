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
    public BoxCollider2D Occulsion_effect;
    List<Transform> Blocks_Del = new List<Transform>();
    [SerializeField] E_ObjectType ObjectType;

    private void Awake()
    {
        EventLogger.ItemToggle += ToggleObject;
    }

    public void ToggleObject(E_ObjectType TargetType)
    {
        if(TargetType == ObjectType)
        {
            if ((int)ObjectType <= 3)
            {
                StartCoroutine(ToggleBlocks(false));
                return;
            }

            gameObject.SetActive(false);        // set Target gameobjects to false.
        }
        else if (!gameObject.activeSelf | (int)ObjectType <= 3)
        {
            if ((int)ObjectType <= 3 && Blocks_Del.Count > 0)
            {
                StartCoroutine(ToggleBlocks(true));
                return;
            }
            gameObject.SetActive(true);     // set objects back to enabled when they are not the target.

        }
    }

    IEnumerator ToggleBlocks(bool status)
    {
        if(status == false) { yield return new WaitForSeconds(2.5f); }
        // toggling off
        if (!status)
        {
            foreach (Transform Child in transform)
            {
                int DistanceCheck = 0;                                                                                                              // to avoid iterating through blocks that are literally too far away
                bool DeletedBlock = false;
                if (Occulsion_effect == null) { Debug.LogWarning("Object_Toggler.cs: WARNING. Occulsion effect not assigned."); yield return null; }

                foreach (Transform block in Child.transform)
                {
                    var Block = block.gameObject;
                    if (Block.activeSelf == status) { continue; }        // if the block is not going to change, do not bother doing anything. Useful for cases where we get the same role again.
                    if (Occulsion_effect.bounds.Contains(block.position))
                    {
                        Block.SetActive(false);
                        if (!DeletedBlock) { DeletedBlock = true; if (!Blocks_Del.Contains(Child)) { Blocks_Del.Add(Child); } }        // keep a reference to any blocks that have stuff deleted so we don't have to iterate through the whole list again.
                    }
                    else
                    {
                        // turn on blocks that are too far away here.
                        var Dist = (Occulsion_effect.transform.position - block.position).sqrMagnitude;
                        if(Dist > 450) { break; }                                                                      // extreme cases 
                        if (Dist > 350) { DistanceCheck++; }
                        if (DistanceCheck >= 3) { break; }                                                                    // if there are more than 3 blocks very far away from the player, break from the block group.
                    }

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        // when toggling on
        else
        {

            for(int i = 0; i < Blocks_Del.Count; i++)
            {
                foreach (Transform block in Blocks_Del[i])
                {
                    var Block = block.gameObject;
                    if (Block.activeSelf == status) { continue; }        // if the block is not going to change, do not bother doing anything.
                    Block.SetActive(true);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            Blocks_Del.Clear();
            //gameObject.SetActive(status);     // set objects back to enabled when they are not the target.
        }

    }

    private void OnDestroy()
    {
        EventLogger.ItemToggle -= ToggleObject;
    }
}
