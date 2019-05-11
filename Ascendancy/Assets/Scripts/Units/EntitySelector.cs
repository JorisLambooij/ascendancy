using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class enables the selection of an Entity. Does not handle Input, only Visuals.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class EntitySelector : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool selected;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Selected = false;
    }

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            spriteRenderer.enabled = value;
        }
    }
}
