using UnityEngine;
using GLTFast;
using TMPro;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{

    public GameObject hierarchyItem;
    public GameObject threeDAxis;

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
        
        // Get the file path using the "Open File Dialog" which is a Windows specific functionnality,
        // thus requires the "System Windows Form" DLL imported in the project files
        string path = "";
        using (System.Windows.Forms.OpenFileDialog ofd = new())
        {
            ofd.Filter = "GLB files (*.glb)| *.glb";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = ofd.FileName;
            }
            else
            {
                Debug.Log("Failed to open file");
                return;
            }
        }

        // Once the file path is acquired, use the GLTFast library to import the file.
        GLTFast.GltfImport gltf = new();
        if (await gltf.LoadFile(path))
        {
            bool success = await gltf.InstantiateMainSceneAsync(new GameObjectInstantiator(gltf, GameObject.Find("root").transform));
            if (success)
            {
                GameObject root = GameObject.Find("root");
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

    /// <summary>
    /// Moves the 3D Axis object to the destination transform given in paramaters
    /// </summary>
    /// <param name="dest">Transform to which the 3D axis must be moved</param>
    private void Move3dAxis(Transform dest)
    {
        threeDAxis.transform.position = dest.position;
        threeDAxis.transform.rotation = dest.rotation;
    }

    /// <summary>
    /// Updates the position of the focused game object info box with the vector 3 position given in parameter
    /// </summary>
    /// <param name="position"></param>
    private void PositionUpdate(Vector3 position)
    {
        focusedObject.transform.localPosition = position;
        Move3dAxis(focusedObject.transform);
    }
    private void RotationUpdate(Vector3 rotation)
    {
        focusedObject.transform.localRotation = new Quaternion(rotation.x, rotation.y, rotation.z, 0);
        Move3dAxis(focusedObject.transform);

    }
    private void ScaleUpdate(Vector3 scale)
    {
        focusedObject.transform.localScale = scale;
        Move3dAxis(focusedObject.transform);
    }

}
