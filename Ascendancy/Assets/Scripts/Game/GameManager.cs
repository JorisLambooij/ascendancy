using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    private MPMenu_NetworkRoomManager playerManager;

    public void Initialize(int playerID)
    {
        instance = this;
        this.playerNumber = playerID;
        playerManager = FindObjectOfType<MPMenu_NetworkRoomManager>();
        
        playerScript = playerManager.GetPlayerByID(playerNumber);

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

    public UI_Manager Ui_Manager { get => ui_Manager; }
}
