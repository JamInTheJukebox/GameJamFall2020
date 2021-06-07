
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;
    public float parallaxEffect;

    float MaximumY;
    float MinimumY;
    float Amplitude;
    void Start()
    {
        startPos = transform.position.x;
        var Temp = GetComponent<SpriteRenderer>();
        length = Temp.bounds.size.x;
    }

    void LateUpdate()
    {
        //if (gameObject.name.Contains("3")) { print(transform.localPosition.y); }
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distance = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
        //transform.position -= Vector3.up * 0.02f;
        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;
    }
}
