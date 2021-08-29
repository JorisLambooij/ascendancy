using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    public bool AttemptPlaceBuilding(EntityInfo buildingInfo, Vector3 position, bool cheat = false)
    {
        bool validLocation = CheckPlacementValid(buildingInfo, position);
        if (validLocation)
        {
            if (cheat || EnoughResources(buildingInfo))
            {
                if (!cheat)
                    foreach (ResourceAmount res_amount in buildingInfo.resourceAmount)
                        economy.RemoveResourceAmount(res_amount);

                GameManager.Instance.GetPlayer.CmdSpawnConstructionSite(buildingInfo.name, position);

                return true;
            }
        }
        else
            // invalid spot, do NOT place building
            Debug.Log("Area not flat or other building here");

        return false;
    }
    public bool CheckPlacementValid(EntityInfo buildingInfo, Vector3 position)
    {
        Vector2Int intPosition = new Vector2Int((int)position.x, (int)position.z);
        // Location is valid if tile is both flatland and empty of other Entities of the same BuildingLayer.
        bool flatArea = World.Instance.IsAreaFlat(intPosition, buildingInfo.dimensions);
        bool freeSpace = GameManager.Instance.occupationMap.AreTilesFree(position, buildingInfo.dimensions);

        return flatArea && freeSpace;
    }

    private bool EnoughResources(EntityInfo buildingInfo)
    {
        foreach (ResourceAmount resource_Amount in buildingInfo.resourceAmount)
        {
            if (!economy.IsRecourceAmountAvailable(resource_Amount))
            {
                Debug.Log("Not enough resources to build! (" + resource_Amount.amount + " " + resource_Amount.resource.name + ")");
                return false;
            }
        }
        return true;
    }
}
