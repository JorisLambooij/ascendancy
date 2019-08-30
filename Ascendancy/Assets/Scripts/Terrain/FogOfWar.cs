using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public World world;

    [Range(1, 10)]
    public int resolution = 2;

    private bool disabled = false;
    public enum FogTileState { UNKNOWN, EXPLORED, VISIBLE }
    public FogTileState[,] fogMap { get; private set; }
    private List<ISeeing> trackedEntities;
    private List<ISeeing> trackedCandidates;

    // Start is called before the first frame update
    void Start()
    {
        trackedEntities = new List<ISeeing>();
        trackedCandidates = new List<ISeeing>();

        if (world == null)
        {
            Debug.LogError("No world object in FogOfWar. FoW will be disabled.");
            disabled = true;
        }
        else
        {
            fogMap = new FogTileState[world.worldSize * resolution, world.worldSize * resolution];
        }

        LogToConsole();
    }

    // Update is called once per frame
    void Update()
    {
        //Adding the candidates outside of iterations
        trackedEntities.AddRange(trackedCandidates);
        trackedCandidates.Clear();


        if (!disabled)
        {
            Vector2Int position;

            //first, we flush the fogMap
            for (int y = 0; y < fogMap.GetLength(1); y++)
                for (int x = 0; x < fogMap.GetLength(0); x++)
                {
                    if (fogMap[x,y] == FogTileState.VISIBLE)
                        fogMap[x, y] = FogTileState.EXPLORED;
                }

            //then, we calculate the new visibles
            foreach (ISeeing e in trackedEntities)
            {
                position = EntityToFogMapPosition(e);
                fogMap[position.x, position.y] = FogTileState.VISIBLE;

                for (int i = 1; i < e.GetViewRange(); i++)
                {
                    fogMap[position.x - i   , position.y - i]       = FogTileState.VISIBLE;
                    fogMap[position.x       , position.y - i]       = FogTileState.VISIBLE;
                    fogMap[position.x + i   , position.y - i]       = FogTileState.VISIBLE;
                    fogMap[position.x + i   , position.y    ]       = FogTileState.VISIBLE;
                    fogMap[position.x + i   , position.y + i]       = FogTileState.VISIBLE;
                    fogMap[position.x       , position.y + i]       = FogTileState.VISIBLE;
                    fogMap[position.x - i   , position.y + i]       = FogTileState.VISIBLE;
                    fogMap[position.x - i   , position.y - i]       = FogTileState.VISIBLE;
                }
            }
        }
    }

    private Vector2Int EntityToFogMapPosition(ISeeing e)
    {
        //TODO: calculate position
        return new Vector2Int(20, 90);
    }

    private void LogToConsole()
    {
        string log = "Logging " + this.name + ":\r\n";
        string line;
        for (int y = 0; y < fogMap.GetLength(1); y++)
        {
            line = "|";
            for (int x = 0; x < fogMap.GetLength(0); x++)
            {
                line += (int)fogMap[x, y] + "|";
            }
            log += line + "\r\n";
        }
        Debug.Log(log);
    }

    public void AddSeeingEntity(ISeeing e)
    {
        trackedCandidates.Add(e);
    }
}
