using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementMode : ControlMode
{
    public static bool cheatMode = false;

    private static bool orderMode = false;
    public static BuildingConversionFeature sender;

    private GameObject ghostBuilding;
    private EntityInfo buildingInfo;
    private float floorClipthrough = 0.01f;
    private GameObject constructionSitePrefab;

    public GameObject preview;

    public EntityInfo Building
    {
        get => buildingInfo;
        set
        {
            buildingInfo = value;
            GameObject.Destroy(ghostBuilding);
            ghostBuilding = GameObject.Instantiate(buildingInfo.prefab, preview.transform);
        }
    }

    public BuildingPlacementMode()
    {
        constructionSitePrefab = Resources.Load("Prefabs/Entities/Construction Site") as GameObject;
        preview = GameObject.Find("Building Preview");
        if (preview == null)
            Debug.Log("BuildingPreview not found!");
        else
            preview.SetActive(false);
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
            if (tile == null)
                return;

            int x = (int)tile.worldX, y = (int)tile.worldZ;
            Vector3 position = new Vector3(x, tile.Height - floorClipthrough, y);
            preview.transform.position = position;

            bool validLocation = CheckPlacementValid(buildingInfo, position);
            preview.GetComponentInChildren<BuildingPreview>().Valid = validLocation;
            
            if(Input.GetMouseButtonDown(0))
                if(!orderMode)
                    AttemptPlaceBuilding(Building, position, cheatMode);
                else
                {
                    orderMode = false;

                    sender.BuildAt(position);

                    sender = null;
                    gameManager.SwitchToMode(ControlModeEnum.gameMode);
                }
        }
        
    }

    public bool AttemptPlaceBuilding(EntityInfo buildingInfo, Vector3 position, bool cheat = false)
    {
        bool validLocation = CheckPlacementValid(buildingInfo, position);
        if (validLocation)
        {
            if (cheat || EnoughResources())
            {
                if (!cheat)
                    foreach (ResourceAmount res_amount in buildingInfo.resourceAmount)
                        gameManager.GetPlayer.PlayerEconomy.RemoveResourceAmount(res_amount);

                GameManager.Instance.GetPlayer.CmdSpawnConstructionSite(buildingInfo.name, position);

                // valid spot, place construction site for building
                //ConstructionSite constructionSite = GameObject.Instantiate(constructionSitePrefab, gameManager.GetPlayer.transform).GetComponent<ConstructionSite>();
                //constructionSite.transform.position = position;
                //constructionSite.buildingInfo = buildingInfo;

                // (old) direct building placement
                //GameObject newBuildingGO = Building.CreateInstance(gameManager.GetPlayer, position);
                //Entity b = newBuildingGO.GetComponent<Entity>();


                // Mark all the spots that this building occupies as occupied in the world map.
                //gameManager.occupationMap.NewOccupation(position, constructionSite, TileOccupation.OccupationLayer.Building);
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
        bool flatArea = gameManager.world.IsAreaFlat(intPosition, Building.dimensions);
        bool freeSpace = gameManager.occupationMap.AreTilesFree(position, Building.dimensions);

        return flatArea && freeSpace;
    }

    private bool EnoughResources()
    {
        foreach (ResourceAmount resource_Amount in buildingInfo.resourceAmount)
        {
            if (!gameManager.GetPlayer.PlayerEconomy.IsRecourceAmountAvailable(resource_Amount))
            {
                Debug.Log("Not enough resources to build! (" + resource_Amount.amount + " " + resource_Amount.resource.name + ")");
                return false;
            }
        }
        return true;
    }

    public override void Start()
    {
        preview.SetActive(true);
    }

    public override void Stop()
    {
        preview.SetActive(false);
    }

    public void StartOrderMode(BuildingConversionFeature sender)
    {
        orderMode = true;
        BuildingPlacementMode.sender = sender;
    }
}
