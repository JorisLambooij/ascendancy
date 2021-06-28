using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR) 
public class EntityInfoEditor : EditorWindow
{
    EntityInfo entityInfo;
    public string infoPath = "Assets/Resources/ScriptableObjects/";

    [MenuItem("Window/Entity Editor")]
    private static void OpenWindow()
    {
        EntityInfoEditor window = GetWindow<EntityInfoEditor>();
        window.minSize = new Vector2(180f, 256f);
        window.maxSize = new Vector2(200f, 512f);
        window.titleContent = new GUIContent("Entity Editor");
    }

    private void OnEnable()
    {
        RefreshWindow();
    }
    
    void OnGUI()
    {
        string name = entityInfo != null ? entityInfo.name : "-";
        GUILayout.Label("Edit Entity: " + name);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New", GUILayout.Width(60)))
        {
            Debug.Log("Newing");
            EntityInfo info = new EntityInfo();
            info.name = "test";
            CreateNewEntityInfo(info);

            entityInfo = info;

        }
        if(GUILayout.Button("Load", GUILayout.Width(60)))
        {
            Debug.Log("Loading");
            LoadEntityInfo();
        }
        if (GUILayout.Button("Save", GUILayout.Width(60)))
        {
            Debug.Log("Saving");
        }
        GUILayout.EndHorizontal();

    }

    void RefreshWindow()
    {
        
    }

    private EntityInfo LoadEntityInfo()
    {
        string path = EditorUtility.OpenFilePanel("Load Entity Info", infoPath, "asset");
        int index = path.IndexOf(infoPath);
        path = path.Substring(index);
        EntityInfo info = (EntityInfo)AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

        if (info == null)
        {
            Debug.LogError("Not an EntityInfo");
            return null;
        }
        Debug.Log("Loaded " + info.name);
        return null;
    }

    private void CreateNewEntityInfo(EntityInfo info)
    {
        //EntityInfo asset = CreateInstance<EntityInfo>();

        string path = EditorUtility.SaveFilePanel("Save Entity", infoPath, entityInfo.name, "asset");
        int index = path.IndexOf(infoPath);
        path = path.Substring(index);

        AssetDatabase.CreateAsset(info, path);
        AssetDatabase.SaveAssets();

        //EditorUtility.FocusProjectWindow();

        //Selection.activeObject = asset;
    }
}
#endif
