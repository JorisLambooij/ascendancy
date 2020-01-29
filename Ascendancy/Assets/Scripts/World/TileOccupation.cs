using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOccupation
{
    public enum OccupationLayer { Building };
    public Player Owner;

    public Dictionary<OccupationLayer, Entity> occupation;

    public TileOccupation()
    {
        occupation = new Dictionary<OccupationLayer, Entity>();

        int count = System.Enum.GetNames(typeof(OccupationLayer)).Length;
        for (int i = 0; i < count; i++)
            occupation.Add((OccupationLayer)i, null);
    }
}
