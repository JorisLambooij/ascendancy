using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct COM_PlayerInfo : IComponentData
{
    public byte playerID;
    public Color playerColor;
}
