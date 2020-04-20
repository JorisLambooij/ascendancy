using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResearchFeature", menuName = "Entity Features/Research Feature")]
public class ResearchFeature : EntityFeature
{
    public float researchProduced;

    public override void Update10Override()
    {
        Debug.Log("Researching " + entity.Owner.gameObject.name + " " + researchProduced);
        entity.Owner.TechLevel.AddResearchPoints(researchProduced);
    }
}
