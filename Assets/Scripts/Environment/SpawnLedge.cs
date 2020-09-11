using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLedge : MonoBehaviour
{
    public bool RightLedge;
    public GameObject Ledge;
    public E_PlatformType PlatformType = E_PlatformType.small;

    public float X_Offset = 0;
    public float Y_Offset = 0;

    public enum E_PlatformType
    {
        small = 0,
        medium = 1,
        big = 2
    }
    private void Awake()
    {
        float BoxHeight = gameObject.GetComponent<Renderer>().bounds.size.y / 2;
        float BoxWidth = gameObject.GetComponent<Renderer>().bounds.size.x / 2;

        BoxWidth *= (RightLedge) ? 1 : -1;
        BoxHeight *= (int)PlatformType;
        Vector3 LadderTopPos = transform.position + new Vector3(BoxWidth + X_Offset, BoxHeight + Y_Offset, 0);
        
        GameObject.Instantiate(Ledge, LadderTopPos, Quaternion.identity).transform.parent = transform;
    }
}
