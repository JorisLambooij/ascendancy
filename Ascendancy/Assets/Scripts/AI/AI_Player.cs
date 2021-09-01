using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AI_Player : Player
{
    public AI_Personality personality;

    private const float updateFrequency = 1f;
    private const int cooldown = 4;

    #region Economy
    private IEnumerator EconomicUpdate()
    {
        if (economy?.availableResources == null)
            yield return null;

        // to avoid spamming actions, set each resource update loop on cooldown when action is taken
        Dictionary<string, int> resourceCooldowns = new Dictionary<string, int>();
        while (true)
        {
            // for every resource unlocked, try to meet the production quota given by the AI personality
            foreach (string rs in economy.availableResources)
            {
                if (!resourceCooldowns.ContainsKey(rs))
                    resourceCooldowns.Add(rs, 0);

                if (resourceCooldowns[rs] > 0)
                {
                    resourceCooldowns[rs]--;
                    continue;
                }

                Resource resource = ResourceLoader.GetResourceFromString(rs);
                float currentSurplus = economy.AverageProduction(resource);
                float targetSurplus = personality.ResourceTarget(resource);

                if (currentSurplus < targetSurplus)
                {
                    List<EntityInfo> potentialBuildings = ResourceLoader.GetBuildingsForResourceProduction(resource);
                    // TODO: actually evaluate cost/benefits, not just the most powerful one

                    for (int i = 0; i < potentialBuildings.Count; i++)
                        if (TryBuild(potentialBuildings[i]))
                        {
                            resourceCooldowns[rs] = cooldown;
                            break;
                        }
                }
            }
            yield return new WaitForSeconds(updateFrequency);
        }
    }

    private bool TryBuild(EntityInfo entityInfo)
    {
        if (!techLevel.IsUnitUnlocked(entityInfo))
            return false;

        Debug.Log("Trying to build: " + entityInfo.name);

        float randomAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
        float randomDistance = Random.Range(2f, 10f);
        Vector3 randomPosition = spawnPosition + new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle) * randomDistance);

        return AttemptPlaceBuilding(entityInfo, randomPosition);
    }
    #endregion

    #region Technology
    protected IEnumerator TechnologyUpdate()
    {
        while (true)
        {
            ChangeTechFocus();

            // research quota is not met, build research buildings
            if (techLevel?.averageResearchProduction.Calculate() < personality.ResearchProductionTarget())
            {
                List<EntityInfo> potentialBuildings = ResourceLoader.GetBuildingsForResearchProduction();
                //Debug.Log("Not enough research " + potentialBuildings[0].name);
                // TODO: actually evaluate cost/benefits, not just the most powerful one

                for (int i = 0; i < potentialBuildings.Count; i++)
                    if (TryBuild(potentialBuildings[i]))
                    {
                        yield return new WaitForSeconds(cooldown * updateFrequency);
                    }
            }

            yield return new WaitForSeconds(updateFrequency);
        }
    }

    protected void ChangeTechFocus()
    {
        // only change focus if not already focussing on a tech
        if (techLevel?.currentFocus != -1)
            return;

        List<Technology> researchables = techLevel.GetTechsByResearchability(Researchability.Researchable);

        int bestTech = -1;
        float bestCost = Mathf.Infinity;

        // pick the cheapest tech currently available
        foreach (Technology tech in researchables)
        {
            if (tech.cost < bestCost)
            {
                bestTech = tech.id;
                bestCost = tech.cost;
            }
        }

        techLevel.SetFocus(bestTech);
    }
    #endregion

    #region Network Initialization
    public void SetRoomScript(PlayerRoomScript roomScript)
    {
        RoomPlayer = roomScript;
    }

    protected override void RpcLocalInitialize()
    {
        Debug.Log("Local initialization for AI " + PlayerName);
        lobby = FindObjectOfType<MP_Lobby>();
        playerID = RoomPlayer.index;
        CmdChangeID(playerID);
        SpawnStartUnit();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        Debug.Log("Starting AI update routine");
        StartCoroutine(EconomicUpdate());
        StartCoroutine(TechnologyUpdate());
    }

    #endregion
}
