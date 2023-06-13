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

    private GameObject threeDAxisInstance = null;

    private void Start()
    {
        threeDAxisInstance = Instantiate(threeDAxis);
        threeDAxisInstance.SetActive(false);
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
        threeDAxisInstance.SetActive(true);
        threeDAxisInstance.transform.position = currentObject.transform.position;
        threeDAxisInstance.transform.rotation = currentObject.transform.rotation;
        FillInfoBox(currentObject.transform);
    }

    /// <summary>
    /// Fills the right panel (infoBox) with the transform informations given in parameters
    /// </summary>
    /// <param name="currentObjectTransform">The transform of the object linked to the button that was clicked</param>
    private void FillInfoBox(Transform currentObjectTransform)
    {
        xAxis.text = currentObjectTransform.localPosition.x.ToString();
        yAxis.text = currentObjectTransform.localPosition.y.ToString();
        zAxis.text = currentObjectTransform.localPosition.z.ToString();
    }
}
    