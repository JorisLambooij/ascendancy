using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AI_Player : Player
{
    public void SetRoomScript(PlayerRoomScript roomScript)
    {
        RoomPlayer = roomScript;
    }

    protected override void RpcLocalInitialize()
    {
        Debug.Log("Local initialization for AI " + PlayerName);
        lobby = FindObjectOfType<MP_Lobby>();
        playerID = RoomPlayer.index;
        CmdChangeID(playerID);

        //PlayerEconomy.Initialize();
        //TechLevel.Initialize();
        //FindObjectOfType<GameManager>().Initialize(playerID);
        //GetComponent<CheatCodes>().Initialize();

        SpawnStartUnit();
    }
}
