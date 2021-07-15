using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Formation
{
    public Dictionary<Entity, Vector3> assignedPositions = new Dictionary<Entity, Vector3>();
    public Dictionary<Entity, Vector3> assignedOrientations = new Dictionary<Entity, Vector3>();


    public abstract Dictionary<Entity, Vector3> AssignPositions(List<Entity> units);

    protected SortedDictionary<float, Entity> SortUnitsOnLine(List<Entity> units, Vector3 referencePoint, Vector3 direction)
    {
        // list of the units, sorted along the line
        SortedDictionary<float, Entity> unitsSorted = new SortedDictionary<float, Entity>();

        foreach (Entity u in units)
        {
            // Entity might have been destroyed, so check if it still exists
            if (u == null)
                continue;

            // Project the Unit's position onto the drag line
            Vector3 startToUnitPos = referencePoint - u.transform.position;
            Vector3 projectedVector = Vector3.Project(startToUnitPos, direction.normalized);
            float projectedDistance = Vector3.Dot(projectedVector, direction);

            // if, by chance, two units happen to have the same projected dictance, just move the second one slightly further down.
            while (unitsSorted.ContainsKey(projectedDistance))
            {
                projectedDistance += 0.0001f;
                Debug.Log("Double distance");
            }

            // sort by length of the projected vector
            unitsSorted.Add(projectedDistance, u);
        }

        // Make sure nothing has gone horribly wrong (no units missing or counted twice)
        // This will throw an error when a Unit is destroyed while it was selected.
        // TODO: Properly remove these destroyed Units from the list
        Debug.Assert(unitsSorted.Count == units.Count, ".Count mismatch:" + "UnitsSorted.Count: " + unitsSorted.Count + "; SelectedUnits.Count: " + units.Count);

        return unitsSorted;
    }
}
