using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SetOrderInLayer : MonoBehaviour
{
    [SerializeField]
    private int layer;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.sortingOrder = layer;
    }
    
}
