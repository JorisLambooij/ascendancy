using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

public class SYS_ColorChanger : ComponentSystem
{
    protected override void OnUpdate()
    {
        
        Entities.ForEach((RenderMesh renderM) => 
        {
            renderM.material.SetColor("_Color", Color.white);
            //RenderMesh renderMesh;
            //renderMesh.material.color = playerComponent.playerColor;
        });
    }
}
