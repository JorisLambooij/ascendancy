using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public Vector3 topLeft;
    public Vector3 topRight;
    public Vector3 botLeft;
    public Vector3 botRight;

    public Vector3[] GetVectors()
    {
        return new Vector3[4] { topLeft, topRight, botLeft, botRight };
    }
}
