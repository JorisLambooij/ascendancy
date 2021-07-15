using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnFeature", menuName = "Entity Features/Debug Spawn Feature")]
public class DebugSpawnFeature : EntityFeature
{
    public EntityInfo spawnedUnit;

    public override void Update10Override()
    {
        Transform parent = entity.Owner.UnitsGO.transform;
        GameObject newUnit = spawnedUnit.CreateInstance(entity.Owner, entity.transform.position);
    }
}
