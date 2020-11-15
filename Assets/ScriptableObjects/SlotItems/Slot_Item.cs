using UnityEngine;

[CreateAssetMenu(fileName = "Slot Item", menuName = "ScriptableObjects/Slot_Item", order = 1)]
public class Slot_Item : ScriptableObject
{
    public string Item_Name;

    [Range(0, 100)] [Tooltip("The number of times this item will be observed when the slot machine spins 100 times")]
    public int Probability = 0;

    public Sprite Item_PNG;

    public string Effect;

    public Color Pixel_Color;

    public E_ObjectType ObjectType;
}