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
            try
            {
                return MP_Lobby.instance.playerColors[playerColorIndex];
            }
            catch
            {
                throw new Exception("Index out of range: " + playerColorIndex);
            }
        }
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();
        Debug.Log("Client entered room: " + gameObject.name + " (" + playerName + ")");
        //gameObject.name = "Player - " + playerName + "";

        CmdBroadcastName(playerName);
        CmdColorChange(playerColorIndex);
    }

    #region playerColor
    public void ColorChange(int newColorIndex)
    {
        this.playerColorIndex = newColorIndex;
        //Debug.Log("Player " + playerName + " changes color index to " + newColorIndex);
        CmdColorChange(newColorIndex);
        if (isServer)
            RpcColorChange(newColorIndex);
        //OnColorChangeEvent.Invoke();
    }

    [Command]
    public void CmdColorChange(int newColorIndex)
    {
        this.playerColorIndex = newColorIndex;
        //Debug.Log("Player " + playerName + " changes color to " + newColorIndex);
        RpcColorChange(newColorIndex);
    }

    [ClientRpc]
    public void RpcColorChange(int newColorIndex)
    {
        this.playerColorIndex = newColorIndex;
        OnColorChangeEvent.Invoke();
        //Debug.Log("HOOK: Player " + playerName + " color changed to " + newColorIndex);
    }
    #endregion

    #region playerName
    [Command]
    public void CmdBroadcastName(string pname)
    {
        gameObject.name = "Player - " + pname;
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
        MP_Lobby.instance.NetworkPlayerInitialization(this);
        transform.SetParent(MP_Lobby.instance.transform);//FindObjectOfType<MPMenu_NetworkRoomManager>().transform);
    }

    #endregion
}
