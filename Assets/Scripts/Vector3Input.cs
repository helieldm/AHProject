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

    public void UpdateX(string xVal)
    {
        try
        {
            inputVector.x = float.Parse(xVal);
        }
        catch (FormatException e) // input is invalid
        {
            Debug.Log(e);
            inputVector.x = 0;
        }
        v3event.Invoke(inputVector);
    }

    public void UpdateY(string yVal)
    {
        try
        {
            inputVector.y = float.Parse(yVal);
        }
        catch (FormatException e) // input is invalid
        {
            Debug.Log(e);
            inputVector.y = 0;
        }
        v3event.Invoke(inputVector);

    }

    public void UpdateZ(string zVal)
    {
        try
        {
            inputVector.z = float.Parse(zVal);
        }
        catch (FormatException e) // input is invalid
        {
            Debug.Log(e);
            inputVector.z = 0;
        }
        v3event.Invoke(inputVector);

    }

    public void ResetValues(Vector3 v3)
    {
        xInput.text = v3.x.ToString();
        yInput.text = v3.y.ToString();
        zInput.text = v3.z.ToString();
    }

}
