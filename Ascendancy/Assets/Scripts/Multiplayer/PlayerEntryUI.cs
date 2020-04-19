using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntryUI : MonoBehaviour
{
    public Text playerNameText;
    public Player player;

    private int playerNo;
    private Dropdown colorDropdown;
    private MP_Lobby lobby;

    void Awake()
    {
        colorDropdown = GetComponentInChildren<Dropdown>();
        lobby = GameObject.Find("PlayerManager").GetComponent<MP_Lobby>();
    }

    public PlayerInfo InfoFromEntry
    {
        get
        {
            Color playerColor = lobby.playerColors[PlayerColorIndex];
            PlayerInfo info = new PlayerInfo(playerNameText.text, PlayerNo, playerColor);
            return info;
        }
    }

    public int PlayerNo {
        get => playerNo;
        set
        {
            playerNo = value;
            //playerNameText.text = "Player " + playerNo;
        }
    }

    public int PlayerColorIndex
    {
        get => colorDropdown.value;
        set => colorDropdown.value = value;
        
    }
    public void UpdateColor()
    {
        player.playerColor = lobby.playerColors[PlayerColorIndex];
    }
}
