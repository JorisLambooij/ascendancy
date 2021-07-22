using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
public class BugDetails : EditorWindow
{
    Vector2 scrollPosition;
    SerializedObject info;

    private static void OpenWindow()
    {
        BugDetails window = GetWindow<BugDetails>();
    }

    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);


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
                   
                    if (sp.name == "description")
                    {
                        //GUILayout.Label("description"); 
                        //sp.stringValue = EditorGUILayout.TextArea(sp.stringValue, GUILayout.Height(400));
                        EditorGUILayout.PropertyField(sp, true, GUILayout.Height(400));

                    }
                    else
                    {
                        EditorGUILayout.PropertyField(sp, true);
                    }
            }
        }

        info.ApplyModifiedProperties();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();



        EditorGUIUtility.ExitGUI();
    }

    public void Initialize(SerializedObject info)
    {
        this.info = info;
        BugDetails window = GetWindow<BugDetails>();
        window.titleContent = new GUIContent(info.FindProperty("name").stringValue);
        window.maxSize = new Vector2(800f, 800f);
        window.minSize = window.maxSize;
    }
}
#endif