using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUpdate : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Building[] buildings = GetComponentsInChildren<Building>();

        foreach(Building b in buildings)
        {

        }
    }
}
