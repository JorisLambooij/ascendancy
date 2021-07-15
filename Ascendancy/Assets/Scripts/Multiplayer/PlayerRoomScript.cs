using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class PlayerRoomScript : NetworkRoomPlayer
{
    [Header("Player Info")]
    [SyncVar(hook = nameof(OnNameChange))]
    public string playerName;

    [SyncVar(hook = nameof(HookColorChange))]
    public int playerColorIndex;

    [HideInInspector]
    public UnityEvent nameChangeEvent = new UnityEvent();
    [HideInInspector]
    public UnityEvent setReadyEvent = new UnityEvent();
    [HideInInspector]
    public static event Action<PlayerRoomScript, ChatMessage> OnMessage;

    public Color PlayerColor
    {
        get
        {
            return MP_Lobby.singleton.playerColors[playerColorIndex];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            CmdSpawnTest();
        }
    }

    [Command]
    public void CmdSpawnTest()
    {
        GameObject test = Instantiate(FindObjectOfType<MP_Lobby>().testPrefab, new Vector3(10, 2, 10), Quaternion.identity);
        NetworkServer.Spawn(test);
    }

    #region playerColor
    [Command]
    public void CmdColorChange(int newColorindex)
    {
        this.playerColorIndex = newColorindex;
        //colorChangeEvent.Invoke();
        Debug.Log("Player " + playerName + " changes color to " + newColorindex);
    }

    public void HookColorChange(int oldColorIndex, int newColorIndex)
    {
        this.playerColorIndex = newColorIndex;
        Debug.Log("HOOK: Player " + playerName + " color changed from " + oldColorIndex + " to " + newColorIndex);
    }
    #endregion

    #region playerName

    private void OnNameChange(string oldName, string newName)
    {
        Debug.Log("Name Changed by Server: " + newName);
        nameChangeEvent.Invoke();
        gameObject.name = "Player (" + newName + ")";
    }

    public void InvokeCmdNameChange()
    {
        Debug.Log("InvokeCmdNameChange");
        if (hasAuthority)
            CmdNameChange(playerName);
    }

    [Command]
    public void CmdNameChange(string newName)
    {
        Debug.Log("Client changes name to " + newName);
        playerName = newName;
        //if (hasAuthority)
        //    CmdNameChange(newName);
    }

    [ClientRpc]
    public void RpcLookupName()
    {
        PrefManager prefManager = GameObject.Find("PlayerPrefManager").GetComponent<PrefManager>();
        playerName = prefManager.GetPlayerName();
        Debug.Log("playername is now " + playerName);
        if (hasAuthority)
            CmdNameChange(playerName);
    }

    #endregion

    #region Lobby
    public void SetReady(bool ready)
    {
        readyToBegin = ready;
        setReadyEvent.Invoke();
        CmdSetReady(ready);
    }

    [Command]
    private void CmdSetReady(bool ready)
    {
        readyToBegin = ready;

        setReadyEvent.Invoke();
        Debug.Log("Player " + playerName + " is ready? " + ready);
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
        FindObjectOfType<MP_Lobby>().NetworkPlayerInitialization(this);
    }

    #endregion
}
