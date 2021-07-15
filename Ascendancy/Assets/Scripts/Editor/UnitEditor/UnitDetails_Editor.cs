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
    }

    private void OnEnable()
    {

    }

    void OnGUI()
    {
        #region button row
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test", GUILayout.Width(60)))
        {
            Debug.Log("Test");
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.BeginVertical(GUILayout.Width(600));

        if (info != null)
        {
            info.Update();
            //EditorGUILayout.LabelField("name:");
            //EditorGUILayout.PropertyField(info.FindProperty("name"), GUILayout.Width(400));

            var sp = info.GetIterator();

            while (sp.NextVisible(true))
            {
                //if (sp.propertyType == SerializedPropertyType.ObjectReference)

                if (sp != null)
                    if (sp.name == "m_Script")
                    {
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(sp, true);
                        Debug.Log(sp.name);
                    }
            }

            //info.ApplyModifiedProperties();
        }

        info.ApplyModifiedProperties();
        GUILayout.EndVertical();

        EditorGUIUtility.ExitGUI();
    }

    public void Initialize(SerializedObject info)
    {
        this.info = info;
        UnitDetails_Editor window = GetWindow<UnitDetails_Editor>();
        window.titleContent = new GUIContent(info.FindProperty("name").stringValue);
        window.maxSize = new Vector2(800f, 800f);
        window.minSize = window.maxSize;
    }
}
#endif