using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GLTFast;

public class UIScript : MonoBehaviour
{

    public async void importGLB()
    {
        var gltf = new GLTFast.GltfImport();
        string path = EditorUtility.OpenFilePanel("Select a GLB to import", "", "glb");
        if (await gltf.LoadFile(path))
        {
            bool success = await gltf.InstantiateMainSceneAsync(new GameObjectInstantiator(gltf, GameObject.Find("root").transform));
            if (success)
            {
                return;
            }
        }
        Debug.Log("Failed to import GLB");

    }


}
