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

    public static MP_Lobby singleton { get; private set; }

    private MPMenu_NetworkRoomManager roomMngr;

    private Player localPlayer = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (singleton != null)
        {
            Debug.Log("Destroying second MP_Lobby");
            singleton.messageWindow = messageWindow;
            singleton.Initialize();
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
            Initialize();
        }
    }

    public void Initialize()
    {
        DontDestroyOnLoad(this);
        playerDict = new Dictionary<int, Player>();
        playerList = GameObject.Find("Player List").transform;
        playersInLobby = new List<PlayerInfo>();
        playerCount = 0;
        Debug.Assert(playerColors.Count >= maxPlayers, "Not enough Player Colors!");

        Button buttonReadyStart = GameObject.Find("StartGameButton").GetComponent<Button>();

        MPMenu_NetworkRoomManager roomMngr = GameObject.Find("NetworkManager").GetComponent<MPMenu_NetworkRoomManager>();
        if (roomMngr.mode == NetworkManagerMode.Host)
        {
            isServer = true;
            buttonReadyStart.GetComponentInChildren<Text>().text = "Start Game";
            buttonReadyStart.interactable = false;
        }
        else
        {
            buttonReadyStart.GetComponentInChildren<Text>().text = "Ready";
        }

        Debug.Log("I am " + roomMngr.mode.ToString());



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
    }


    public void NetworkPlayerInitialization(Player player)
    {
        Transform playerEntry = Instantiate(playerEntryPrefab, playerList).transform;
        PlayerEntryUI entryUI = playerEntry.GetComponent<PlayerEntryUI>();

        Button kickPlayerButton = playerEntry.GetComponentInChildren<Button>();

        if (player.hasAuthority) //if true, this is the local player
        {
            localPlayer = player;

            PrefManager prefManager = GameObject.Find("PlayerPrefManager").GetComponent<PrefManager>();
            prefManager.RegisterPlayer(player);

            #region UI

            Destroy(kickPlayerButton.gameObject);

            entryUI.GetComponentInChildren<Dropdown>().interactable = true; //should be already, but better to be sure

            #endregion

            player.playerName = prefManager.GetPlayerName();

            Debug.Log(player.playerName + " connected");

        }
        else // if false, this is a client player
        {

            #region UI
            if (isServer)
                kickPlayerButton.onClick.AddListener(() => KickPlayerButtonListener(player));
            else
                Destroy(kickPlayerButton.gameObject);

            //you should only be able to change your own color
            entryUI.GetComponentInChildren<Dropdown>().interactable = false;

            #endregion

            if (isServer)
                player.RpcLookupName();

            Debug.Log("Remote player connected"); //we do not have the correct playername at this time
        }

        entryUI.player = player;
        entryUI.PlayerColorIndex = playerCount;
        entryUI.PlayerNo = (++playerCount);
        entryUI.playerNameText.text = player.playerName;

        player.playerNo = entryUI.PlayerNo;

        playerDict.Add(player.playerNo, player);

        player.setReadyEvent.AddListener(() => SetReadyEventListener(player));


        if (isServer && playerDict.Count > 1)
        {
            //Host player is ready if enough players are currently connected

            //NetworkRoomPlayer nwrPlayer = localPlayer.GetComponent<NetworkRoomPlayer>();
            //RawImage riReadyState = FindPlayerEntry(localPlayer).GetComponentInChildren<RawImage>();

            //nwrPlayer.readyToBegin = true;
            //riReadyState.color = Color.green;

            localPlayer.SetReady(true);
        }
        else if (playerDict.Count > 1)
        {
            //host should be ready, so lets manually switch host indicator to ready
            SetReady(playerDict[1], true);
        }
    }

    public void RemovePlayer(Player player)
    {
        playerDict.Remove(player.playerNo);
        PlayerEntryUI deletemeUI = FindPlayerEntry(player);
        deletemeUI.playerNameText.text = "DELETED";
        Destroy(deletemeUI.gameObject);
        PrintChatMessage(new ChatMessage("SYSTEM", player.playerName + " disconnected", Color.gray));

        if (isServer && playerDict.Count == 1)
        {
            //Host player is not ready if not enough players are currently connected

            //NetworkRoomPlayer nwrPlayer = localPlayer.GetComponent<NetworkRoomPlayer>();
            //RawImage riReadyState = FindPlayerEntry(localPlayer).GetComponentInChildren<RawImage>();

            //nwrPlayer.readyToBegin = false;
            //riReadyState.color = Color.red;

            localPlayer.SetReady(false);
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

    //public void UpdatePlayerNumbers()
    //{
    //    Debug.Log("Updating player numbers");
    //}

    public void ButtonReadyStartClick()
    {
        if (isServer)
        {
            //Button Start
            foreach (Player client in playerDict.Values)
            {
                client.RpcStartGame();
            }
        }
        else
        {
            //Button Ready
            //NetworkRoomPlayer nwrPlayer = localPlayer.GetComponent<NetworkRoomPlayer>();
            if (localPlayer.isReady())
            {
                //SetReady(localPlayer, false);
                localPlayer.SetReady(false);
            }
            else
            {
                //SetReady(localPlayer, true);
                localPlayer.SetReady(true);
            }            
        }
    }

    public void ToggleStartButton(bool on)
    {
        Button buttonReadyStart = GameObject.Find("StartGameButton").GetComponent<Button>();
        buttonReadyStart.interactable = on;
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

    private PlayerEntryUI FindPlayerEntry(Player player)
    {
        PlayerEntryUI[] entries = playerList.GetComponentsInChildren<PlayerEntryUI>();
        PlayerEntryUI correctEntry = null;
        foreach (PlayerEntryUI entry in entries)
        {
            if (entry.player.Equals(player))
            {
                correctEntry = entry;
            }
        }

        if (correctEntry == null)
        {
            Debug.LogError("Unable to find playerEntry for player " + player.playerName);
            return null;
        }
        else
        {
            return correctEntry;
        }
    }

    public void SetReady(Player player, bool ready)
    {
        Button buttonReadyStart = GameObject.Find("StartGameButton").GetComponent<Button>();
        RawImage riReadyState = FindPlayerEntry(player).GetComponentInChildren<RawImage>();
        ColorBlock cbButton = buttonReadyStart.colors;

        if (ready)
        {
            //change color of button and indicator to green
            cbButton.normalColor = Color.green;
            riReadyState.color = Color.green;
        }
        else
        {
            //change color of button and indicator to red
            cbButton.normalColor = Color.red;
            riReadyState.color = Color.red;
        }

        if (!isServer) //only clients need to change color of button
            buttonReadyStart.colors = cbButton;
        else // let us check if all players are ready
        {
            bool allReady = true;

            foreach(Player p in playerDict.Values)
            {
                if (p.isReady() == false)
                    allReady = false;
            }

            if (allReady == true)
            {
                buttonReadyStart.interactable = true;
            }
            else
            {
                buttonReadyStart.interactable = false;
            }
        }
    }

    private void SetReadyEventListener(Player player)
    {
        SetReady(player, player.isReady());
    }

        private void KickPlayerButtonListener(Player player)
    {
        Debug.Log("Trying to kick " + player.playerName);
        player.GetComponent<NetworkRoomPlayer>().connectionToClient.Disconnect();
    }
}
