using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SYS_LevelUp : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref COM_Level levelComponent) =>
        {
            levelComponent.level += 1f * Time.DeltaTime;
        }
        );
    }
}
