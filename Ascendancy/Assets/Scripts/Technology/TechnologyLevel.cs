﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologyLevel : MonoBehaviour
{
    public TechnologyTree techTree { get; private set; }
    public int currentFocus;

    private HashSet<EntityInfo> unitsUnlocked;
    private HashSet<BuildingInfo> buildingsUnlocked;
    private HashSet<Resource> resourcesUnlocked;

    // Start is called before the first frame update
    public void Initialize()
    {
        techTree = TechTreeReader.LoadTechTree();

        unitsUnlocked     = new HashSet<EntityInfo>();
        buildingsUnlocked = new HashSet<BuildingInfo>();
        resourcesUnlocked = new HashSet<Resource>();

        List<Technology> unlockedTechs = techTree.UnlockedTechs();
        foreach (Technology tech in unlockedTechs)
            UnlockThingsFromTech(tech.id);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            techTree.AddProgress(currentFocus, 1000);
            if (techTree.TechResearchability(currentFocus) == Researchability.Researched)
                UnlockThingsFromTech(currentFocus);
        }
    }

    public void ResearchFocus(int techID)
    {
        if (techTree.TechResearchability(techID) == Researchability.Researchable)
            currentFocus = techID;
        else
            Debug.Log("Technology not researchable");
    }
    
    public void UnlockThingsFromTech(int techID)
    {
        Technology tech = techTree.techDictionary[techID];

        if (tech.unitsUnlocked != null)
            foreach (EntityInfo unitInfo in tech.unitsUnlocked)
                UnlockEntity(unitInfo);
        /*
        if (tech.buildingsUnlocked != null)
            foreach (BuildingInfo buildingInfo in tech.buildingsUnlocked)
                UnlockEntity(buildingInfo);
        */
        if (tech.resourcesUnlocked != null)
            foreach (Resource resource in tech.resourcesUnlocked)
            {
                resourcesUnlocked.Add(resource);
                GetComponent<Economy>().NewAvailableResource(resource);
            }

    }

    private void UnlockEntity(EntityInfo info)
    {
        if (!unitsUnlocked.Contains(info))
            unitsUnlocked.Add(info);
    }

    public bool IsUnitUnlocked(EntityInfo unitInfo)
    {
        return unitsUnlocked.Contains(unitInfo);
    }

    public bool IsBuildingUnlocked(BuildingInfo buildingInfo)
    {
        return buildingsUnlocked.Contains(buildingInfo);
    }

    public bool IsResourceUnlocked(Resource resource)
    {
        return resourcesUnlocked.Contains(resource);
    }
}
