using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityFeature : ScriptableObject
{
    public abstract void Initialize(Entity entity);
    public abstract void UpdateOverride(Entity entity);
}
