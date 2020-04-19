using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public string playerName;

    private int playerNo;
    public int PlayerNo { get => playerNo; private set => playerNo = value; }
    public Color playerColor;

    public PlayerInfo(string name, int playerNo, Color playerColor)
    {
        this.playerName = name;
        this.playerNo = playerNo;
        this.playerColor = playerColor;
    }
}
