
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;
    public float parallaxEffect;

    [Header("Parallax Y")]
    private float height, startPos_Y;
    public Transform Max_Y;
    public Transform Min_Y;

    float MaximumY;
    float MinimumY;
    float Amplitude;
    void Start()
    {
        startPos = transform.position.x;
        startPos_Y = transform.position.y;
        var Temp = GetComponent<SpriteRenderer>();
        length = Temp.bounds.size.x;
        height = Temp.bounds.size.y;
        MaximumY = Max_Y.localPosition.y;
        MinimumY = Min_Y.localPosition.y;
        Amplitude = Max_Y.position.y + Min_Y.position.y;
        print(height);
    }

    void Update()
    {
        //if (gameObject.name.Contains("3")) { print(transform.localPosition.y); }
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distance = (cam.transform.position.x * parallaxEffect);
        float Distance_Y = (cam.transform.position.y * 0.4f);
        transform.position = new Vector3(startPos + distance, startPos_Y + Distance_Y, transform.position.z);
        //transform.position -= Vector3.up * 0.02f;
        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;

        if (transform.localPosition.y > MaximumY)
        {
            Vector2 A = transform.position;
            transform.position = new Vector2(transform.position.x, Min_Y.position.y);
            float Dist = Vector2.Distance(A, transform.position);

            startPos_Y -= Dist;
        }
        else if (transform.localPosition.y < MinimumY)
        {
            Vector2 A = transform.position;
            transform.position = new Vector2(transform.position.x, Max_Y.position.y);
            float Dist = Vector2.Distance(A, transform.position);

            startPos_Y += Dist;
        }
    }
}
