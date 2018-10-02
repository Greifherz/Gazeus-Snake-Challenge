using UnityEngine;
using System.Collections;
using UnityEditor;

public class ScriptableObjectUtility
{
    [MenuItem("Assets/Create/ScriptableObject from Selection F5")]
    public static void CreateAssetFromSelectionMenu()
    {
        Create();
    }

    public static void Create()
    {
        try
        {
            MonoScript SelectedScript = (MonoScript)Selection.activeObject;

            string Path = AssetDatabase.GetAssetPath(SelectedScript.GetInstanceID());
            Path = Path.Substring(0, Path.Length - 3) + ".asset";

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(SelectedScript.GetClass().Name), Path);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Debug.Log(SelectedScript.name + ".asset created Successfully");
        }
        catch
        {
            Debug.LogError("Selected object on Editor is not a script or ScriptableObject");
        }
    }
}