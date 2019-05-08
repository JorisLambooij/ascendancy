using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ModelMaterialHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Color c = transform.parent.GetComponentInParent<Unit>().Owner.playerColor;

        Material mat = GetComponent<MeshRenderer>().material;

        mat.SetColor("_Color_Standard", c);
    }
}
