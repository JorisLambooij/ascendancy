using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodes : MonoBehaviour
{
    public bool cheatMode = true;

    private Player player;
    private bool ingame = false;

    public void Initialize()
    {
        ingame = true;
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!ingame)
            return;

        if (!cheatMode)
            return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Y))
                player.TechLevel.AddResearchPoints(500);
            
            if (Input.GetKeyDown(KeyCode.T))
                UnlockAllResearch();
            
            if (Input.GetKeyDown(KeyCode.R))
                InfiniteResources();

            if (Input.GetKeyDown(KeyCode.F))
                UncoverFogOfWar();
        }
    }

    private void UncoverFogOfWar()
    {
        GameObject.Find("FOW Camera").GetComponent<Camera>().backgroundColor = Color.white;
    }

    private void InfiniteResources()
    {
        foreach (Resource r in player.PlayerEconomy.availableResources.AsList)
            player.PlayerEconomy.SetResourceAmount(r, 1000000);
    }

    private void UnlockAllResearch()
    {
        foreach (Technology tech in player.TechLevel.techTree.technologies)
        {
            if (player.TechLevel.techTree.TechResearchability(tech.id) != Researchability.Researched)
            {
                player.TechLevel.techTree.techProgress.SetValue(tech.id, tech.cost);
                player.TechLevel.UnlockThingsFromTech(tech.id);
            }
        }
    }
}
