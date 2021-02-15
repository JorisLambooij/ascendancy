using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewConversionFeature", menuName = "Entity Features/Building Conversion Feature")]
public class BuildingConversionFeature : EntityFeature
{
    public EntityInfo convertedEntity;

    public override void ContextMenuOption()
    {
        //GameManager.Instance.SwitchToMode(ControlModeEnum.buildingMode);
        BuildingPlacementMode buildingMode = GameManager.Instance.controlModeDict[ControlModeEnum.buildingMode] as BuildingPlacementMode;
        buildingMode.Building = convertedEntity;

        bool success = buildingMode.AttemptPlaceBuilding(convertedEntity, entity.transform.position);
        Debug.Log(entity.transform.position);
        if (success)
            Destroy(entity.gameObject);

        //GameObject newBuilding = Instantiate(buildingPrefab, entity.transform.parent);
    }
}
