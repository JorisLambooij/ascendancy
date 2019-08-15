using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptButtonGridClick : MonoBehaviour
{
    World world;
    bool grid = true;

    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        if (world == null)
            Debug.LogError("World not found in map editor.");
    }

    public void OnClick()
    {
        if (world == null)
            Debug.LogError("World not found in map editor.");

        grid = !grid;
        world.ToggleGrid(grid);
    }
}
