using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SphereSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Sphere sphere) =>
        {
            float h, s, v;
            Color.RGBToHSV(sphere.color, out h, out s, out v);
            h += 1f * Time.DeltaTime;

            sphere.color = Color.HSVToRGB(h, s, v);
        });
    }
}
