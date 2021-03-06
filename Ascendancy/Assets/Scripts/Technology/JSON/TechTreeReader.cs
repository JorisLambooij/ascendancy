﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TechTreeReader : MonoBehaviour
{
    public static string techPath = "Assets/_Data/Technology/tech_general.json";
    public static string nodePath = "Assets/_Data/Technology/tech_general_nodeData.json";

    private static TechTreeReader instance;
    public static TechTreeReader Instance
    {
        get
        {
            if (instance == null)
                instance = new TechTreeReader();
            return instance;
        }
    }
    
    public static TechnologyTree LoadTechTree()
    {
        TechnologyTree techTree = LoadTechData();
        LoadNodeData(techTree);
        return techTree;
    }

    private static TechnologyTree LoadTechData()
    {
        string dataAsJSON;
        if (!File.Exists(techPath))
            Debug.LogError("Technology data file not found at: " + techPath);
        
        dataAsJSON = File.ReadAllText(techPath);
        JSONTechTree loadedData = JsonUtility.FromJson<JSONTechTree>(dataAsJSON);

        // Add each of the Technologies to the TechTree
        TechnologyTree techTree = new TechnologyTree(loadedData.technologies.Length);
        foreach (JSON_Technology t in loadedData.technologies)
            techTree.AddTech(JSON_To_Tech_Converter.Convert(t));
        
        // Add each Technology to the leadsToTechs-list of its dependencies.
        foreach(Technology t in techTree.technologies)
            foreach(int dep in t.dependencies)
                techTree.techDictionary[dep].leadsToTechs.Add(t.id);

        return techTree;
    }

    private static void LoadNodeData(TechnologyTree techTree)
    {
        if (!File.Exists(nodePath))
            Debug.LogError("Technology node file not found at: " + nodePath);

        string dataAsJSON = File.ReadAllText(nodePath);
        NodeDataCollection loadedData = JsonUtility.FromJson<NodeDataCollection>(dataAsJSON);

        foreach (NodeData n in loadedData.nodeDataCollection)
            foreach (Technology t in techTree.technologies)
                if (t.id == n.id_Node)
                {
                    //Vector2 snapPos = n.position / loadedData.gridSnap;
                    //Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(snapPos.x), Mathf.RoundToInt(snapPos.x));
                    techTree.techPosition.Add(t.id, n.position);
                }
    }

}
