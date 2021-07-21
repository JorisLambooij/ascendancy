using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerEntryUI : NetworkBehaviour
{
    public Text playerNameText;
    public PlayerRoomScript player;

    [SyncVar]
    private int playerNo;
    private Dropdown colorDropdown;

    public RawImage riReadyState;
    public RawImage colorIndicator;

    private MP_Lobby lobby;

    void Awake()
    {
        colorDropdown = GetComponentInChildren<Dropdown>();
        lobby = FindObjectOfType<MP_Lobby>();

        //colorIndicator.gameObject.SetActive(isLocalPlayer);
    }

    void Update()
    {
        playerNameText.text = player.playerName;
        playerNameText.color = player.PlayerColor;

        riReadyState.color = player.readyToBegin ? Color.green : Color.red;
        //colorIndicator.color = player.PlayerColor;
        //this.PlayerColorIndex = player.playerColorIndex;
    }

    public PlayerInfo InfoFromEntry
    {
        get
        {
            Color playerColor = player.PlayerColor;
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
        if (player.isLocalPlayer)
            player.CmdColorChange(PlayerColorIndex);
    }
    public void OnColorChange()
    {
        colorIndicator.color = player.PlayerColor;
    }
}
