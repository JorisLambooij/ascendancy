using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(PlayerIDHook))]
    public int playerID = -1;

    private PlayerRoomScript roomPlayer;

    private Economy economy;
    private TechnologyLevel techLevel;
    private Transform buildingsGO;
    private Transform unitsGO;

    public Economy PlayerEconomy { get => economy; set => economy = value; }
    public TechnologyLevel TechLevel { get => techLevel; set => techLevel = value; }
    public Transform BuildingsGO { get => buildingsGO; set => buildingsGO = value; }
    public Transform UnitsGO { get => unitsGO; set => unitsGO = value; }

    public string PlayerName { get; protected set; }
    public Color PlayerColor { get; protected set; }
    public PlayerRoomScript RoomPlayer
    {
        get => roomPlayer;
        set
        {
            roomPlayer = value;
            gameObject.name = roomPlayer.playerName;
        }
    }

    private MP_Lobby lobby;

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

    private void Setup()
    {
        PlayerEconomy = GetComponent<Economy>();
        TechLevel = GetComponent<TechnologyLevel>();

        UnitsGO = transform.Find("Units");
        BuildingsGO = transform.Find("Buildings");
        PlayerEconomy = GetComponent<Economy>();
        TechLevel = GetComponent<TechnologyLevel>();

        lobby = FindObjectOfType<MP_Lobby>();
        lobby.AddPlayer(this);
        transform.SetParent(FindObjectOfType<MPMenu_NetworkRoomManager>().transform);

        if (isLocalPlayer)
            RpcLocalInitialize();
    }

    public void RpcLocalInitialize()
    {
        Debug.Log("Local initialization");
        lobby = FindObjectOfType<MP_Lobby>();
        RoomPlayer = lobby.localPlayer;
        playerID = RoomPlayer.index;
        CmdChangeID(playerID);

        //PlayerEconomy.Initialize();
        TechLevel.Initialize();
        FindObjectOfType<GameManager>().Initialize(playerID);
        GetComponent<CheatCodes>().Initialize();

        SpawnStartUnit(new Vector2(10 + 5 * RoomPlayer.index, 10));
    }

    private void SpawnStartUnit(Vector2 startPosition)
    {
        float tileSize = (World.Instance as World)?.tileSize ?? 1;
        Vector2 position = new Vector3(startPosition.x * tileSize + (tileSize / 2), startPosition.y * tileSize + (tileSize / 2));
        float height = (World.Instance as World)?.GetHeight(position) ?? 1;

        CmdSpawnUnit("E.S.V.", new Vector3(position.x, height, position.y));
    }

    [Command]
    public void CmdSpawnUnit(string entityName, Vector3 position)
    {
        //Debug.Log("Cmd Spawning unit for " + this.name + " " + this.connectionToClient);
        EntityInfo entityInfo = ResourceLoader.instance.entityInfoData[entityName];
        GameObject newUnit = entityInfo.CreateInstance(this, position);

        //spawn the GO across the network
        NetworkServer.Spawn(newUnit, this.connectionToClient);
        //newUnit.GetComponent<Entity>().RpcSetOwner(transform);

        //GameObject testUnit = Instantiate(lobby.testPrefab);
        //testUnit.name = "Test Sphere";
        //testUnit.GetComponent<NetworkSphereTest>().testValue = 2;
        //NetworkServer.Spawn(testUnit);

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
}
