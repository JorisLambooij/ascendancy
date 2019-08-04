using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementMode : ControlMode
{
    BuildingInfo building;
    GameObject preview;
    
    public BuildingPlacementMode()
    {
        preview = GameObject.Find("Building Preview");
    }

    public override void HandleInput()
    {
        //Vector2 mousePos = MousePosToWorldCoordinates(Input.mousePosition);
        //Debug.Log(mousePos);

        Ray ray = gameManager.camScript.MouseCursorRay();
        RaycastHit hit;
            
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Tile tile = gameManager.world.GetTile(hit.point);

            preview.transform.position = new Vector3(tile.worldX, tile.height + 1, tile.worldZ);
            preview.GetComponent<BuildingPreview>().valid = tile.flatLand;
            //Debug.Log(gameManager.world.GetHeight(hit.point));
            //Debug.Log(gameManager.world.IsFlat(hit.point));
        }
        
    }

    private Vector2 MousePosToWorldCoordinates(Vector2 mousePos)
    {
        return gameManager.camScript.GetComponent<Camera>().ScreenToWorldPoint(mousePos);
    }
}
