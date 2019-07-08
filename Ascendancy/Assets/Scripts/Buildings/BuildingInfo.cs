using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Building SO", order = 2)]
public class BuildingInfo : ScriptableObject
{
    public string buildingName;

    public float goldCost;

    public List<BuildingFeature> features;
}
