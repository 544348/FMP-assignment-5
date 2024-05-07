using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //variables
    private float length, startpos;
    private float height, startposY;
    public GameObject cam;
    public float parallaxEffect;
    void Start()
    {
        startpos = transform.position.x;
        startposY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;  
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void FixedUpdate()
    {
        float dist = (cam.transform.position.x * parallaxEffect);
        float distY = (cam.transform.position.y * parallaxEffect);
        transform.position = new Vector3(startpos + dist, startposY + distY, transform.position.z);
    }
}
