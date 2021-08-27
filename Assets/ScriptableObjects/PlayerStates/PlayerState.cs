using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player State", menuName = "ScriptableObjects/PlayerState", order = 1)]
public class PlayerState : ScriptableObject
{
    [SerializeField] float MaxWalkSpeed = 2.5f;
    [SerializeField] float WalkAcceleration = 7;
    [SerializeField] float MaxRunSpeed = 5;
    [SerializeField] float RunAcceleration = 10;

    [SerializeField] float JumpImpulse = 4.95f;
    [SerializeField] float GravityScale = 0.8f;
    [SerializeField] float MaxFallSpeed = 10f;
    [Header("SpecialState")]
    [SerializeField] Vector2 SpecialSpeed;
    [SerializeField] float MaxYSpeed;
    public float GetMaxWalkSpeed()
    {
        return MaxWalkSpeed;
    }

    public float GetWalkAcceleration()
    {
        return WalkAcceleration;
    }
    public float GetMaxRunSpeed()
    {
        return MaxRunSpeed;
    }

    public float GetRunAcceleration()
    {
        return RunAcceleration;
    }

    public float GetJumpImpulse()
    {
        return JumpImpulse;
    }

    public float GetGravityScale()
    {
        return GravityScale;
    }
    
    public float GetMaxFallSpeed()
    {
        return MaxFallSpeed;
    }

    public Vector2 GetSpecialSpeed()
    {
        return SpecialSpeed;
    }
    public float GetMaxYSpeed()
    {
        return MaxYSpeed;
    }

}
