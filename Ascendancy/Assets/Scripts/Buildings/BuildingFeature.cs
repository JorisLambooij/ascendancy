using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingFeature : ScriptableObject
{
    public abstract void Initialize(Building building);
    public abstract void UpdateOverride(Building building);
}
