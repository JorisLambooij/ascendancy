using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TechnologyEditor : EditorWindow
{
    private Node selectedNode;
    private Technology tech;

    private TechTreeEditor techTreeEditor;

    private GUIExpandableList<UnitInfo> unitList;
    private GUIExpandableList<BuildingInfo> buildingList;
    private GUIExpandableList<Resource> resourceList;

    [MenuItem("Window/Technology Editor")]
    private static void OpenWindow()
    {
        TechnologyEditor window = GetWindow<TechnologyEditor>();
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
        tech = newNode.tech;
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
            if (GUILayout.Button("Save"))
                SaveData();

            GUILayout.Label("Edit Technology:");
            if (selectedNode != null)
            {
                ///tech.name = EditorGUILayout.TextField(selectedNode.tech.name);

                tech.name = EditorGUILayout.TextField(selectedNode.tech.name);
                tech.cost = EditorGUILayout.IntField(selectedNode.tech.cost);

                unitList.elements = new List<UnitInfo>(tech.unitsUnlocked);
                
                unitList.OnGUI();
                buildingList.OnGUI();
                resourceList.OnGUI();
                
                tech.unitsUnlocked = unitList.ElementsArray;
            }
        }
        
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void SaveData()
    {
        //Technology tech = new Technology()
        //selectedNode.tech
    }

    void RefreshWindow()
    {
        techTreeEditor = TechTreeEditor.instance;
        //techTreeEditor = GetWindow<TechTreeEditor>();
        techTreeEditor.selectedNode.Subscribe(OnSelectedNodeChange);
    }
}
