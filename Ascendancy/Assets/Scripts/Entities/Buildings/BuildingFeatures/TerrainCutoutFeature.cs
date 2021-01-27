using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTerrainCutoutFeature", menuName = "Entity Features/Graphical/Terrain Cutout Feature")]
public class TerrainCutoutFeature : EntityFeature
{
    [Header("Custom Dimensions")]
    public bool useCustomDimensions = false;
    public int customXSize = 1;
    public int customYSize = 1;
    public int customXOffset = 0;
    public int customYOffset = 0;


    private List<Vector2Int> holes;

    public override void Initialize(Entity entity)
    {
        base.Initialize(entity);

        holes = new List<Vector2Int>();

        Vector3 pos = entity.transform.position;

        Vector2Int tilePos = (World.Instance as World).IntVector(pos);

        if (!useCustomDimensions)
        {
            Vector2Int dimensions = entity.entityInfo.dimensions;

            for (int x = 0; x < dimensions.x; x++)
                for (int y = 0; y < dimensions.y; y++)
                {
                    (World.Instance as World).SetTileVisible(tilePos.x - x, tilePos.y - y, false);
                    holes.Add(new Vector2Int(tilePos.x - x, tilePos.y - y));
                }
        }
        else
        {
            Vector2Int dimensions = new Vector2Int(customXSize, customYSize);

            for (int x = 0; x < dimensions.x; x++)
                for (int y = 0; y < dimensions.y; y++)
                {
                    (World.Instance as World).SetTileVisible(tilePos.x - x + customXOffset, tilePos.y - y + customYOffset, false);
                    holes.Add(new Vector2Int(tilePos.x - x, tilePos.y - y));
                }
        }
    }

    private void OnDestroy()
    {
        //onDestroy, make tiles visible again
        foreach (Vector2Int holePos in holes)
            (World.Instance as World).SetTileVisible(holePos.x, holePos.y, true);
    }
}
