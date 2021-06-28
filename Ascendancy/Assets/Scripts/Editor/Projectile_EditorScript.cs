using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if (UNITY_EDITOR)
using UnityEditor;
using UnityEditor.UIElements;

[CustomEditor(typeof(RangedAttackFeature))]
public class Projectile_EditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (RangedAttackFeature)target;
        
        DrawDefaultInspector();
        if (script.projectileInfo != null)
            script.projectileInfo.DoAdditionalLayout();
    }
}
#endif