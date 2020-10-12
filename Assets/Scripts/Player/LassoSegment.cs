using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoSegment : MonoBehaviour
{
    // UNUSED
    // UNUSED
    // UNUSED
    // UNUSED

    public GameObject Segment_Above, Segment_Below;

    private void Start()
    {
        Segment_Above = GetComponent<HingeJoint2D>().connectedBody.gameObject;
        LassoSegment aboveSegment = Segment_Above.GetComponent<LassoSegment>();
        if (aboveSegment != null)
        {
            aboveSegment.Segment_Below = gameObject;
            float spriteBottom = Segment_Above.GetComponent<SpriteRenderer>().bounds.size.y;
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, spriteBottom * -1);
        }
        else
        {
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0); 
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            print("VOILA!!");
        }
    }
}
