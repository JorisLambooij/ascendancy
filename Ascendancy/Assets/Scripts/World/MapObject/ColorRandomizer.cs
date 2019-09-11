using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    public Color randomColor1;
    public Color randomColor2;

    // Start is called before the first frame update
    void Start()
    {
        // change the colors
        foreach (MeshRenderer bobby in this.gameObject.GetComponents<MeshRenderer>())
        {
            bobby.material.SetColor("_BaseColor", Color.Lerp(randomColor1, randomColor2, Random.value));

        }
    }
}
