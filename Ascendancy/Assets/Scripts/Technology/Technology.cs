using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewTechnology", menuName = "Technology")]
[System.Serializable]
public class Technology
{
    /// <summary>
    /// The Name of this Technology.
    /// </summary>
    public string name;

    /// <summary>
    /// The unique identifier for this technology.
    /// </summary>
    public int id;

    /// <summary>
    /// The icon for this Technology.
    /// </summary>
    public string icon;

    /// <summary>
    /// List of the IDs of the requirements for this Technology.
    /// </summary>
    public int[] dependencies;

    /// <summary>
    /// How expensive this Technology is.
    /// </summary>
    public int cost;

    /// <summary>
    /// Whether or not this Technology has been researched at the start of the game;
    /// </summary>
    public bool startTech;
    
    public Technology(string name, int id, int[] dependencies, int cost, bool startTech, string icon)
    {
        this.name = name;
        this.id = id;
        this.dependencies = dependencies;
        this.cost = cost;
        this.startTech = startTech;
        this.icon = icon;
    }

    // Effects
    //public List<UnitInfo> unitsUnlocked;
    //public List<BuildingInfo> buildingsUnlocked;


}
