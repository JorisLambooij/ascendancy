using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSquare : Formation
{
    public Vector3 center;
    public Vector3 orientation;

    public float size;
    private float entitySpacing;

    private Vector3 dragLineDirection;

    public FormationSquare(Vector3 startDrag, Vector3 endDrag, float entitySpacing)
    {
        Vector3 lineV = startDrag - endDrag;
        this.center = (startDrag + endDrag) * 0.5f;
        this.orientation = Vector3.Cross(Vector3.up, lineV.normalized);
        this.size = lineV.magnitude;
        this.entitySpacing = entitySpacing;

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

        int rowLength = Mathf.FloorToInt(size / entitySpacing); //Mathf.Max(3, Mathf.CeilToInt(Mathf.Sqrt(count)));
        int rowCount = Mathf.Max(1, Mathf.CeilToInt((float)count / rowLength));

        List<List<Entity>> unitRows = new List<List<Entity>>(units.Count);

        // create 'squareSize'amount of 
        int i = 0, j = 0;
        foreach (KeyValuePair<float, Entity> kvp in unitsSorted)
        {
            if (j == 0)
                unitRows.Add(new List<Entity>(rowCount));

            unitRows[i].Add(kvp.Value);

            j++;
            if (j == rowLength)
            {
                i++;
                j = 0;
            }    
        }

        int y = 0;
        foreach (List<Entity> row in unitRows)
        {
            SortedDictionary<float, Entity> rowSorted = SortUnitsOnLine(row, center, dragLineDirection);

            Vector3 startPosition = center - 1.5f * entitySpacing * orientation * y + size * dragLineDirection / 2;
            Vector3 endPosition = center - 1.5f * entitySpacing * orientation * y - size * dragLineDirection / 2;
            y++;
            int x = 0;
            foreach (KeyValuePair<float, Entity> kvp in rowSorted)
            {

                // Determine the lerped position within the row
                float lerpFactor;
                if (row.Count > 1)
                    lerpFactor = (float)x / (row.Count - 1);
                else
                    lerpFactor = 0.5f;

                x++;

                Vector3 lerpedPos = Vector3.Lerp(startPosition, endPosition, lerpFactor);

                assignedPositions.Add(kvp.Value, lerpedPos);
                assignedOrientations.Add(kvp.Value, orientation);
            }
        }

        return assignedPositions;
    }
}
