using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class Player
{
    public override void OnStartServer()
    {
        base.OnStartServer();

        //Debug.Log("Starting server! " + gameObject.name);
        //Setup();
        //RpcLocalInitialize();
    }

    // TODO: this should ideally be moved into OnStartServer and then synced across clients (like a proper server-authoritative model)
    public override void OnStartClient()
    {
        base.OnStartClient();
        //Debug.Log("Starting client! " + gameObject.name);
        Setup();
    }

    protected void Setup()
    {
        //Debug.Log("Setup for " + gameObject.name);
        PlayerEconomy = GetComponent<Economy>();
        TechLevel = GetComponent<TechnologyLevel>();

        UnitsGO = transform.Find("Units");
        BuildingsGO = transform.Find("Buildings");
        PlayerEconomy = GetComponent<Economy>();
        TechLevel = GetComponent<TechnologyLevel>();

        PlayerEconomy.Initialize();

        lobby = FindObjectOfType<MP_Lobby>();
        lobby.AddPlayer(this);
        transform.SetParent(FindObjectOfType<MPMenu_NetworkRoomManager>().transform);

        if (isLocalPlayer || isServer && this is AI_Player)
            RpcLocalInitialize();
    }

    protected virtual void RpcLocalInitialize()
    {
        lobby = FindObjectOfType<MP_Lobby>();
        RoomPlayer = lobby.localPlayer;
        playerID = RoomPlayer.index;
        //Debug.Log("Local initialization for " + RoomPlayer.playerName);
        CmdChangeID(playerID);

        FindObjectOfType<GameManager>().Initialize(playerID);
        GetComponent<CheatCodes>().Initialize();

        SpawnStartUnit();
    }
    [Command]
    public void CmdChangeID(int newID)
    {
        playerID = newID;
    }

    public void PlayerIDHook(int oldValue, int newValue)
    {
        //Debug.Log("player id hook: " + newValue);
        playerID = newValue;
        //lobby?.playerDict.TryGetValue(newValue, out roomPlayer);

        PlayerRoomScript[] playerRoomScripts = FindObjectsOfType<PlayerRoomScript>();
        foreach (PlayerRoomScript p in playerRoomScripts)
            if (p.index == newValue)
                RoomPlayer = p;
    }


    public void SpawnStartUnit()
    {
        // wait until the world has finished initialization, and spawnpoints are set
        // this should in theory be redundant, but i decided to leave it in just in case
        //yield return new WaitWhile(() => !(World.Instance?.GetComponentInChildren<SpawnPoints>()?.spawnPoints != null));
        float tileSize = World.Instance.tileSize;
        Vector2 spawnPoint = World.Instance.GetComponentInChildren<SpawnPoints>().spawnPoints[playerID];
        Vector2 worldPosition = new Vector2(spawnPoint.x * tileSize + (tileSize / 2), spawnPoint.y * tileSize + (tileSize / 2));
        if (isLocalPlayer)
            CameraScript.instance.FocusOn(worldPosition + Vector2.down * 2);
        float height = World.Instance.GetHeight(worldPosition);

        spawnPosition = new Vector3(worldPosition.x, height - 0.05f, worldPosition.y);

        if (isServer)
            SpawnUnit("E.S.V.", spawnPosition);
        else
            CmdSpawnUnit("E.S.V.", spawnPosition);
    }

    protected void SpawnUnit(string entityName, Vector3 position)
    {
        EntityInfo entityInfo = ResourceLoader.instance.entityInfoData[entityName];
        GameObject newUnit = entityInfo.CreateInstance(this, position);

        //spawn the GO across the network
        NetworkServer.Spawn(newUnit, this.connectionToClient);
    }

    [Command]
    public void CmdSpawnUnit(string entityName, Vector3 position)
    {
        SpawnUnit(entityName, position);
    }

    [Command]
    public void CmdSpawnConstructionSite(string entityName, Vector3 position)
    {
        GameObject constructionSite = Instantiate(ResourceLoader.instance.constructionSitePrefab);
        constructionSite.transform.position = position;
        constructionSite.GetComponent<ConstructionSite>().buildingName = entityName;
        constructionSite.GetComponent<ConstructionSite>().ownerID = this.playerID;

        NetworkServer.Spawn(constructionSite, this.connectionToClient);
    }


    public void SpawnBuilding(string entityName, Vector3 position)
    {
        CmdSpawnBuilding(entityName, position);
    }

    [Command]
    public void CmdSpawnBuilding(string entityName, Vector3 position)
    {
        Debug.Log("Spawning building: " + entityName);
        EntityInfo entityInfo = ResourceLoader.instance.entityInfoData[entityName];
        GameObject newUnit = entityInfo.CreateInstance(this, position);
        //spawn the GO across the network
        NetworkServer.Spawn(newUnit, this.connectionToClient);

        Entity b = newUnit.GetComponent<Entity>();

        GameManager.Instance.occupationMap.ClearOccupation(newUnit.transform.position, entityInfo.dimensions, TileOccupation.OccupationLayer.Building);
        GameManager.Instance.occupationMap.NewOccupation(newUnit.transform.position, b, TileOccupation.OccupationLayer.Building);
    }
}
