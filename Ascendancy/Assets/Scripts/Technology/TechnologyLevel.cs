using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologyLevel : MonoBehaviour
{
    public TechnologyTree techTree { get; private set; }
    public int currentFocus;

    public SubscribableProperty<float> storedResearch;

    public SubscribableList<EntityInfo> entitiesUnlocked { get; private set; }
    public SubscribableList<BuildingInfo> buildingsUnlocked { get; private set; }
    public SubscribableList<Resource> resourcesUnlocked { get; private set; }

    // Start is called before the first frame update
    public void Initialize()
    {
        techTree = TechTreeReader.LoadTechTree();

        currentFocus = -1;
        storedResearch = new SubscribableProperty<float>(0);

        entitiesUnlocked  = new SubscribableList<EntityInfo>();
        buildingsUnlocked = new SubscribableList<BuildingInfo>();
        resourcesUnlocked = new SubscribableList<Resource>();

        List<Technology> unlockedTechs = techTree.UnlockedTechs();
        foreach (Technology tech in unlockedTechs)
            UnlockThingsFromTech(tech.id);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AddResearchPoints(500);
        }
    }

    public void AddResearchPoints(float amount)
    {
        // No current Research, so keep the points until a Focus is set.
        if (currentFocus == -1)
        {
            storedResearch.Value += amount;
            return;
        }

        // If there are stored points, add those to the progress as well,
        // but only as many as the specified amount already being added.
        // (So if there are 200 points stored, but only 50 are being added by another source, then only 50 additional points will be added from the 200 stored)
        if (storedResearch.Value > 0)
        {
            float additionalAmount = Mathf.Clamp(storedResearch.Value, 0, amount);
            amount += additionalAmount;
            storedResearch.Value -= additionalAmount;
        }

        float overflow = techTree.AddProgress(currentFocus, amount);
        if (techTree.TechResearchability(currentFocus) == Researchability.Researched)
        {
            UnlockThingsFromTech(currentFocus);
            currentFocus = -1;
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
        if (!entitiesUnlocked.Contains(info))
            entitiesUnlocked.Add(info);
    }

    public bool IsUnitUnlocked(EntityInfo unitInfo)
    {
        return entitiesUnlocked.Contains(unitInfo);
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
