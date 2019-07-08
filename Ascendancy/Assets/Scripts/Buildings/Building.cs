using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{
    public BuildingInfo buildingInfo;

    public UnitInfo dummyTank;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = buildingInfo.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (buildingInfo.features.Count > 0)
                (buildingInfo.features[0] as RecruitmentFeature).Recruit(dummyTank, this);
        }
        */
    }
}
