using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MP_Lobby : MonoBehaviour
{
    public int maxPlayers;

    public GameObject playerEntryPrefab;

    private Transform playerList;
    private List<PlayerInfo> playersInLobby;

    private int playerCount;

    public List<Color> playerColors;

    public List<PlayerInfo> PlayersInLobby { get => playersInLobby; }

    // Start is called before the first frame update
    void Start()
    {
        playerList = GameObject.Find("Player List").transform;
        playersInLobby = new List<PlayerInfo>();
        playerCount = 0;
        Debug.Assert(playerColors.Count >= maxPlayers, "Not enough Player Colors!");

        AddPlayer();

        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddPlayer()
    {
        if (playerCount >= maxPlayers)
            return;

        Transform playerEntry = Instantiate(playerEntryPrefab, playerList).transform;
        PlayerEntryUI entryUI = playerEntry.GetComponent<PlayerEntryUI>();

        entryUI.PlayerColor = playerColors[playerCount];
        entryUI.PlayerNo = (++playerCount);
    }

    public void StartGame()
    {
        PlayerEntryUI[] entries = playerList.GetComponentsInChildren<PlayerEntryUI>();
        foreach (PlayerEntryUI entry in entries)
            playersInLobby.Add(entry.InfoFromEntry);

        NextSceneStatic.sceneName = "DEV_Units";
        SceneManager.LoadScene("LoadScreen");
    }

    public void ConstructPlayerData()
    {

    }
}
