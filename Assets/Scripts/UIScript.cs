using UnityEngine;
using GLTFast;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class UIScript : MonoBehaviour
{


    public GameObject hierarchyItem;
    public GameObject threeDAxis;
    public GameObject root;
    public GameObject hierarchy;

    public Vector3Input position;
    public Vector3Input rotation;
    public Vector3Input scale;

    public AxisController xAxis;
    public AxisController yAxis;
    public AxisController zAxis;

    public TMP_Text infoBoxTitle;

    private GameObject focusedObject = null;

    /// <summary>
    /// On Start, add all relevant listeners for the infobox fields as well as the 3D axis
    /// </summary>
    public void Start()
    {
        position.v3event.AddListener(PositionUpdate);
        rotation.v3event.AddListener(RotationUpdate);
        scale.v3event.AddListener(ScaleUpdate);

        xAxis.axisMoved.AddListener(FollowAxisEvent);
        yAxis.axisMoved.AddListener(FollowAxisEvent);
        zAxis.axisMoved.AddListener(FollowAxisEvent);
    }

    /// <summary>
    /// Quit the application
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
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
            bool success = await gltf.InstantiateMainSceneAsync(new GameObjectInstantiator(gltf, root.transform));
            if (success)
            {
                //clear hierarchy and rebuild it
                ClearHierarchy();
                RecurseCreateChildren(root);
                return;
            }
        }
        Debug.Log("Failed to import GLB");

    }

    private void ClearHierarchy()
    {
        for (int i = 0; i < hierarchy.transform.childCount; i++)
        {
            Destroy(hierarchy.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Recursively creates a button in the left panel for each children of the object given in parameters
    /// </summary>
    /// <param name="currentObject">The object linked to the button</param>
    /// <param name="depth">The depth of the recursion</param>
    private void RecurseCreateChildren(GameObject currentObject, int depth = 0)
    {
        for (int i = 0; i < currentObject.transform.childCount; ++i)
        {
            GameObject childObject = currentObject.transform.GetChild(i).gameObject;
            CreateChildInHierarchy(childObject, depth);
            RecurseCreateChildren(childObject, depth + 1);
        }
        
    }

    /// <summary>
    /// Creates a button in the left 'Hierarchy' Panel of the scene linked to the Gameobject passed in parameters
    /// </summary>
    /// <param name="child">The GameObject linked to the button</param>
    /// <param name="depth">The depth of the recursion</param>
    private void CreateChildInHierarchy(GameObject child, int depth = 0)
    {
        GameObject uiButton = Instantiate(hierarchyItem, hierarchy.transform);
        uiButton.GetComponent<Button>().onClick.AddListener(() => { OnHierarchyClick(child); });
        uiButton.GetComponentInChildren<TextMeshProUGUI>().SetText(string.Concat(Enumerable.Repeat("  ", depth)) + child.name);
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
        rotation.ResetValues(curT.localEulerAngles);
        scale.ResetValues(curT.localScale);
        infoBoxTitle.text = focusedObject.name;

    }

    /// <summary>
    /// Moves the 3D Axis object to the destination transform given in paramaters
    /// </summary>
    /// <param name="dest">Transform to which the 3D axis must be moved</param>
    private void Move3dAxis(Transform dest)
    {
        threeDAxis.transform.SetPositionAndRotation(dest.position, dest.rotation);
    }

    /// <summary>
    /// Updates the position of the focused game object info box with the vector 3 position given in parameter
    /// </summary>
    /// <param name="position">New position of the focused game object</param>
    private void PositionUpdate(Vector3 position)
    {
        focusedObject.transform.localPosition = position;
        Move3dAxis(focusedObject.transform);
    }
    private void RotationUpdate(Vector3 rotation)
    {
        focusedObject.transform.localRotation = Quaternion.Euler(rotation);
        Move3dAxis(focusedObject.transform);

    }
    private void ScaleUpdate(Vector3 scale)
    {
        focusedObject.transform.localScale = scale;
        Move3dAxis(focusedObject.transform);
    }

    public void FollowAxisEvent(Transform t, int axis)
    {
        position.ResetValues(t.localPosition);
        rotation.ResetValues(t.localRotation.eulerAngles);
        Vector3 newPos = focusedObject.transform.localPosition;
        newPos[axis] = t.localPosition[axis];
        focusedObject.transform.localPosition = newPos;
    }

}
