using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using UnityEngine.Events;

public partial class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(PlayerIDHook))]
    public int playerID = -1;

    protected PlayerRoomScript roomPlayer;

    protected Economy economy;
    protected TechnologyLevel techLevel;
    protected Transform buildingsGO;
    protected Transform unitsGO;
    protected MP_Lobby lobby;

    [SyncVar]
    public Vector3 spawnPosition;

    public Economy PlayerEconomy { get => economy; set => economy = value; }
    public TechnologyLevel TechLevel { get => techLevel; set => techLevel = value; }
    public Transform BuildingsGO { get => buildingsGO; set => buildingsGO = value; }
    public Transform UnitsGO { get => unitsGO; set => unitsGO = value; }

    public string PlayerName { get; protected set; }
    public Color PlayerColor { get { return roomPlayer.PlayerColor; } }

    public PlayerRoomScript RoomPlayer
    {
        get => roomPlayer;
        set
        {
            roomPlayer = value;
            gameObject.name = roomPlayer.playerName;
        }
    }
}
