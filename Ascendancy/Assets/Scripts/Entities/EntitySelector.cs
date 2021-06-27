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

        EntityInfo info = transform.parent.GetComponent<Entity>().entityInfo;
        transform.localScale = new Vector3(info.dimensions.x * 0.75f, info.dimensions.y * 0.75f, 1);

        Selected = false;
    }

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            if (spriteRenderer != null)
                spriteRenderer.enabled = value;
        }
    }

    public Entity ParentEntity
    {
        get { return GetComponentInParent<Entity>(); }
    }
}
