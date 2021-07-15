using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomPropertyDrawer(typeof(DamageAmount))]
public class DamageAmount_EditorScript : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var typeRect = new Rect(position.x, position.y, 100, position.height);
        var apLabel = new Rect(position.x + 110, position.y, 25, position.height);
        var apRect = new Rect(position.x + 135, position.y, 50, position.height);
        var nonApLabel = new Rect(position.x + 185, position.y, 50, position.height);
        var nonApRect = new Rect(position.x + 235, position.y, 50, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);
        EditorGUI.LabelField(apLabel, "AP");
        EditorGUI.PropertyField(apRect, property.FindPropertyRelative("APAmount"), GUIContent.none);
        EditorGUI.LabelField(nonApLabel, "non-AP");
        EditorGUI.PropertyField(nonApRect, property.FindPropertyRelative("nonAPAmount"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
    
}

