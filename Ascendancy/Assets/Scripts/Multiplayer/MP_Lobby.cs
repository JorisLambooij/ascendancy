using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MP_Lobby : NetworkBehaviour
{
    public int maxPlayers;
    public List<Color> playerColors;
    public GameObject playerEntryPrefab;
    public GameObject testPrefab;

    public List<PlayerInfo> PlayersInLobby { get => playersInLobby; }
    public bool isServer = false;

    public MessageWindow messageWindow;

    private Transform playerList;
    private List<PlayerInfo> playersInLobby;
    private int playerCount;
    public Dictionary<int, PlayerRoomScript> playerDict;

    public static MP_Lobby singleton { get; private set; }

    private MPMenu_NetworkRoomManager roomMngr;

    public PlayerRoomScript localPlayer = null;

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
        playerDict = new Dictionary<int, PlayerRoomScript>();
        playerList = GameObject.Find("Player List").transform;
        playersInLobby = new List<PlayerInfo>();
        playerCount = 0;
        Debug.Assert(playerColors.Count >= maxPlayers, "Not enough Player Colors!");

        Button buttonReadyStart = GameObject.Find("StartGameButton").GetComponent<Button>();

        roomMngr = GameObject.Find("NetworkManager").GetComponent<MPMenu_NetworkRoomManager>();
        if (roomMngr.mode == NetworkManagerMode.Host)
        {
            isServer = true;
            buttonReadyStart.GetComponentInChildren<Text>().text = "Start Game";
            buttonReadyStart.interactable = false;
        }
        else
        {
            buttonReadyStart.interactable = false;
        }

        Debug.Log("I am " + roomMngr.mode.ToString());


        roomMngr.InitPlayerDict(playerDict);
        PlayerRoomScript.OnMessage += OnPlayerMessage;

        messageWindow = FindObjectOfType<MessageWindow>();
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

    
    public void NetworkPlayerInitialization(PlayerRoomScript player)
    {
        Debug.Log("NetworkPlayerInit");
        Transform playerEntry = Instantiate(playerEntryPrefab, playerList).transform;
        PlayerEntryUI entryUI = playerEntry.GetComponent<PlayerEntryUI>();

        Button kickPlayerButton = playerEntry.GetComponentInChildren<Button>();

        if (player.hasAuthority) //if true, this is the local player
        {
            localPlayer = player;

            PrefManager prefManager = GameObject.Find("PlayerPrefManager").GetComponent<PrefManager>();
            prefManager.RegisterPlayer(player);

            // UI
            Destroy(kickPlayerButton.gameObject);
            entryUI.GetComponentInChildren<Dropdown>().interactable = true; //should be already, but better to be sure

            player.playerName = prefManager.GetPlayerName();
            Debug.Log(player.playerName + " connected");

        }
        else // if false, this is a remote client / player
        {

            // UI
            if (isServer)
                kickPlayerButton.onClick.AddListener(() => KickPlayerButtonListener(player));
            else
                Destroy(kickPlayerButton.gameObject);

            //you should only be able to change your own color
            entryUI.GetComponentInChildren<Dropdown>().interactable = false;

            //if (isServer)
            //    player.RpcLookupName();

            Debug.Log("Remote player connected"); //we do not have the correct playername at this time
        }


        entryUI.player = player;

        player.playerColorIndex = playerCount;
        entryUI.PlayerColorIndex = playerCount;

        entryUI.PlayerNo = (++playerCount);
        entryUI.playerNameText.text = player.playerName;
        player.OnColorChangeEvent.AddListener(entryUI.OnColorChange);

        player.index = entryUI.PlayerNo;

        playerDict.Add(player.index, player);

        //player.setReadyEvent.AddListener(() => SetReadyEventListener(player));

        /*
        if (isServer && playerDict.Count > 1)
        {
            //Host player is ready if enough players are currently connected

            //NetworkRoomPlayer nwrPlayer = localPlayer.GetComponent<NetworkRoomPlayer>();
            //RawImage riReadyState = FindPlayerEntry(localPlayer).GetComponentInChildren<RawImage>();

            //nwrPlayer.readyToBegin = true;
            //riReadyState.color = Color.green;

            //localPlayer.SetReady(true);
        }
        else if (playerDict.Count > 1)
        {
            //host should be ready, so lets manually switch host indicator to ready
            SetReady(playerDict[1], true);
        }
        */
    }
    

    public void ButtonReadyStartClick()
    {
        bool ready = !localPlayer.GetReadyState();
        localPlayer.CmdChangeReadyState(ready);
        localPlayer.SetReady(ready);
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

        //ClientScene.RegisterPrefab(testPrefab);
        RpcLoadGame();

        //InitializePlayers();
    }

    [ClientRpc]
    public void RpcLoadGame()
    {
        Debug.Log("Loading game...");
        NextSceneStatic.sceneName = "GameScene";
        SceneManager.LoadScene("LoadScreen");

        //localPlayer.Initialize();
    }

    [ServerCallback]
    public void InitializePlayers()
    {
        //foreach (PlayerRoomScript player in playerDict.Values)
        //    player.Initialize();

        Debug.Log("Initializing Players");
        //DEVSpawnStartUnitsForAll();

        //GameObject testUnit = Instantiate(testPrefab);
        //testUnit.name = "NetworkSphere-Test";
        //NetworkServer.Spawn(testUnit, playerDict[1].connectionToClient);
        //NetworkServer.Spawn(testUnit, playerDict[2].connectionToClient);
        //NetworkServer.Spawn(testUnit);

        //Debug.Log(connectionToClient);
        //Debug.Log(playerDict[2].name + " : " + playerDict[2].connectionToClient.isReady);
    }

    public PlayerRoomScript GetPlayer(int id)
    {
        return playerDict[id];
    }

    private PlayerEntryUI FindPlayerEntry(PlayerRoomScript player)
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

    
    public void SetReady(PlayerRoomScript player, bool ready)
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

            foreach (PlayerRoomScript p in playerDict.Values)
            {
                if (p.GetReadyState() == false)
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

    #region chat
    public void SendChatMessage(ChatMessage message)
    {
        PlayerRoomScript player = NetworkClient.connection.identity.GetComponent<PlayerRoomScript>();

        // send a message
        player.CmdSend(message);
    }

    public void SendChatMessage(string text)
    {
        PlayerRoomScript player = NetworkClient.connection.identity.GetComponent<PlayerRoomScript>();

        // send a message
        player.CmdSend(new ChatMessage(player.playerName, text, player.PlayerColor));
    }

    public void PrintChatMessage(ChatMessage message)
    {
        messageWindow.PrintMessage(message);
    }

    public void OnPlayerMessage(PlayerRoomScript player, ChatMessage message)
    {
        PrintChatMessage(message);
    }

    #endregion

    private void SetReadyEventListener(PlayerRoomScript player)
    {
        Debug.Log("Ready player " + player.name);
        SetReady(player, player.GetReadyState());
    }

    private void KickPlayerButtonListener(PlayerRoomScript player)
    {
        Debug.Log("Trying to kick " + player.playerName);
        player.GetComponent<NetworkRoomPlayer>().connectionToClient.Disconnect();
    }
    
}
