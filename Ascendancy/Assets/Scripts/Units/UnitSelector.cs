using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : MonoBehaviour
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
