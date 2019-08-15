﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementMode : ControlMode
{
    public BuildingInfo building;
    public GameObject preview;
    
    public BuildingPlacementMode()
    {
        preview = GameObject.Find("Building Preview");
    }

    public override void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            // if Mouse over UI element, do not do anything
            return;

        if (Input.GetMouseButtonUp(1))
            //RMB, so cancel build mode
            gameManager.SwitchToMode(ControlModeEnum.gameMode);

        Ray ray = gameManager.camScript.MouseCursorRay();
        RaycastHit hit;
        
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Tile tile = gameManager.world.GetTile(hit.point);

            preview.transform.position = new Vector3(tile.worldX, tile.height + 1, tile.worldZ);

            // TODO: also check for other buildings in this spot
            bool validLocation = tile.flatLand;

            preview.GetComponent<BuildingPreview>().valid = validLocation;
            
            if (Input.GetMouseButtonDown(0))
                if (validLocation)
                {
                    // valid spot, place building
                    Debug.Log("Placing a " + building.buildingName + " at: " + preview.transform.position);
                    GameObject newBuildingGO = GameObject.Instantiate(building.prefab, gameManager.playerScript.buildingsGO.transform);
                    newBuildingGO.transform.position = preview.transform.position;

                }
                else
                {
                    // invalid spot, do NOT place building
                    Debug.Log("Not here, buckaroo");
                }
        }
        
    }
}