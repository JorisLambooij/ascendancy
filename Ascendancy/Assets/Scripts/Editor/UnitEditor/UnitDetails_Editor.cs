using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
public class UnitDetails_Editor : EditorWindow
{
    SerializedObject info;

    private static void OpenWindow()
    {
        UnitDetails_Editor window = GetWindow<UnitDetails_Editor>();
        window.minSize = new Vector2(600f, 600f);
        window.maxSize = new Vector2(600, 600f);        
    }

    private void OnEnable()
    {     

    }

    void OnGUI()
    {
        #region button row
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Width(60)))
        {
            Debug.Log("Saving");
            //SaveEntityInfo(info);
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.BeginVertical(GUILayout.Width(600));

        if (info != null)
        {
            info.Update();
            EditorGUILayout.LabelField("name:");
            EditorGUILayout.PropertyField(info.FindProperty("name"), GUILayout.Width(400));
            info.ApplyModifiedProperties();
        }

        GUILayout.EndVertical();

        EditorGUIUtility.ExitGUI();
    }

    public void Initialize(SerializedObject info)
    {
        this.info = info;
        UnitDetails_Editor window = GetWindow<UnitDetails_Editor>();
        window.titleContent = new GUIContent(info.FindProperty("name").stringValue);
        window.maxSize = new Vector2(600f, 600f);
        window.minSize = window.maxSize;
    }
}
#endif