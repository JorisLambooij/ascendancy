using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(HeightMapGenerator))]
public class HeightMapDebugger : MonoBehaviour
{
    public MeshRenderer heightmapTarget;
    public MeshRenderer firstDerivativeTarget;
    public MeshRenderer secondDerivativeTarget;
    
    private HeightMapGenerator heightMapGen;

    // Start is called before the first frame update
    void Start()
    {
        heightMapGen = GetComponent<HeightMapGenerator>();

        Texture2D heightmap = heightMapGen.WorldTexture(heightMapGen.noise, World.DisplayMode.Height);
        Texture2D firstDerivMap = heightMapGen.WorldTexture(heightMapGen.AmplifyCliffs(), World.DisplayMode.Height);
        Texture2D secondDerivMap = heightMapGen.WorldTexture(heightMapGen.Derivative2(heightMapGen.noise), World.DisplayMode.Height);
        

        heightmapTarget.material.SetTexture("_BaseMap", heightmap);
        firstDerivativeTarget.material.SetTexture("_BaseMap", firstDerivMap);
        secondDerivativeTarget.material.SetTexture("_BaseMap", secondDerivMap);
    }
}
