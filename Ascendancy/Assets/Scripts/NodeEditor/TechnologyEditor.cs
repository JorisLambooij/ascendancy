using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TechnologyEditor : EditorWindow
{
    public string techSpriteFolder = "Assets/Resources/Sprites/Technologies/";
    
    private Node selectedNode;
    private JSON_Technology tech;
    private Sprite icon;

    private TechTreeEditor techTreeEditor;

    private GUIExpandableList<UnitInfo> unitList;
    private GUIExpandableList<BuildingInfo> buildingList;
    private GUIExpandableList<Resource> resourceList;

    [MenuItem("Window/Technology Editor")]
    private static void OpenWindow()
    {
        TechnologyEditor window = GetWindow<TechnologyEditor>();
        window.maxSize = new Vector2(200f, 512f);
        window.titleContent = new GUIContent("Technology Editor");
    }

    private void OnEnable()
    {
        RefreshWindow();
        unitList     = new GUIExpandableList<UnitInfo>("Units Unlocked", true, 0);
        buildingList = new GUIExpandableList<BuildingInfo>("Buildings Unlocked", true, 0);
        resourceList = new GUIExpandableList<Resource>("Resources Unlocked", true, 0);
    }

    void OnSelectedNodeChange(Node newNode)
    {
        selectedNode = newNode;
        if (selectedNode != null)
            tech = newNode.tech;
        else
            tech = null;
    }

    void OnGUI()
    {
        if (techTreeEditor == null)
        {
            GUILayout.Label("No Technology Tree Editor Window found!");
            if (GUILayout.Button("REFRESH"))
                RefreshWindow();
        }
        else
        {
            GUILayout.Label("Edit Technology:");
            if (selectedNode != null)
            {
                ///tech.name = EditorGUILayout.TextField(selectedNode.tech.name);
                
                tech.name = EditorGUILayout.TextField("Name", selectedNode.tech.name);
                
                tech.cost = EditorGUILayout.IntField("Cost", selectedNode.tech.cost);
                
                icon = AssetDatabase.LoadAssetAtPath<Sprite>(techSpriteFolder + selectedNode.tech.iconPath);
                if (icon == null)
                    Debug.LogError("Icon Missing:  " + AssetDatabase.GetAssetPath(icon));
                
                icon = EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false) as Sprite;

                if (icon != null)
                {
                    string relativePath = Path.GetFileName(AssetDatabase.GetAssetPath(icon));
                    tech.iconPath = relativePath;
                }
                
                unitList.elements     = ConvertFromStringArray<UnitInfo>(tech.unitsUnlocked);
                buildingList.elements = ConvertFromStringArray<BuildingInfo>(tech.buildingsUnlocked);
                resourceList.elements = ConvertFromStringArray<Resource>(tech.resourcesUnlocked);

                unitList.OnGUI();
                buildingList.OnGUI();
                resourceList.OnGUI();

                tech.unitsUnlocked     = ConvertToStringArray(unitList.elements);
                tech.buildingsUnlocked = ConvertToStringArray(buildingList.elements);
                tech.resourcesUnlocked = ConvertToStringArray(resourceList.elements);
            }
        }
        
    }

    private List<T> ConvertFromStringArray<T>(string[] oldArray) where T : Object
    {
        List<T> newList = new List<T>(oldArray.Length);

        foreach (string s in oldArray)
        {
            string path = "Assets/Resources/" + s;
            T element = AssetDatabase.LoadAssetAtPath<T>(path);
            if (element != null)
                newList.Add(element);
        }

        return newList;
    }

    private string[] ConvertToStringArray<T>(List<T> oldList) where T : Object
    {
        string[] newArray = new string[oldList.Count];

        for (int i = 0; i < oldList.Count; i++)
        {
            T t = oldList[i];
            string s = AssetDatabase.GetAssetPath(t);
            int index = "Assets/Resources/".Length;
            if (s.Length > index)
                s = s.Substring(index);
            else
            {
                Debug.LogError("Something wrong with " + s);
                s = "";
            }
            newArray[i] = s;
        }

        return newArray;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
    
    void RefreshWindow()
    {
        techTreeEditor = TechTreeEditor.instance;
        techTreeEditor.selectedNode.Subscribe(OnSelectedNodeChange);
    }
}
