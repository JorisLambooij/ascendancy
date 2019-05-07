using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlModeEnum { gameMode, menuMode };

public class GameManager : MonoBehaviour
{
    public int playerNo;
    public CameraScript camScript;

    private ControlMode controlMode;

    // Might need refactoring
    private Dictionary<ControlModeEnum, ControlMode> controlModeDict;
    
    // Start is called before the first frame update
    void Start()
    {
        ControlMode.gameManager = this;
        controlModeDict = new Dictionary<ControlModeEnum, ControlMode>
        {
            { ControlModeEnum.gameMode, new GameMode() },
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
        controlMode = controlModeDict[mode];
    }
    
}
