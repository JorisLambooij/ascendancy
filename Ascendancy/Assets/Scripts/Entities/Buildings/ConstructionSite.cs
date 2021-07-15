using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ConstructionSite : NetworkBehaviour, OccupationType
{
    [SyncVar]
    public string buildingName;
    public EntityInfo buildingInfo;
    private ProgressBar progressbar;

    float constructionProgress;
    bool constructed;

    void Start()
    {
        progressbar = GetComponentInChildren<ProgressBar>();
        constructed = false;

    }

    public EntityInfo GetEntityInfo()
    {
        return buildingInfo;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (buildingInfo == null)
        {
            if (ResourceLoader.instance.entityInfoData.ContainsKey(buildingName))
                buildingInfo = ResourceLoader.instance.entityInfoData[buildingName];
            else
            {
                Debug.LogError("ConstructionSite has no building assigned!");
            }    

            if (buildingInfo == null)
            {
                Debug.LogError("ConstructionSite has no BuildingInfo!");
                return;
            }
        }
        constructionProgress += Time.deltaTime;

        float percentage = Mathf.Clamp01(constructionProgress / buildingInfo.buildTime);
        progressbar.percentage = percentage;

        if (constructionProgress >= buildingInfo.buildTime && !constructed)
        {
            Debug.Log("Construction Completed!!");
            constructed = true;

            GameManager.Instance.GetPlayer.CmdSpawnBuilding(buildingName, transform.position);
            // old, non-networked version
            //GameObject newBuildingGO = buildingInfo.CreateInstance(GameManager.Instance.GetPlayer, transform.position);
            //Entity b = newBuildingGO.GetComponent<Entity>();
            //GameManager.Instance.occupationMap.ClearOccupation(transform.position, buildingInfo.dimensions, TileOccupation.OccupationLayer.Building);
            //GameManager.Instance.occupationMap.NewOccupation(transform.position, b, TileOccupation.OccupationLayer.Building);

            Destroy(this.gameObject);
        }
    }

}
