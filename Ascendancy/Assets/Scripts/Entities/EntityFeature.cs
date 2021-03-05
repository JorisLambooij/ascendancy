using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public abstract class EntityFeature : ScriptableObject
{
    /// <summary>
    /// The order in which the features are ranked when a click is registered. Higher values go first.
    /// </summary>
    public int clickPriority;

    public Sprite contextMenuThumbnail; 

    public Entity entity { get; private set; }

    public virtual void Initialize(Entity entity)
    {
        this.entity = entity;
    }

    public virtual void UpdateOverride()
    {

    }

    public virtual void Update10Override()
    {

    }

    public virtual void ContextMenuOption()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="hit"></param>
    /// <param name="enqueue"></param>
    /// <returns>True if order was successfull, false otherwise.</returns>
    public virtual bool ClickOrder(RaycastHit hit, bool enqueue = false, bool ctrl = false)
    {
        return true;
    }
    
}
