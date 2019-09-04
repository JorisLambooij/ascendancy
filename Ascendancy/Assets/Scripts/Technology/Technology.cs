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
    /// The icon path for this Technology.
    /// </summary>
    public string iconPath;

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
    
    public Technology(string name, int id, int[] dependencies, int cost, bool startTech, string iconPath)
    {
        this.name = name;
        this.id = id;
        this.dependencies = dependencies;
        this.cost = cost;
        this.startTech = startTech;
        this.iconPath = iconPath;
    }

    public TechnologySO techSO;

    // Effects
    public UnitInfo[] unitsUnlocked;
    public BuildingInfo[] buildingsUnlocked;
    public Resource[] resourcesUnlocked;

    // maybe also unit/building buffs (+Unit dmg, +Production etc.)
}
