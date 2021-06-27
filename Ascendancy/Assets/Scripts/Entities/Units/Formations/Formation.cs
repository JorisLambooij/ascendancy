using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Formation
{
    public Dictionary<Entity, Vector3> assignedPositions = new Dictionary<Entity, Vector3>();
    public Dictionary<Entity, Vector3> assignedOrientations = new Dictionary<Entity, Vector3>();


    public abstract Dictionary<Entity, Vector3> AssignPositions(List<Entity> units);
}
