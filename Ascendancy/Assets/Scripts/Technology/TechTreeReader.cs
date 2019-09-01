using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TechTreeReader : MonoBehaviour
{
    public static string techPath = "Assets/Technology/tech_general.json";
    public static string nodePath = "Assets/Technology/tech_general_nodeData.json";

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
        string dataAsJSON;
        if (!File.Exists(techPath))
            Debug.LogError("Technogoly data file not found at: " + techPath);
        
        dataAsJSON = File.ReadAllText(techPath);
        JSONTechTree loadedData = JsonUtility.FromJson<JSONTechTree>(dataAsJSON);

        TechnologyTree techTree = new TechnologyTree(loadedData.technologies.Length);
        foreach (Technology t in loadedData.technologies)
            techTree.AddTech(t);
        
        return techTree;
    }

}
