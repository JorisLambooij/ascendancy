using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public List<Vector2Int> spawnPoints;



    // debug
    // TODO: integrate properly
    public int amountOfPlayers;
    // TODO: can be removed, debug only
    public bool markSpawnPoints;

    [SerializeField]
    private float startAreaRadius;
    [SerializeField, Min(1)]
    private int iterations;

    public void SetSpawnPoints()//nt amountOfPlayers)
    {
        spawnPoints = new List<Vector2Int>();

        List<Vector2Int> randomPositions = TerrainFeature.RandomPositions(amountOfPlayers, World.Instance.worldSize, World.Instance.worldSize, 20);

        for (int i = 0; i < amountOfPlayers; i++)
        {
            Vector2Int startPos = randomPositions[i];
            for (int j = 0; j < iterations; j++)
            {
                float avgHeight = GetAvgHeight(startPos, startAreaRadius);

                if (avgHeight < 0)
                    startPos = World.Instance.MoveAgainstGradient(startPos);
                else if (avgHeight > 0)
                    startPos = World.Instance.MoveAlongGradient(startPos);
                else
                {
                    //Debug.Log("SpawnPoint: " + startPos + " AvgHeight: " + avgHeight + " (j = " + j + ")");
                    spawnPoints.Add(startPos);
                    break;
                }
                if (j == iterations - 1 && avgHeight <= 1)
                {
                    //Debug.Log("SpawnPoint desperate: " + startPos + " AvgHeight: " + avgHeight + " (j = " + j + ")");
                    spawnPoints.Add(startPos);
                    break;
                }
            }
            
        }

        if (markSpawnPoints)
            foreach (Vector2Int spawnPoint in spawnPoints)
                World.Instance.GetTile(spawnPoint).terrainType = TerrainType.NONE;
    }

    private float GetAvgHeight(Vector2Int pos, float radius)
    {
        float avgHeight = 0;
        float normalizationFactor = 0;
        int intRadius = Mathf.CeilToInt(radius);
        for (int dx = -intRadius; dx <= intRadius; dx++)
            for (int dy = -intRadius; dy <= intRadius; dy++)
            {
                Vector2Int d = new Vector2Int(dx, dy);
                Vector2Int v = pos + d;
                avgHeight += World.Instance.GetTile(v).Height;//World.Instance.GetHeight(pos + d);
                normalizationFactor += 1;
            }

        return avgHeight / normalizationFactor;
    }
}
