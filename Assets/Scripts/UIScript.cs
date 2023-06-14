using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GLTFast;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{

    public GameObject hierarchyItem;
    public GameObject threeDAxis;

    public TMP_InputField xAxis;
    public TMP_InputField yAxis;
    public TMP_InputField zAxis;

    public TMP_InputField xRot;
    public TMP_InputField yRot;
    public TMP_InputField zRot;

    public TMP_InputField xScale;
    public TMP_InputField yScale;
    public TMP_InputField zScale;

    public Vector3Input position;
    public Vector3Input rotation;
    public Vector3Input scale;

    private GameObject focusedObject = null;

    public void Start()
    {
        position.v3event.AddListener(PositionUpdate);
        rotation.v3event.AddListener(RotationUpdate);
        scale.v3event.AddListener(ScaleUpdate);
    }

    /// <summary>
    /// Imports a GLB file into the current scene by opening an "Open File" Windows Dialog
    /// </summary>
    public async void ImportGLB()
    {
        var gltf = new GLTFast.GltfImport();
        string path = EditorUtility.OpenFilePanel("Select a GLB to import", "", "glb");
        if (await gltf.LoadFile(path))
        {
            bool success = await gltf.InstantiateMainSceneAsync(new GameObjectInstantiator(gltf, GameObject.Find("root").transform));
            if (success)
            {
                var root = GameObject.Find("root");
                RecurseCreateChildren(root);
                return;
            }
        }
        Debug.Log("Failed to import GLB");

    }

    /// <summary>
    /// Creates a button in the left 'Hierarchy' Panel of the scene linked to the Gameobject passed in parameters
    /// </summary>
    /// <param name="child">The GameObject linked to the button</param>
    private void CreateChildInHierarchy(GameObject child)
    {
        GameObject uiButton = Instantiate(hierarchyItem, GameObject.Find("Hierarchy").transform);
        uiButton.GetComponent<Button>().onClick.AddListener(() => { OnHierarchyClick(child); });
        uiButton.GetComponentInChildren<TextMeshProUGUI>().SetText(child.name);
    }

    /// <summary>
    /// Recursively creates a button in the left panel for each children of the object given in parameters
    /// </summary>
    /// <param name="currentObject"></param>
    private void RecurseCreateChildren(GameObject currentObject)
    {
        for (int i = 0; i < currentObject.transform.childCount; ++i)
        {
            GameObject childObject = currentObject.transform.GetChild(i).gameObject;
            Debug.Log("creating child" + childObject.name);
            CreateChildInHierarchy(childObject);
            RecurseCreateChildren(childObject);
        }
    }

    /// <summary>
    /// Callback function when clicking a button in the left panel
    /// </summary>
    /// <param name="currentObject">The Object </param>
    public void OnHierarchyClick(GameObject currentObject)
    {
        threeDAxis.SetActive(true);
        Move3dAxis(currentObject.transform);
        focusedObject = currentObject;
        FillInfoBox();
    }

    /// <summary>
    /// Fills the right panel (infoBox) with the information of the object currently focused
    /// </summary>
    public void FillInfoBox()
    {
        Transform curT = focusedObject.transform;
        position.ResetValues(curT.localPosition);
        // I don't know yet of a better way to convert Quaternions to and from Vector 3s 
        rotation.ResetValues(new Vector3(curT.localRotation.x, curT.localRotation.y, curT.localRotation.z));
        scale.ResetValues(curT.localScale);

    }

    private void Move3dAxis(Transform dest)
    {
        threeDAxis.transform.position = dest.position;
        threeDAxis.transform.rotation = dest.rotation;
    }

    private void PositionUpdate(Vector3 position)
    {
        focusedObject.transform.localPosition = position;
        Move3dAxis(focusedObject.transform);
    }
    private void RotationUpdate(Vector3 rotation)
    {
        focusedObject.transform.localRotation = new Quaternion(rotation.x,rotation.y,rotation.z, 0);
        Move3dAxis(focusedObject.transform);

    }
    private void ScaleUpdate(Vector3 scale)
    {
        focusedObject.transform.localScale = scale;
        Move3dAxis(focusedObject.transform);
    }

}
    