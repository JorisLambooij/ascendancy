using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuOption : MonoBehaviour
{
    public BuildingInfo building;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void SelectBuilding()
    {
        gameManager.SwitchToMode(ControlModeEnum.buildingMode);
        BuildingPlacementMode buildingMode = gameManager.controlMode as BuildingPlacementMode;
        buildingMode.building = building;
    }
}
