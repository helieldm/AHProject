using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Vector3Input : MonoBehaviour
{
    public TMP_InputField xInput;
    public TMP_InputField yInput;
    public TMP_InputField zInput;

    public Vector3 inputVector = Vector3.zero;

    public Vector3ChangedEvent v3event = new();

    public void updateX(string xVal)
    {
        inputVector.x = float.Parse(xVal);
        v3event.Invoke(inputVector);
    }

    public void updateY(string yVal)
    {
        inputVector.y = float.Parse(yVal);
        v3event.Invoke(inputVector);

    }

    public void updateZ(string zVal)
    {
        inputVector.z = float.Parse(zVal);
        v3event.Invoke(inputVector);

    }

    public void ResetValues(Vector3 v3)
    {
        xInput.text = v3.x.ToString();
        yInput.text = v3.y.ToString();
        zInput.text = v3.z.ToString();
    }

}
