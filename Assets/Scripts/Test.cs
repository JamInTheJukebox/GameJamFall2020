using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform TestPoint;

    private void Awake()
    {
        Vector3 point = TestPoint.position;
        print(GetComponent<BoxCollider2D>().bounds.Contains(point));
    }
}
