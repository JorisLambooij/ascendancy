using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
public class EntityInfoEditor : EditorWindow
{
    private SerializedObject[] loadedEIs;
    private string[] fileNames;
    private EntityInfo selectedEntityInfo;
    private string infoPath = "ScriptableObjects/";
    private int selectedIndex;
    private Vector2 scrollPos;

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
        Object[] entityInfoClasses = Resources.LoadAll(infoPath, typeof(EntityInfo));
        loadedEIs = new SerializedObject[entityInfoClasses.Length];
        fileNames = new string[entityInfoClasses.Length];

        for (int i = 0; i < entityInfoClasses.Length; i++)
        {
            loadedEIs[i] = new SerializedObject(entityInfoClasses[i]);
            fileNames[i] = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(entityInfoClasses[i]));
        }
        Debug.Log("Loaded " + loadedEIs.Length + " Entities");

        RefreshWindow();
    }
    
    void OnGUI()
    {
        //string name = selectedEntityInfo != null ? selectedEntityInfo.name : "-";        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New", GUILayout.Width(60)))
        {
            Debug.Log("Newing");
            EntityInfo info = (EntityInfo)CreateInstance("EntityInfo");
            info.name = "test";
            selectedEntityInfo = info;

        }
        if(GUILayout.Button("Load", GUILayout.Width(60)))
        {
            Debug.Log("Loading");
            LoadEntityInfo();
        }
        if (GUILayout.Button("Save", GUILayout.Width(60)))
        {
            Debug.Log("Saving");
            //SaveEntityInfo(info);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(70));
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));    //this one is bullying me! -.-

        for (int i = 0; i < loadedEIs.Length; i++)
        {
            GUI.backgroundColor = (selectedIndex == i) ? Color.blue : Color.white;


            EditorGUILayout.PropertyField(loadedEIs[i].FindProperty("name"));
            

            Rect lastRect = GUILayoutUtility.GetLastRect();
            //if (GUI.Button(lastRect, loadedEIs[i].FindProperty("name").stringValue))  this uses "name" property
            if (GUI.Button(lastRect, fileNames[i]))
            {
                selectedIndex = i;
            }
        }

        GUILayout.EndVertical();

        EditorGUIUtility.ExitGUI();
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

    private void SaveEntityInfo(EntityInfo info)
    {
        //EntityInfo asset = CreateInstance<EntityInfo>();

        if (info == null)
        {
            Debug.LogError("Could not create new Entity!");
            return;
        }


        //this line is throws an exception because it crashes the gui stack. Exit gui first if you need it.
        string path = EditorUtility.SaveFilePanel("Save Entity", infoPath, selectedEntityInfo.name, "asset");
        
        int index = path.IndexOf(infoPath);
        path = path.Substring(index);

        AssetDatabase.CreateAsset(info, path);
        AssetDatabase.SaveAssets();

        //EditorUtility.FocusProjectWindow();

        //Selection.activeObject = asset;
    }
}
#endif
