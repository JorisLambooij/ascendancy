using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public World world;
    public float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        float size = 2 * world.worldSize / world.tileSize;
        Vector3 center = new Vector3(1, 0, 1) * size;
        Vector3 upOffset = new Vector3(0, yOffset, 0);
        this.transform.position = world.transform.position + center + upOffset;
        GetComponent<Camera>().orthographicSize = size;
    }
}
