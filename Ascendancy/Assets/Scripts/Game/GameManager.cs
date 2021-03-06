﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlModeEnum { gameMode, menuMode, buildingMode };

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    public int playerNumber;
    public Player playerScript;

    public CameraScript camScript;
    public ControlMode controlMode;
    public World world;
    public TileOccupationMap occupationMap;
    private UI_Manager ui_Manager;

    // Might need refactoring
    public Dictionary<ControlModeEnum, ControlMode> controlModeDict { get; protected set; }

    private MP_Lobby playerManager;

    public void Initialize()
    {
        instance = this;

        playerManager = FindObjectOfType<MP_Lobby>();

        Player[] allPlayers = playerManager.GetComponentsInChildren<Player>();
        foreach(Player player in allPlayers)
            if (player.PlayerNumber == playerNumber)
                playerScript = player;   //.GetComponent<PlayerLoader>().LoadPlayersIntoScene(playerNo);

        Debug.Log("Found local Player: " + playerScript);

        // Create a new UI Manager
        ui_Manager = Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Manager")).GetComponent<UI_Manager>();

        ControlMode.gameManager = this;
        controlModeDict = new Dictionary<ControlModeEnum, ControlMode>
        {
            { ControlModeEnum.gameMode, new GameMode() },
            { ControlModeEnum.buildingMode, new BuildingPlacementMode() },
            { ControlModeEnum.menuMode, new MenuMode() }
        };
        SwitchToMode(ControlModeEnum.gameMode);
    }
    
    // Update is called once per frame
    void Update()
    {
        controlMode?.HandleInput();
    }

    public void SwitchToMode(ControlModeEnum mode)
    {
        if (controlMode != null)
            controlMode.Stop();

        controlMode = controlModeDict[mode];
        controlMode.Start();
    }

    public Player GetPlayer
    {
        get 
        {
            return playerScript;
            //return playerManager.GetPlayer(playerNumber);
        }
    }

    public Player[] GetPlayers
    {
        get { return playerManager.GetComponentsInChildren<Player>(); }
    }

    public UI_Manager Ui_Manager { get => ui_Manager; }
}
