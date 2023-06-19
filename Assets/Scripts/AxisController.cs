using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
public class AxisController : MonoBehaviour
{

    public int axis; // 0 for X, 1 for Y, 2 for Z
    private Vector3 mousePos;
    public AxisMovedEvent axisMoved = new();
    
    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.parent.localPosition);
    }

    private void OnMouseDown()
    {
        mousePos = Input.mousePosition - GetMousePos();
    }

    private void OnMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePos);
        Vector3 pos2 = transform.parent.localPosition;
        /*
        Quaternion originalRotation = transform.parent.rotation;
        transform.parent.Rotate(0, 0, 0);

        switch (axis)
        {
            case 0:
                pos2 = transform.parent.right;
                break;
            case 1:
                pos2 = transform.parent.up;
                break;
            case 2:
                pos2 = transform.parent.forward;
                break;
            
        }
        Vector3 translation = new();
        translation[axis] = pos[axis]; 
        transform.parent.Translate(translation * Time.deltaTime);*/
        pos2[axis] = pos[axis];
        transform.parent.localPosition = pos2;
        axisMoved.Invoke(transform.parent, axis);
    }
}

public class AxisMovedEvent : UnityEvent<Transform, int> { }