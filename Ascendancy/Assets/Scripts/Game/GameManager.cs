using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlModeEnum { gameMode, menuMode, buildingMode };

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    public int playerNo;
    public Player playerScript;

    public CameraScript camScript;
    public ControlMode controlMode;
    public World world;
    public TileOccupationMap occupationMap;
    public UI_Canvas UICanvas;

    // Might need refactoring
    public Dictionary<ControlModeEnum, ControlMode> controlModeDict { get; protected set; }
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        
        playerScript = GameObject.Find("PlayerManager").GetComponent<PlayerLoader>().LoadPlayersIntoScene(playerNo);
        
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
        controlMode.HandleInput();
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
        get { return GameObject.Find("Player " + playerNo).GetComponent<Player>(); }
    }
    
}
