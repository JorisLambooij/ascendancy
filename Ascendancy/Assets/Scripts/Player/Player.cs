using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    public PlayerRoomScript roomPlayer;

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

    private MP_Lobby lobby;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Starting client! " + gameObject.name);
    }
    // When the NetworkManager creates this Player, do this
    private void Awake()
    {
        PlayerEconomy = GetComponent<Economy>();
        TechLevel = GetComponent<TechnologyLevel>();

        UnitsGO = transform.Find("Units");
        BuildingsGO = transform.Find("Buildings");
        PlayerEconomy = GetComponent<Economy>();
        TechLevel = GetComponent<TechnologyLevel>();

        Transform playerManager = FindObjectOfType<MP_Lobby>().transform;//GameObject.Find("PlayerManager").transform;
        transform.SetParent(playerManager);
        playerManager.GetComponent<MP_Lobby>().AddPlayer(this);

        if (hasAuthority)
        {
            Debug.Log("Awoken player " + gameObject.name);
            lobby = FindObjectOfType<MP_Lobby>();
            roomPlayer = lobby.localPlayer;
            Initialize();
        }
    }

    public void Initialize()
    {
        PlayerEconomy.Initialize();
        TechLevel.Initialize();
        GetComponent<CheatCodes>().Initialize();

        SpawnStartUnit(new Vector2(10 + 5 * roomPlayer.playerNumber, 10));
    }

    private void SpawnStartUnit(Vector2 startPosition)
    {
        EntityInfo esv = Resources.Load("ScriptableObjects/Buildings/Command/ESV") as EntityInfo;

        string ownerName = roomPlayer.playerName;

        if (esv != null)
            Debug.Log("Successfully loaded " + esv.name + " for player " + ownerName + " (Player " + roomPlayer.playerNumber + ")");
        else
            Debug.LogError("Could not load starting unit for player " + ownerName + " (Player " + roomPlayer.playerNumber + ")");

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
        GameObject testSphere = Instantiate(lobby.testPrefab, position, Quaternion.identity);
        //spawn the GO across the network
        NetworkServer.Spawn(newUnit);
        NetworkServer.Spawn(testSphere);
    }

}
