using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{
    public BuildingInfo buildingInfo;
    
    public override void ClickOrder(RaycastHit hit, bool enqueue)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        currentHealth = buildingInfo.MaxHealth;

        foreach (EntityFeature feature in buildingInfo.EntityFeatures)
        {
            Debug.Log("feature: " + feature);
            feature.Initialize(this);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        foreach (EntityFeature feature in buildingInfo.EntityFeatures)
            feature.UpdateOverride(this);
    }
}
