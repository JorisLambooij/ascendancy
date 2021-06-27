using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationLine : Formation
{
    public Vector3 startPosition;
    public Vector3 endPosition;

    public FormationLine(Vector3 start, Vector3 end)
    {
        startPosition = start;
        endPosition = end;
    }

    public override Dictionary<Entity, Vector3> AssignPositions(List<Entity> units)
    {
        int count = units.Count;

        // the Dict that will be returned. Contains Units and their respective assignments
        assignedPositions = new Dictionary<Entity, Vector3>(count);
        assignedOrientations = new Dictionary<Entity, Vector3>(count);

        // direction of the line
        Vector3 dragLineDirection = (startPosition - endPosition);

        // orientation of the line (perpendicular to the line)
        Vector3 orientation = Vector3.Cross(dragLineDirection, Vector3.up).normalized;

        // list of the units, sorted along the line
        SortedDictionary<float, Entity> unitsSorted = new SortedDictionary<float, Entity>();

        // sort units, then issue commands according to units' relative position towards the goal line
        foreach (Entity u in units)
        {
            // Entity might have been destroyed, so check if it still exists
            if (u == null)
                continue;

            // Project the Unit's position onto the drag line
            Vector3 startToUnitPos = u.transform.position - startPosition;
            Vector3 projectedVector = Vector3.Project(startToUnitPos, dragLineDirection);
            float projectedDistance = projectedVector.magnitude;

            // if, by some chance, two units happen to have the same projected dictance, just move the second one slightly further down.
            while (unitsSorted.ContainsKey(projectedDistance))
                projectedDistance += 0.0001f;

            // sort by length of the projected vector
            unitsSorted.Add(projectedDistance, u);
        }

        // Make sure nothing has gone horribly wrong (no units missing or counted twice)
        // This will throw an error when a Unit is destroyed while it was selected.
        // TODO: Properly remove these destroyed Units from the list
        Debug.Assert(unitsSorted.Count == count, ".Count mismatch:" + "UnitsSorted.Count: " + unitsSorted.Count + "; SelectedUnits.Count: " + count);

        // assign a position to each unit
        int i = 0;
        foreach (KeyValuePair<float, Entity> kvp in unitsSorted)
        {
            // Determine the lerped position on the drag line
            float lerpFactor;
            if (count > 1)
                lerpFactor = (float)i / (count - 1);
            else
                lerpFactor = 0.5f;
            i++;
            Vector3 lerpedPos = Vector3.Lerp(startPosition, endPosition, lerpFactor);

            assignedPositions.Add(kvp.Value, lerpedPos);
            assignedOrientations.Add(kvp.Value, orientation);
        }

        return assignedPositions;
    }
}
