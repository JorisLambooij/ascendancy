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

            bool validLocation = gameManager.GetPlayer.CheckPlacementValid(buildingInfo, position);
            preview.GetComponentInChildren<BuildingPreview>().Valid = validLocation;
            
            if(Input.GetMouseButtonDown(0))
                if(!orderMode)
                    gameManager.GetPlayer.AttemptPlaceBuilding(Building, position, cheatMode);
                else
                {
                    orderMode = false;

                    sender.BuildAt(position);

                    sender = null;
                    gameManager.SwitchToMode(ControlModeEnum.gameMode);
                }
        }
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
