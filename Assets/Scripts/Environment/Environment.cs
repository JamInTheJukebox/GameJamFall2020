using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public GameObject Platform;
    public Vector2 SpawnHere;

    public float X_OFFSET = 0.4061f;   // based on the dimensions of the block. This is how much distance is needed between one block and another        

    public void SpawnPlatforms()
    {
        if(Platform == null) { Debug.LogWarning("Environment.cs: WARNING. PLATFORM IS NULL"); return; }
        Instantiate(Platform, SpawnHere, Quaternion.identity);
    }
}
