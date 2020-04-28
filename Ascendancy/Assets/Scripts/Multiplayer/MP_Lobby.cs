using Mirror;
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

    private Player localPlayer = null;

    // Start is called before the first frame update
    void Awake()
    {
        playerDict = new Dictionary<int, Player>();
        playerList = GameObject.Find("Player List").transform;
        playersInLobby = new List<PlayerInfo>();
        playerCount = 0;
        Debug.Assert(playerColors.Count >= maxPlayers, "Not enough Player Colors!");

        MPMenu_NetworkRoomManager roomMngr = GameObject.Find("NetworkManager").GetComponent<MPMenu_NetworkRoomManager>();
        if (roomMngr.mode == NetworkManagerMode.Host)
            isServer = true;

        Debug.Log("I am " + roomMngr.mode.ToString());

        DontDestroyOnLoad(this);

        Player.OnMessage += OnPlayerMessage;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddPlayer(Player player)
    {
        if (playerCount >= maxPlayers)
            return;

        Transform playerEntry = Instantiate(playerEntryPrefab, playerList).transform;
        PlayerEntryUI entryUI = playerEntry.GetComponent<PlayerEntryUI>();

        entryUI.player = player;
        entryUI.PlayerColorIndex = playerCount;
        entryUI.PlayerNo = (++playerCount);
        entryUI.playerNameText.text = player.playerName;

        player.playerNo = entryUI.PlayerNo;

        playerDict.Add(player.playerNo, player);

        if (localPlayer != null)
            PrintChatMessage(new ChatMessage("SYSTEM", player.playerName + " connected",Color.gray));

        Button kickPlayerButton = playerEntry.GetComponentInChildren<Button>();
        if (isServer)
            kickPlayerButton.onClick.AddListener(() => kickPlayerButtonListener(player));
        else
            Destroy(kickPlayerButton.gameObject);
    }

    public void LocalPlayerInitialization(Player player)
    {
        localPlayer = player;
        Debug.Log(player.playerName + " connected as a local player");
    }

    public void RemovePlayer(Player player)
    {
        playerDict.Remove(player.playerNo);

        PlayerEntryUI[] entries = playerList.GetComponentsInChildren<PlayerEntryUI>();
        PlayerEntryUI deletemeUI = null;
        foreach (PlayerEntryUI entry in entries)
        {
            if (entry.player.Equals(player))
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
            PrintChatMessage(new ChatMessage("SYSTEM", player.playerName + " disconnected", Color.gray));
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


    #region chat
    public void SendChatMessage(ChatMessage message)
    {
        Player player = NetworkClient.connection.identity.GetComponent<Player>();

        // send a message
        player.CmdSend(message);
    }

    public void SendChatMessage(string text)
    {
        Player player = NetworkClient.connection.identity.GetComponent<Player>();

        // send a message
        player.CmdSend(new ChatMessage(player.playerName, text, player.playerColor));
    }

    public void PrintChatMessage(ChatMessage message)
    {
        messageWindow.PrintMessage(message);
    }

    public void OnPlayerMessage(Player player, ChatMessage message)
    {
        PrintChatMessage(message);
    }

    #endregion

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
