using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

public class SYS_ColorChanger : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<RenderMesh, COM_PlayerInfo>().ForEach((Entity e, ref COM_PlayerInfo info) =>
        {
            info.playerColor = Color.cyan;
            //rm.material.SetColor("_Color", Color.white);
        });
        
    }
}
