using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        // On right click: 
        if (Input.GetMouseButton(1))
        {
            GameObject root = GameObject.Find("root");
            // move rotate around parent (which is root of scene and also where the imported item is spawned)
            transform.RotateAround(root.transform.position, transform.up, -Input.GetAxis("Mouse X") * speed);
            transform.RotateAround(root.transform.position, transform.right, -Input.GetAxis("Mouse Y") * speed);
        }
    }
}
