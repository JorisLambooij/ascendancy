﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EntityInfo))]
public class EntityInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EntityInfo entityInfo = (EntityInfo)target;

        DrawDefaultInspector();
        

        foreach (EntityFeature feature in entityInfo.EntityFeatures)
        {
            feature.DrawLayout();
        }
    }
}

[CustomEditor(typeof(MovementFeature))]
public class MovementFeatureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MovementFeature entityInfo = (MovementFeature)target;

        EditorGUILayout.LabelField("Movement");
        DrawDefaultInspector();
    }
}
