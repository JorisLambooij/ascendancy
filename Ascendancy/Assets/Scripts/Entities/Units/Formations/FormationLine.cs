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
        Vector3 orientation = Vector3.Cross(Vector3.up, dragLineDirection).normalized;

        // list of the units, sorted along the line
        SortedDictionary<float, Entity> unitsSorted = SortUnitsOnLine(units, startPosition, dragLineDirection);


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
