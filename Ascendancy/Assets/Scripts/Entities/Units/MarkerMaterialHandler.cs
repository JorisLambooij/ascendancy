using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerMaterialHandler : MonoBehaviour
{
    public Color markerColor = Color.magenta;

    // Start is called before the first frame update
    void Start()
    {
        bool hasOwner = false;

        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();

        Entity e = transform.GetComponentInParent<Entity>();
        if (e == null)
            e = transform.parent.GetComponentInParent<Entity>();

        if (e != null)
            if (e.Owner != null)
                hasOwner = true;
        
        Color c = Color.magenta;

        if (!markerColor.Equals(Color.magenta))
            c = markerColor;
        else if (hasOwner)
            c = e.Owner.PlayerColor;
        else
        {
            Debug.LogError("No color information for " + this.transform.parent.name + "'s minimap marker");
        }

        c.a = 1;

        sRenderer.color = c;
    }

}
