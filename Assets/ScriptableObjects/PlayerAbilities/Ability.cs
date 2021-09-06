using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Ability", menuName = "ScriptableObjects/PlayerAbility", order = 1)]
public class Ability : ScriptableObject
{
    public Sprite AbilityPNG;
    public string Name;
    public float Cooldown;
}
