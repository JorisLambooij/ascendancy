using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDiscoverContinuously", menuName = "Entity Features/Sight/Discover Continuously")]
public class DiscoverContinuously : EntityFeature
{
    public int radius = 5;

    private FogOfWarHandler fowHandler;
    private Vector2Int tilePos;

    public override void Initialize(Entity entity)
    {
        base.Initialize(entity);

        //tilePos = (World.Instance as World).IntVector(entity.transform.position);

        //fowHandler = (World.Instance as World).fowHandler;
        //fowHandler.DiscoverTerrain(tilePos.x, tilePos.y, radius);
        //fowHandler.UpdateMaterial();
    }

    public override void UpdateOverride()
    {
        //tilePos = (World.Instance as World).IntVector(entity.transform.position);

        //fowHandler.DiscoverTerrain(tilePos.x, tilePos.y, radius);
        //fowHandler.UpdateMaterial();
    }
}

