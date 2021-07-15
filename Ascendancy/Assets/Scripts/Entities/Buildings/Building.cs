using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{
    public BuildingInfo buildingInfo;
    
    public override void ClickOrder(RaycastHit hit, bool enqueue, bool ctrl = false)
    {
        bool success = false;
        int i = 0;
        while (!success && i < features.Count)
            success = features[i++].ClickOrder(hit, enqueue, ctrl);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        currentHealth = buildingInfo.maxHealth;

        foreach (EntityFeature feature in buildingInfo.entityFeatures)
        {
            Debug.Log("feature: " + feature);
            feature.Initialize(this);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        foreach (EntityFeature feature in buildingInfo.entityFeatures)
            feature.UpdateOverride();
    }
}
