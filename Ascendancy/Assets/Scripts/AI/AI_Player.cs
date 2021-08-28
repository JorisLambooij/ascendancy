using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AI_Player : Player
{
    public AI_Personality personality;

    public Resource wood;

    private void Update()
    {
        EconomicUpdate();
    }

    private void EconomicUpdate()
    {

    }

    #region Network Initialization
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
        SpawnStartUnit();
    }

    #endregion
}
