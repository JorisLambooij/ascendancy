using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeightMapParameters
{
    [Tooltip("The scale of the noise function.")]
    public float noiseScale;

    [Tooltip("Number of Noise functions to sample.")]
    public int octaves;

    [Tooltip("The relative scale of each subsequent function.")]
    public float frequency;

    [Tooltip("The amount of influence each subsequent function has in the grand total.")]
    public float persistance;
}
