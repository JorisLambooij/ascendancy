using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDiscoverOnceFeature", menuName = "Entity Features/Sight/Discover Once")]
public class DiscoverOnceFeature : EntityFeature
{
    public int radius = 5;

    public override void Initialize(Entity entity)
    {
        base.Initialize(entity);

        Vector3 pos = entity.transform.position;
        Vector2Int tilePos = (World.Instance as World).IntVector(pos);

        FogOfWarHandler fowHandler = (World.Instance as World).fowHandler;
        fowHandler.DiscoverTerrain(tilePos.x, tilePos.y, radius);
        fowHandler.UpdateMaterial();
    }
}
