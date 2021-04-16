using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : MonoBehaviour, OccupationType
{
    public EntityInfo buildingInfo;
    private ProgressBar progressbar;

    float constructionProgress;

    void Start()
    {
        progressbar = GetComponentInChildren<ProgressBar>();
    }

    public EntityInfo GetEntityInfo()
    {
        return buildingInfo;
    }

    // Update is called once per frame
    protected void Update()
    {
        constructionProgress += Time.deltaTime;

        float percentage = Mathf.Clamp01(constructionProgress / buildingInfo.buildTime);
        progressbar.percentage = percentage;

        if (constructionProgress >= buildingInfo.buildTime)
        {
            Debug.Log("Construction Completed!!");

            GameObject newBuildingGO = buildingInfo.CreateInstance(GameManager.Instance.GetPlayer, transform.position);
            Entity b = newBuildingGO.GetComponent<Entity>();

            GameManager.Instance.occupationMap.ClearOccupation(transform.position, b.entityInfo.dimensions, TileOccupation.OccupationLayer.Building);
            GameManager.Instance.occupationMap.NewOccupation(transform.position, b, TileOccupation.OccupationLayer.Building);

            Destroy(this.gameObject);
        }
    }
}
