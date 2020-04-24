using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public int playerNo;
    [SyncVar(hook =nameof(OnNameChange))]
    public string playerName;
    [SyncVar(hook=nameof(OnColorChange))]
    public Color playerColor;
    
    private Economy economy;
    private TechnologyLevel techLevel;
    private Transform buildingsGO;
    private Transform unitsGO;

    public Economy PlayerEconomy { get => economy; set => economy = value; }
    public TechnologyLevel TechLevel { get => techLevel; set => techLevel = value; }
    public Transform BuildingsGO { get => buildingsGO; set => buildingsGO = value; }
    public Transform UnitsGO { get => unitsGO; set => unitsGO = value; }

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


        //GameObject.Find("AddPlayer Button").GetComponent<Button>().onClick.AddListener(InvokeCmdNameChange);
    }

    public void Initialize()
    {
        Debug.Log("Init player " + playerNo);

        PlayerEconomy.Initialize();
        TechLevel.Initialize();
    }

    private void OnColorChange(Color oldColor, Color newColor)
    {
        Debug.Log("Color Change: " + newColor);
        //this.playerColor = newColor;
    }

    private void OnNameChange(string oldName, string newName)
    {
        Debug.Log("Name Changed by Server: " + newName);
    }
    
    public void InvokeCmdNameChange()
    {
        CmdNameChange(playerName);
    }

    [Command]
    public void CmdNameChange(string newName)
    {
        Debug.Log("Client changes name to " + newName);
        playerName = newName;
    }
}
