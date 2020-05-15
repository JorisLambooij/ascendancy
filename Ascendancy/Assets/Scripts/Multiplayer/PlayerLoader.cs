using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    public GameObject playerPrefab;
    private Transform playerManager;
    private Dictionary<int, Player> playerDict;

    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").transform;
    }

    /// <summary>
    /// Loads instances for all players into the scene.
    /// </summary>
    /// <returns> Returns the player object that this game is in control of.</returns>
    public Player LoadPlayersIntoScene(int playerNo)
    {
        Debug.Log("Loading Players into Scene");
        Player returnPlayer = null;
        MP_Lobby lobby = GetComponent<MP_Lobby>();
        Transform playerContainerObject = GameObject.Find("Player Container").transform;

        playerDict = new Dictionary<int, Player>(lobby.playerColors.Count);

        foreach (PlayerInfo info in lobby.PlayersInLobby)
        {
            GameObject playerGO = Instantiate(playerPrefab, playerContainerObject);
            Player playerScript = playerGO.GetComponent<Player>();

            playerScript.playerNo = info.PlayerNo;
            playerScript.playerName = info.playerName;
            playerScript.playerColor = info.playerColor;

            playerScript.Initialize();

            if (playerNo == info.PlayerNo)
                returnPlayer = playerScript;
        }

        return returnPlayer;
    }

    /// <summary>
    /// Returns a Player by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Player GetPlayer(int id)
    {
        return playerDict[id];
    }

}
