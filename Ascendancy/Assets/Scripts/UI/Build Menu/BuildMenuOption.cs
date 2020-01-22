using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuOption : MonoBehaviour
{
    public EntityInfo building;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        GetComponentInChildren<Image>().sprite = building.Thumbnail;
    }

    public void SelectBuilding()
    {
        gameManager.SwitchToMode(ControlModeEnum.buildingMode);
        BuildingPlacementMode buildingMode = gameManager.controlMode as BuildingPlacementMode;
        buildingMode.building = building;
    }
}
