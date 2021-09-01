using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Personality", menuName ="AI Personality")]
public class AI_Personality : ScriptableObject
{
    /// <summary>
    /// Calculates the desired production surplus of a given resource that this AI will try to achieve
    /// </summary>
    /// <param name="resource">Which resource</param>
    /// <returns></returns>
    public float ResourceTarget(Resource resource)
    {
        return 25;
    }

    public float ResearchProductionTarget()
    {
        return 100;
    }


}
