﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MP_Lobby : MonoBehaviour
{
    public int maxPlayers;
    public List<Color> playerColors;
    public GameObject playerEntryPrefab;

    public List<PlayerInfo> PlayersInLobby { get => playersInLobby; }
    public bool isServer = false;

    public MessageWindow messageWindow;

    private Transform playerList;
    private List<PlayerInfo> playersInLobby;
    private int playerCount;
    private Dictionary<int, Player> playerDict;

    private MPMenu_NetworkRoomManager roomMngr;

    // Start is called before the first frame update
    void Awake()
    {
        playerDict = new Dictionary<int, Player>();
        playerList = GameObject.Find("Player List").transform;
        playersInLobby = new List<PlayerInfo>();
        playerCount = 0;
        Debug.Assert(playerColors.Count >= maxPlayers, "Not enough Player Colors!");

        roomMngr = GameObject.Find("NetworkManager").GetComponent<MPMenu_NetworkRoomManager>();
        if (roomMngr.mode == NetworkManagerMode.Host)
            isServer = true;

        Debug.Log("I am " + roomMngr.mode.ToString());

        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddPlayer(Player player)
    {
        if (player == null)
            Debug.LogError("Cannot add empty player to Lobby!");

        if (playerCount >= maxPlayers)
            return;

        Transform playerEntry = Instantiate(playerEntryPrefab, playerList).transform;
        PlayerEntryUI entryUI = playerEntry.GetComponent<PlayerEntryUI>();

        entryUI.player = player;
        entryUI.PlayerColorIndex = playerCount;
        entryUI.PlayerNo = (++playerCount);
        entryUI.playerNameText.text = player.playerName;

        Button kickPlayerButton = playerEntry.GetComponentInChildren<Button>();
        if (isServer) //player.isClient is always true somehow
        {
            kickPlayerButton.onClick.AddListener(() => kickPlayerButtonListener(player));
        }
        else
        {
            Destroy(kickPlayerButton.gameObject);
            Debug.Log("Destroy kick button, because isServer is " + isServer + " and  playernumber is " + player.playerNo);
        }

        player.playerNo = entryUI.PlayerNo;

        playerDict.Add(player.playerNo, player);

        messageWindow.ReceiveMessage("SYSTEM", Color.gray, player.playerName + " connected");
    }
    
    public void RemovePlayer(Player player)
    {
        playerDict.Remove(player.playerNo);

        PlayerEntryUI[] entries = playerList.GetComponentsInChildren<PlayerEntryUI>();
        PlayerEntryUI deletemeUI = null;
        foreach (PlayerEntryUI entry in entries)
        {
            if(entry.player.Equals(player))
            {
                deletemeUI = entry;
            }
        }

        if (deletemeUI == null)
            Debug.LogError("Unable to remove player " + player.playerName);
        else
        {
            deletemeUI.playerNameText.text = "DELETED";
            Destroy(deletemeUI.gameObject);
            messageWindow.ReceiveMessage("SYSTEM", Color.gray, player.playerName + " disconnected");
        }

        
    }

    public void PlayerDisconnected(NetworkIdentity identity)
    {
        Player dictPlayer = null;

        foreach (Player player in playerDict.Values)
        {
            if (player.netIdentity.Equals(identity))
            {
                Debug.Log("Removing entry for player" + player.playerName);
                dictPlayer = player;
            }
        }

        if (dictPlayer != null)
            RemovePlayer(dictPlayer);
    }

    public void UpdatePlayerNumbers()
    {
        Debug.Log("Updating player numbers");
    }

    public void LoadGame()
    {
        PlayerEntryUI[] entries = playerList.GetComponentsInChildren<PlayerEntryUI>();
        foreach (PlayerEntryUI entry in entries)
            playersInLobby.Add(entry.InfoFromEntry);

        NextSceneStatic.sceneName = "DEV_Terrain_New";
        SceneManager.LoadScene("LoadScreen");
        InitializePlayers();
    }

    public void InitializePlayers()
    {
        foreach (Player player in playerDict.Values)
            player.Initialize();
    }

    public Player GetPlayer(int id)
    {
        return playerDict[id];
    }

    public void ConstructPlayerData()
    {

    }
    
    private void kickPlayerButtonListener(Player player)
    {
        Debug.Log("Trying to kick " + player.playerName);

        player.GetComponent<NetworkRoomPlayer>().connectionToClient.Disconnect();

        //MPMenu_NetworkRoomManager roomMngr = GameObject.Find("NetworkManager").GetComponent<MPMenu_NetworkRoomManager>();
        //roomMngr.
    }
}
