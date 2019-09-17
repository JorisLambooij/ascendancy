using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntryUI : MonoBehaviour
{
    public Image background;
    public Text playerNameField;
    public Text playerNamePlaceholder;

    private int playerNo;

    private Color playerColor;

    public PlayerInfo InfoFromEntry
    {
        get
        {
            PlayerInfo info = new PlayerInfo(playerNameField.text, PlayerNo, playerColor);
            return info;
        }
    }

    public int PlayerNo {
        get => playerNo;
        set
        {
            playerNo = value;
            playerNamePlaceholder.text = "Player " + playerNo;
        }
    }

    public Color PlayerColor {
        get => playerColor;
        set
        {
            playerColor = value;
            background.color = playerColor;
        }
    }
}
