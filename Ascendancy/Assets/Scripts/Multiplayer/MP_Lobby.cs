using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MP_Lobby : MonoBehaviour
{
    public int maxPlayers;
    public List<Color> playerColors;
    public GameObject playerEntryPrefab;

    public List<PlayerInfo> PlayersInLobby { get => playersInLobby; }

    private Transform playerList;
    private List<PlayerInfo> playersInLobby;
    private int playerCount;
    private Dictionary<int, Player> playerDict;

    // Start is called before the first frame update
    void Start()
    {
        playerDict = new Dictionary<int, Player>();
        playerList = GameObject.Find("Player List").transform;
        playersInLobby = new List<PlayerInfo>();
        playerCount = 0;
        Debug.Assert(playerColors.Count >= maxPlayers, "Not enough Player Colors!");
        
        DontDestroyOnLoad(this);
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

        entryUI.PlayerColorIndex = playerCount;
        entryUI.PlayerNo = (++playerCount);
        entryUI.playerNameText.text = player.playerName;
        entryUI.player = player;
        entryUI.UpdateColor();

        player.playerNo = entryUI.PlayerNo;

        playerDict.Add(player.playerNo, player);
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
        foreach (KeyValuePair<int, Player> kvp in playerDict)
            kvp.Value.Initialize();
    }

    public void ConstructPlayerData()
    {

    }
}
