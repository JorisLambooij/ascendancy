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
    [SyncVar]
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

    }

    public void Initialize()
    {
        Debug.Log("Init player " + playerNo);

        PlayerEconomy.Initialize();
        TechLevel.Initialize();
    }

    #region playerColor

    [Command]
    public void CmdColorChange(Color newColor, int index)
    {
        this.playerColor = newColor;
        this.playerColorIndex = index;
        colorChangeEvent.Invoke();
        Debug.Log("Player " + playerName +" changes color to " + newColor);
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
}
