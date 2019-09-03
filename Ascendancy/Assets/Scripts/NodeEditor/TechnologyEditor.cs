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

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name: ");
                tech.name = EditorGUILayout.TextField(selectedNode.tech.name);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Cost:  ");
                tech.cost = EditorGUILayout.IntField(selectedNode.tech.cost);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Icon Path: ");
                tech.icon = EditorGUILayout.ObjectField(selectedNode.tech.icon, typeof(Sprite), false) as Sprite;
                GUILayout.EndHorizontal();

                unitList.elements = new List<UnitInfo>(tech.unitsUnlocked);
                buildingList.elements = new List<BuildingInfo>(tech.buildingsUnlocked);
                resourceList.elements = new List<Resource>(tech.resourcesUnlocked);

                unitList.OnGUI();
                buildingList.OnGUI();
                resourceList.OnGUI();
                
                tech.unitsUnlocked = unitList.ElementsArray;
                tech.buildingsUnlocked = buildingList.ElementsArray;
                tech.resourcesUnlocked = resourceList.ElementsArray;
            }
        }
        
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
    
    void RefreshWindow()
    {
        techTreeEditor = TechTreeEditor.instance;
        //techTreeEditor = GetWindow<TechTreeEditor>();
        techTreeEditor.selectedNode.Subscribe(OnSelectedNodeChange);
    }
}
