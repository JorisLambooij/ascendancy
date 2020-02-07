using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntryUI : MonoBehaviour
{
    //public Image background;
    public Text playerNameText;

    private int playerNo;

    private Color playerColor;

    public PlayerInfo InfoFromEntry
    {
        get
        {
            PlayerInfo info = new PlayerInfo(playerNameText.text, PlayerNo, playerColor);
            return info;
        }
    }

    public int PlayerNo {
        get => playerNo;
        set
        {
            playerNo = value;
            playerNameText.text = "Player " + playerNo;
        }
    }

    //public Color PlayerColor {
    //    get => playerColor;
    //    set
    //    {
    //        playerColor = value;
    //        background.color = playerColor;
    //    }
    //}
}
