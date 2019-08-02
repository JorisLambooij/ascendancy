using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecruitmentFeature", menuName = "Building Features/Recruitment Feature", order = 3)]
public class RecruitmentFeature : BuildingFeature
{
    public List<UnitInfo> recruitableUnits;

    public override void Initialize(Building building)
    {
        
    }

    public override void UpdateOverride(Building building)
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Recruit(recruitableUnits[0], building);
        }
    }

    /// <summary>
    /// Recruits a specific Unit.
    /// </summary>
    /// <param name="unitInfo">The Unit we wish to spawn.</param>
    /// <returns>True on a success, false otherwise.</returns>
    public bool Recruit(UnitInfo unitInfo, Building building)
    {
        Debug.Log("Recruit a " + unitInfo.unitName);
        // if unit is not allowed, abort
        if (!recruitableUnits.Contains(unitInfo))
            return false;

        Debug.Log("Recruitable: YES");
        // enough resources?
        // TODO

        Transform parent = building.Owner.unitsGO.transform;
        Unit newUnit = Instantiate(unitInfo.prefab, parent).GetComponent<Unit>();
        newUnit.transform.position = building.transform.position;
        
        Debug.Log("Recruitment Success");

        return true;
    }
}
