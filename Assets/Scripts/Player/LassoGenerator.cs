using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoGenerator : MonoBehaviour
{

    // UNUSED
    // UNUSED
    // UNUSED

    public Rigidbody2D hook;
    public GameObject[] prefab_segments;
    public int num_links;


    void Awake()
    {
        GenerateRope();    
    }

    void GenerateRope()
    {
        Rigidbody2D prevBod = hook;
        for(int i = 0; i < num_links; i++)
        {
            int index = Random.Range(0, prefab_segments.Length);
            GameObject newSeg = Instantiate(prefab_segments[index]);
            newSeg.transform.parent = transform;
            newSeg.transform.position = transform.position;
            HingeJoint2D hj2 = newSeg.GetComponent<HingeJoint2D>();
            hj2.connectedBody = prevBod;
            prevBod = newSeg.GetComponent<Rigidbody2D>();
        }
    }
    void Update()
    {
        
    }
}
