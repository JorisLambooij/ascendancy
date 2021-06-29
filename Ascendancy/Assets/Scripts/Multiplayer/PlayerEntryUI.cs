using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerEntryUI : NetworkBehaviour
{
    public Text playerNameText;
    public Player player;

    [SyncVar]
    private int playerNo;
    private Dropdown colorDropdown;
    [SyncVar]
    public int colorIndexSync; //remember to make private again
    private MP_Lobby lobby;

    void Awake()
    {
        colorDropdown = GetComponentInChildren<Dropdown>();
        lobby = FindObjectOfType<MP_Lobby>();
    }
    void Update()
    {
        playerNameText.text = player.playerName;
        playerNameText.color = player.playerColor;
        this.PlayerColorIndex = player.playerColorIndex;
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

    public int PlayerNo
    {
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
        if (player.hasAuthority)
        {
            player.playerColorIndex = PlayerColorIndex;
            player.CmdColorChange(PlayerColorIndex);
            Debug.Log("Color changed");
        }
    }
}
