using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public int playerNo;
    [SyncVar(hook = nameof(OnNameChange))]
    public string playerName;
    [SyncVar]
    public Color playerColor;
    [SyncVar(hook = nameof(HookColorChange))]
    public int playerColorIndex;

    private Economy economy;
    private TechnologyLevel techLevel;
    private Transform buildingsGO;
    private Transform unitsGO;

    public Economy PlayerEconomy { get => economy; set => economy = value; }
    public TechnologyLevel TechLevel { get => techLevel; set => techLevel = value; }
    public Transform BuildingsGO { get => buildingsGO; set => buildingsGO = value; }
    public Transform UnitsGO { get => unitsGO; set => unitsGO = value; }

    public static event Action<Player, ChatMessage> OnMessage;

    //Events
    public UnityEvent nameChangeEvent = new UnityEvent();
    public UnityEvent colorChangeEvent = new UnityEvent();
    public UnityEvent setReadyEvent = new UnityEvent();

    private MP_Lobby lobby;

    // When the NetworkManager creates this Player, do this
    private void Awake()
    {
        PlayerEconomy = GetComponent<Economy>();
        TechLevel = GetComponent<TechnologyLevel>();

        UnitsGO = transform.Find("Units");
        BuildingsGO = transform.Find("Buildings");
        PlayerEconomy = GetComponent<Economy>();
        TechLevel = GetComponent<TechnologyLevel>();

        Transform playerManager = GameObject.Find("PlayerManager").transform;
        transform.SetParent(playerManager);
        playerManager.GetComponent<MP_Lobby>().AddPlayer(this);


        lobby = GameObject.Find("PlayerManager").GetComponent<MP_Lobby>();
    }

    public void Initialize()
    {
        PlayerEconomy.Initialize();
        TechLevel.Initialize();
        GetComponent<CheatCodes>().Initialize();
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
        this.playerColor = lobby.playerColors[newColorIndex];
        Debug.Log("HOOK: Player " + playerName + " color changed from " + oldColorIndex + " to " + newColorIndex);
    }

    #endregion

    #region playerName

    private void OnNameChange(string oldName, string newName)
    {
        Debug.Log("Name Changed by Server: " + newName);
        nameChangeEvent.Invoke();
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
        if (hasAuthority)
            CmdNameChange(newName);
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
        GetComponentInParent<MP_Lobby>().NetworkPlayerInitialization(this);
        base.OnStartClient();
    }

    #endregion

    #region Lobby

    public void SetReady(bool ready)
    {
        NetworkRoomPlayer nwrPlayer = GetComponent<NetworkRoomPlayer>();
        nwrPlayer.readyToBegin = ready;
        setReadyEvent.Invoke();
        CmdSetReady(ready);
    }

    [Command]
    private void CmdSetReady(bool ready)
    {
        NetworkRoomPlayer nwrPlayer = GetComponent<NetworkRoomPlayer>();
        nwrPlayer.readyToBegin = ready;

        setReadyEvent.Invoke();
        Debug.Log("Player " + playerName + " is ready? " + ready);
    }


    public bool isReady()
    {
        return GetComponent<NetworkRoomPlayer>().readyToBegin;
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        Debug.Log("Starting game for client " + playerName);
        GetComponentInParent<MP_Lobby>().LoadGame();
    }
    #endregion

    
}
