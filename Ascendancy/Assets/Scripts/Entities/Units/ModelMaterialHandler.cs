using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will change the color of the shader according to the color of the player that owns this Entity.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class ModelMaterialHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Entity e = transform.GetComponentInParent<Entity>();
        if (e == null)
            e = transform.parent.GetComponentInParent<Entity>();

        Color c = e.Owner.playerColor;
        Debug.Log(c);

        Material mat = GetComponent<MeshRenderer>().material;

        mat.SetColor("_Color_Standard", c);
    }
}
