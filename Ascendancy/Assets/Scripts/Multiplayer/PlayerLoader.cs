using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    public GameObject playerPrefab;

    /// <summary>
    /// Loads instances for all players into the scene.
    /// </summary>
    /// <returns> Returns the player object that this game is in control of.</returns>
    public Player LoadPlayersIntoScene(int playerNo)
    {
        Player returnPlayer = null;
        MP_Lobby lobby = GetComponent<MP_Lobby>();
        Transform playerContainerObject = GameObject.Find("Player Container").transform;

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

}
