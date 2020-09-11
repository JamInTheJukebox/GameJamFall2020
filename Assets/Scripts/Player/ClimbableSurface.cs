using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableSurface : MonoBehaviour
{
    [SerializeField] bool HasTop;
    [SerializeField] bool HorizontalClimbingAllowed;
    [SerializeField] Transform Climb_Down_Point;
    public bool Get_HasTop()
    {
        return HasTop;
    }
    public bool Get_HorizAllowed()
    {
        return HorizontalClimbingAllowed;
    }
    public Vector3 Get_Climb_Down_Position()
    {
        return Climb_Down_Point.position;
    }
}
