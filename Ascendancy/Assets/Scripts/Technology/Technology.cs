using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Sprite icon;

    /// <summary>
    /// The associated cost of this Technology.
    /// </summary>
    public int cost;

    /// <summary>
    /// Is this tech unlocked at the start of the game.
    /// </summary>
    public bool startTech;

    /// <summary>
    /// The IDs of the technologies this tech depends on.
    /// </summary>
    public int[] dependencies;

    /// <summary>
    /// The IDs of the technologies that this tech unlocks.
    /// </summary>
    public List<int> leadsToTechs;

    public Technology(string name, int id, Sprite icon, int cost, bool startTech, int[] dependencies, EntityInfo[] unitsUnlocked, BuildingInfo[] buildingsUnlocked, Resource[] resourcesUnlocked)
    {
        this.name = name;
        this.id = id;
        this.icon = icon;
        this.cost = cost;
        this.startTech = startTech;
        this.dependencies = dependencies;
        this.unitsUnlocked = unitsUnlocked;
        this.buildingsUnlocked = buildingsUnlocked;
        this.resourcesUnlocked = resourcesUnlocked;
        this.leadsToTechs = new List<int>();
    }

    public Technology(JSON_Technology tech, Sprite icon)
    {
        name = tech.name;
        id = tech.id;
        cost = tech.cost;
        startTech = tech.startTech;
        dependencies = tech.dependencies;
        this.icon = icon;
        this.leadsToTechs = new List<int>();
    }

    // Effects
    public EntityInfo[] unitsUnlocked;
    public BuildingInfo[] buildingsUnlocked;
    public Resource[] resourcesUnlocked;

    // maybe also unit/building buffs (+Unit dmg, +Production etc.)
}