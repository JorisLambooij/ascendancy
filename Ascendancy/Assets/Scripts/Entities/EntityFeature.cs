using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public abstract class EntityFeature : ScriptableObject
{
    public virtual void Initialize(Entity entity)
    {

    }
    public virtual void UpdateOverride(Entity entity)
    {

    }

    #if UNITY_EDITOR
    public void DrawLayout()
    {
        EditorGUILayout.LabelField(this.GetType().ToString(), EditorStyles.boldLabel);

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty list = serializedObject.FindProperty("test");
        
        EditorGUI.BeginChangeCheck();

        SerializedProperty prop = serializedObject.GetIterator();

        if (prop.NextVisible(true))
        {
            do
                EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
            while (prop.NextVisible(false));

        }
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
    #endif
}
