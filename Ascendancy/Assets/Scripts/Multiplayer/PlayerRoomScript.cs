using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class PlayerRoomScript : NetworkRoomPlayer
{
    [Header("Player Info")]
    [SyncVar]
    public string playerName;

    [SyncVar]
    public int playerColorIndex;

    [HideInInspector]
    public UnityEvent OnColorChangeEvent;

    [HideInInspector]
    public static event Action<PlayerRoomScript, ChatMessage> OnMessage;

    public Color PlayerColor
    {
        get
        {
            return MP_Lobby.singleton.playerColors[playerColorIndex];
        }
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        //gameObject.name = "Player - " + playerName + "";
        CmdBroadcastName(playerName);
        CmdColorChange(playerColorIndex);
    }

    #region playerColor
    [Command]
    public void CmdColorChange(int newColorindex)
    {
        this.playerColorIndex = newColorindex;
        Debug.Log("Player " + playerName + " changes color to " + newColorindex);
        RpcColorChange(newColorindex);
    }

    [ClientRpc]
    public void RpcColorChange(int newColorIndex)
    {
        this.playerColorIndex = newColorIndex;
        OnColorChangeEvent.Invoke();
        Debug.Log("HOOK: Player " + playerName + " color changed to " + newColorIndex);
    }
    #endregion

    #region playerName
    [Command]
    public void CmdBroadcastName(string pname)
    {
        RpcChangeClientName(pname);
    }
    [ClientRpc]
    public void RpcChangeClientName(string pname)
    {
        gameObject.name = "Player - " + pname;
        this.playerName = pname;
    }
    #endregion

    #region Lobby
    public void SetReady(bool ready)
    {
        readyToBegin = ready;
        CmdSetReady(ready);
    }

    [Command]
    private void CmdSetReady(bool ready)
    {
        readyToBegin = ready;
    }

    public bool GetReadyState()
    {
        return readyToBegin;
    }
    #endregion


    #region Chat
    [Command]
    public void CmdSend(ChatMessage message)
    {
        if (message.message.Trim() != "")
            RpcReceive(message);
    }

    [ClientRpc]
    public void RpcReceive(ChatMessage message)
    {
        OnMessage?.Invoke(this, message);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        MP_Lobby lobby = FindObjectOfType<MP_Lobby>();
        lobby.NetworkPlayerInitialization(this);
        transform.SetParent(lobby.transform);//FindObjectOfType<MPMenu_NetworkRoomManager>().transform);
    }

    #endregion
}
