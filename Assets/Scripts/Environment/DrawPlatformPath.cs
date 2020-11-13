using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPlatformPath : MonoBehaviour
{
    // simply draws a path for the player to know where the platform is going.
    public GameObject PathNode;
    [Range(2,20)] public int NodesToDraw;
    public Transform PointB;
    public Color NodeColor = Color.white;

    private void Awake()
    {
        if (PointB is Transform && NodesToDraw > 0)
        {
            Vector3 VecAtoB = PointB.position - transform.position;
            VecAtoB /= (NodesToDraw-1);
            for (int i = 0; i < NodesToDraw; i++)
            {

                var Game = Instantiate(PathNode, transform.position + i * VecAtoB, Quaternion.identity);
                if (NodeColor != Color.white)
                    Game.GetComponent<SpriteRenderer>().color = NodeColor;
            }

        }
    }



}
