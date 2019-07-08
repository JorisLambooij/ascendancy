using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecruitmentFeature", menuName = "Building Features/Recruitment Feature", order = 3)]
public class RecruitmentFeature : BuildingFeature
{
    public List<UnitInfo> recruitableUnits;

    /// <summary>
    /// Recruits a specific Unit.
    /// </summary>
    /// <param name="unitInfo">The Unit we wish to spawn.</param>
    /// <returns>True on a success, false otherwise.</returns>
    public bool Recruit(UnitInfo unitInfo, Building building)
    {
        // if unit is not allowed, abort
        if (!recruitableUnits.Contains(unitInfo))
            return false;

        // enough resources?
        // TODO

        Transform parent = building.Owner.unitsGO.transform;
        Unit newUnit = Instantiate(unitInfo.prefab, parent).GetComponent<Unit>();
        newUnit.transform.position = building.transform.position;
        return true;
    }
}
