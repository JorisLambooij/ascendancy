using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Building SO", order = 2)]
public class BuildingInfo : ScriptableObject
{
    public string buildingName;

    public float goldCost;

    public int maxHealth;

    public List<BuildingFeature> features;

    /// <summary>
    /// How many options the context menu has for this building.
    /// </summary>
    public int contextMenuOptions;
}
