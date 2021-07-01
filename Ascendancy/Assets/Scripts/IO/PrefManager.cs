using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PrefManager : MonoBehaviour
{
    PlayerRoomScript player;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void RegisterPlayer(PlayerRoomScript player)
    {
        this.player = player;
        player.nameChangeEvent.AddListener(OnNameChange);
    }

    private void OnNameChange()
    {
        SetPlayerName(player.playerName);
    }

    public void SetPlayerName(string playerName)
    {
        PlayerPrefs.SetString("playerName", playerName);
        Debug.Log("New name " + playerName + " saved!");
    }

    public string GetPlayerName()
    {
        if (PlayerPrefs.HasKey("playerName"))
            return PlayerPrefs.GetString("playerName");
        else
            return "";
    }
}
