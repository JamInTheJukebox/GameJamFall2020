using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableSurface : MonoBehaviour
{
    [SerializeField] bool HasTop;
    [SerializeField] bool HorizontalClimbingAllowed;

    public bool Get_HasTop()
    {
        return HasTop;
    }
    public bool Get_HorizAllowed()
    {
        return HorizontalClimbingAllowed;
    }
}
