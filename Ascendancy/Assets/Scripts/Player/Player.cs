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
    public int PlayerNumber { get; protected set; }
    public Color PlayerColor { get; protected set; }
    public PlayerRoomScript RoomPlayer { 
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

        //Debug.Log("Starting client! " + gameObject.name);
        //Setup();
    }

    // TODO: this should ideally be moved into OnStartServer and then synced across clients (like a proper server-authoritative model)
    public override void OnStartClient()
    {
        base.OnStartClient();

        Debug.Log("Starting client! " + gameObject.name);
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
        Transform playerManager = lobby.transform;
        transform.SetParent(playerManager);

        if (isLocalPlayer)
            Initialize();
    }

    public void Initialize()
    {
        Debug.Log("local init");
        RoomPlayer = lobby.localPlayer;
        playerID = RoomPlayer.index;

        PlayerEconomy.Initialize();
        TechLevel.Initialize();
        FindObjectOfType<GameManager>().Initialize();
        GetComponent<CheatCodes>().Initialize();

        SpawnStartUnit(new Vector2(10 + 5 * RoomPlayer.index, 10));
    }

    private void SpawnStartUnit(Vector2 startPosition)
    {
        EntityInfo esv = Resources.Load("ScriptableObjects/Buildings/Command/ESV") as EntityInfo;

        string ownerName = RoomPlayer.playerName;

        if (esv != null)
            Debug.Log("Successfully loaded " + esv.name + " for player " + ownerName + " (Player " + RoomPlayer.index + ")");
        else
            Debug.LogError("Could not load starting unit for player " + ownerName + " (Player " + RoomPlayer.index + ")");

        float tileSize = (World.Instance as World)?.tileSize ?? 1;
        Vector2 position = new Vector3(startPosition.x * tileSize + (tileSize / 2), startPosition.y * tileSize + (tileSize / 2));
        float height = (World.Instance as World)?.GetHeight(position) ?? 1;

        //Debug.Log(position);

        CmdSpawnUnit("ScriptableObjects/Buildings/Command/ESV", new Vector3(position.x, height, position.y));
    }

    [Command]
    public void CmdSpawnUnit(string assetPath, Vector3 position)
    {
        Debug.Log("Cmd Spawning unit for " + this.name);
        EntityInfo entityInfo = Resources.Load(assetPath) as EntityInfo;
        GameObject newUnit = entityInfo.CreateInstance(this, position);
        //spawn the GO across the network
        NetworkServer.Spawn(newUnit);
    }

    public void PlayerIDHook(int oldValue, int newValue)
    {
        playerID = newValue;
        //lobby?.playerDict.TryGetValue(newValue, out roomPlayer);

        PlayerRoomScript[] playerRoomScripts = FindObjectsOfType<PlayerRoomScript>();
        foreach (PlayerRoomScript p in playerRoomScripts)
            if (p.index == newValue)
                RoomPlayer = p;
    }


}
