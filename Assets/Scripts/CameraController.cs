using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{

    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        GameObject root = GameObject.Find("root");
        // On right click: 
        if (Input.GetMouseButton(1))
        {
            // move rotate around parent (which is root of scene and also where the imported item is spawned)
            transform.RotateAround(root.transform.position, transform.up, -Input.GetAxis("Mouse X") * speed);
            transform.RotateAround(root.transform.position, transform.right, -Input.GetAxis("Mouse Y") * speed);
        }
        if (Input.mouseScrollDelta.y != 0)
        {
            Vector3 position = Vector3.MoveTowards(transform.position, root.transform.position, Input.mouseScrollDelta.y);
            if (position != root.transform.position)
            {
                transform.SetLocalPositionAndRotation(position, transform.localRotation);
            } // otherwise the camera can get stuck 
        }
    }
}
