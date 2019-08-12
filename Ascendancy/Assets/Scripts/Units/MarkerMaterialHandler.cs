using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerMaterialHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();

        Entity e = transform.GetComponentInParent<Entity>();
        if (e == null)
            e = transform.parent.GetComponentInParent<Entity>();

        Debug.Log(e.Owner.name);
        Debug.Log(e.Owner.playerColor);

        Color c = e.Owner.playerColor;

        sRenderer.color = c;
    }

}
