using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AI_Player : Player
{
    public AI_Personality personality;

    private const float updateFrequency = 1f;

    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateFrequency);

            Debug.Log("update routine...");
            EconomicUpdate();
            TechnologyUpdate();
        }
    }

    #region Economy
    private void EconomicUpdate()
    {
        if (economy.availableResources == null)
            return;

        foreach(string rs in economy.availableResources)
        {
            Resource resource = ResourceLoader.GetResourceFromString(rs);
            float currentSurplus = economy.AverageProduction(resource);
            float targetSurplus = personality.ResourceTarget(resource);

            if (currentSurplus < targetSurplus)
            {
                List<EntityInfo> potentialBuildings = ResourceLoader.BuildingsForProduction(resource);
                if (potentialBuildings.Count > 0)
                    TryBuild(potentialBuildings[0]);
            }
        }
    }

    private void TryBuild(EntityInfo entityInfo)
    {
        if (!techLevel.IsUnitUnlocked(entityInfo))
            return;

        Debug.Log("Trying to build: " + entityInfo.name);

        float randomAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
        float randomDistance = Random.Range(2f, 10f);
        Vector3 randomPosition = spawnPosition + new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle) * randomDistance);

        AttemptPlaceBuilding(entityInfo, randomPosition);
    }
    #endregion

    #region Technology
    protected void TechnologyUpdate()
    {
        
        ChangeTechFocus();
    }

    protected void ChangeTechFocus()
    {
        // only change focus if not already focussing on a tech
        if (techLevel.currentFocus != -1)
            return;

        List<Technology> researchables = techLevel.GetTechsByResearchability(Researchability.Researchable);

        int bestTech = -1;
        float bestCost = Mathf.Infinity;

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
        StartCoroutine(UpdateRoutine());
    }

    #endregion
}
