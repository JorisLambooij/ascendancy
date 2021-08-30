using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class TechnologyLevel : NetworkBehaviour
{
    public TechnologyTree techTree { get; private set; }

    [SyncVar]
    public int currentFocus;

    [SyncVar(hook=nameof(StoredResearchHook))]
    public float storedResearch;
    [HideInInspector]
    public UnityEvent<float> storedResearchUpdate;

    public SyncDictionary<int, float> techProgressSyncDict = new SyncDictionary<int, float>();

    public SubscribableList<EntityInfo> entitiesUnlocked { get; private set; }
    public SubscribableList<BuildingInfo> buildingsUnlocked { get; private set; }
    public SubscribableList<Resource> resourcesUnlocked { get; private set; }

    public RollingAverage averageResearchProduction = new RollingAverage(false);

    public override void OnStartServer()
    {
        base.OnStartServer();
        //Debug.Log("tech level " + gameObject.name);
        storedResearch = 0;
        techTree = TechTreeReader.Instance.LoadTechTree();

        foreach (KeyValuePair<int, Technology> kvp in techTree.techDictionary)
            techProgressSyncDict.Add(kvp.Key, 0);
    }

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        SetupTechs();
    }

    protected void SetupTechs()
    {
        techTree = TechTreeReader.Instance.LoadTechTree();
        SetFocus(-1);

        entitiesUnlocked = new SubscribableList<EntityInfo>();
        buildingsUnlocked = new SubscribableList<BuildingInfo>();
        resourcesUnlocked = new SubscribableList<Resource>();

        List<Technology> unlockedTechs = GetTechsByResearchability(Researchability.Researched);
        foreach (Technology tech in unlockedTechs)
            UnlockThingsFromTech(tech.id);
    }

    [Command]
    public void CmdAddResearchPoints(float amount)
    {
        AddResearchPoints(amount);
    }

    public void AddResearchPoints(float amount)
    {
        if (!isServer)
        {
            CmdAddResearchPoints(amount);
            return;
        }
        averageResearchProduction.QueueDatapoint(amount);
        // No current Research, so keep the points until a Focus is set.
        if (currentFocus == -1)
        {
            storedResearch += amount;
            return;
        }

        // If there are stored points, add those to the progress as well,
        // but only as many as the specified amount already being added.
        // (So if there are 200 points stored, but only 50 are being added by another source, then only 50 additional points will be added from the 200 stored)
        if (storedResearch > 0)
        {
            float additionalAmount = Mathf.Clamp(1 * storedResearch, 0, amount);
            amount += additionalAmount;
            storedResearch -= additionalAmount;
        }

        CheckTech(currentFocus);

        techProgressSyncDict[currentFocus] += amount;
        //float overflow = techTree.AddProgress(currentFocus, amount);
        if (TechResearchability(currentFocus) == Researchability.Researched)
        {
            RpcUnlockThingsFromTech(currentFocus);
            currentFocus = -1;
        }
    }

    public void SetFocus(int newFocus)
    {
        if (isServer)
            currentFocus = newFocus;
        else
            CmdSetFocus(newFocus);
    }

    [Command]
    protected void CmdSetFocus(int newFocus)
    {
        currentFocus = newFocus;
    }

    public void StoredResearchHook(float oldValue, float newValue)
    {
        storedResearchUpdate.Invoke(newValue);
    }

    public void ResearchFocus(int techID)
    {
        if (TechResearchability(techID) == Researchability.Researchable)
            CmdSetFocus(techID);
        else
            Debug.Log("Technology not researchable");
    }
    
    [ClientRpc]
    public void RpcUnlockThingsFromTech(int techID)
    {
        Debug.Log("Unlocked " + techTree.techDictionary[techID].name + " for " + transform.name);
        UnlockThingsFromTech(techID);
    }

    public void UnlockThingsFromTech(int techID)
    {
        Technology tech = techTree.techDictionary[techID];

        if (tech.unitsUnlocked != null)
            foreach (EntityInfo unitInfo in tech.unitsUnlocked)
                UnlockEntity(unitInfo);

        if (tech.buildingsUnlocked != null)
            foreach (EntityInfo buildingInfo in tech.buildingsUnlocked)
                UnlockEntity(buildingInfo);

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

    /// <summary>
    /// Is the specified Technology already researched?
    /// </summary>
    /// <param name="techID">The ID of the Technology.</param>
    /// <returns>True if tech if fully researched, false otherwise.</returns>
    private bool IsTechResearched(int techID)
    {
        CheckTech(techID);
        int cost = techTree.technologies[techID].cost;
        float progress = techProgressSyncDict[techID];
        return cost <= progress;
    }

    /// <summary>
    /// Is the specified Technology available for research?
    /// </summary>
    /// <param name="techID">The ID of the Technology.</param>
    /// <returns>True if all dependencies are researched AND the technology is not already researched. False otherwise.</returns>
    private bool IsTechResearchable(int techID)
    {
        if (IsTechResearched(techID))
            return false;

        foreach (int dependency in techTree.techDictionary[techID].dependencies)
            if (!IsTechResearched(dependency))
                return false;

        return true;
    }
    public List<Technology> GetTechsByResearchability(Researchability researchability)
    {
        List<Technology> matchingTechs = new List<Technology>();

        foreach (Technology tech in techTree.technologies)
            if (TechResearchability(tech.id) == researchability)
                matchingTechs.Add(tech);

        return matchingTechs;
    }

    /// <summary>
    /// Determines whether the Technology if not available for research, or if it is already researched.
    /// </summary>
    /// <param name="techID">The ID of the Technology.</param>
    /// <returns>The Researchability of the specified Technology.</returns>
    public Researchability TechResearchability(int techID)
    {
        if (IsTechResearched(techID))
            return Researchability.Researched;
        else if (IsTechResearchable(techID))
            return Researchability.Researchable;
        else
            return Researchability.NotResearchable;
    }

    private void CheckTech(int techID)
    {
        Debug.Assert(techProgressSyncDict != null, "TechSyncDict is null!");
        Debug.Assert(techProgressSyncDict.ContainsKey(techID), "TechSyncDict has no entry for " + techID + "!");
    }
}
