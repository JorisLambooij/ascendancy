using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitmentFeature : BuildingFeature
{
    public List<UnitInfo> recuitableUnits;

    /// <summary>
    /// Recruits a specific Unit.
    /// </summary>
    /// <param name="unitInfo">The Unit we wish to spawn.</param>
    /// <returns>True on a success, false otherwise.</returns>
    bool Recruit(UnitInfo unitInfo)
    {
        // if unit is not allowed, abort
        if (!recuitableUnits.Contains(unitInfo))
            return false;

        // enough resources?

        Unit newUnit;
        return true;
    }
}
