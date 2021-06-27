using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSquare : Formation
{
    public Vector3 center;
    public Vector3 orientation;

    public float size;

    private Vector3 dragLineDirection;

    public FormationSquare(Vector3 startDrag, Vector3 endDrag)
    {
        Vector3 lineV = startDrag - endDrag;
        this.center = (startDrag + endDrag) * 0.5f;
        this.orientation = Vector3.Cross(Vector3.up, lineV.normalized);
        this.size = lineV.magnitude / 2;

        dragLineDirection = lineV.normalized;
    }

    public override Dictionary<Entity, Vector3> AssignPositions(List<Entity> units)
    {
        int count = units.Count;

        // the Dict that will be returned. Contains Units and their respective assignments
        assignedPositions = new Dictionary<Entity, Vector3>(count);
        assignedOrientations = new Dictionary<Entity, Vector3>(count);

        // list of the units, sorted laterally along the square's orientation axis
        SortedDictionary<float, Entity> unitsSorted = SortUnitsOnLine(units, center, orientation);

        int squareSize = Mathf.Max(3, Mathf.CeilToInt(Mathf.Sqrt(count)));

        List<List<Entity>> unitRows = new List<List<Entity>>(units.Count);

        int i = 0, j = 0;
        foreach (KeyValuePair<float, Entity> kvp in unitsSorted)
        {
            if (j == 0)
                unitRows.Add(new List<Entity>(squareSize));

            unitRows[i].Add(kvp.Value);

            j++;
            if (j == squareSize)
            {
                i++;
                j = 0;
            }    
        }

        int y = 0;
        foreach (List<Entity> row in unitRows)
        {
            SortedDictionary<float, Entity> rowSorted = SortUnitsOnLine(row, center, dragLineDirection);

            Vector3 startPosition = center - size * orientation * y - size * dragLineDirection;
            Vector3 endPosition = center - size * orientation * y + size * dragLineDirection;
            y++;
            int x = 0;
            foreach (Entity e in row)
            {
                // Determine the lerped position within the row
                float lerpFactor;
                if (row.Count > 1)
                    lerpFactor = (float)x / (row.Count - 1);
                else
                    lerpFactor = 0.5f;

                x++;

                Vector3 lerpedPos = Vector3.Lerp(startPosition, endPosition, lerpFactor);

                assignedPositions.Add(e, lerpedPos);
                assignedOrientations.Add(e, orientation);
            }
        }

        return assignedPositions;
    }
}
