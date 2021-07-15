using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
public class UnitDetails_Editor : EditorWindow
{
    SerializedObject info;
    Editor gameObjectEditor;
    Vector2 scrollPosition;
    bool refreshPreview = false;

    private static void OpenWindow()
    {
        UnitDetails_Editor window = GetWindow<UnitDetails_Editor>();
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
                    if (sp.name == "m_Script")
                    {
                    }
                    else if (sp.name == "prefab")
                    {
                        GameObject gameObject = (GameObject)sp.objectReferenceValue;

                        //if (gameObject != null)
                        //{
                        GUILayout.BeginHorizontal();

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(sp, true, GUILayout.Width(550f));
                        if (EditorGUI.EndChangeCheck())
                        {
                            refreshPreview = true;
                        }
                        

                        if (gameObjectEditor == null)
                        {
                            gameObjectEditor = Editor.CreateEditor(gameObject);
                            Debug.Log("3D preview created");
                        }
                        else if (refreshPreview)
                        {
                            gameObjectEditor = Editor.CreateEditor(gameObject);
                            refreshPreview = false;
                            Debug.Log("3D preview refreshed");
                        }

                        if (gameObject != null)
                            gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(50, 50), EditorStyles.whiteLabel);
                        else
                        { 
                        }
                        GUILayout.EndHorizontal();
                        //}
                    }
                    else if (sp.name == "thumbnail")
                    {
                        GUILayout.BeginHorizontal();
                        //sp.objectReferenceValue = EditorGUI.ObjectField(GUILayoutUtility.GetRect(50, 50), sp.objectReferenceValue, typeof(Texture2D), false);
                        sp.objectReferenceValue = EditorGUILayout.ObjectField("Thumbnail", sp.objectReferenceValue, typeof(Sprite), false);
                        GUILayout.EndHorizontal();


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
        UnitDetails_Editor window = GetWindow<UnitDetails_Editor>();
        window.titleContent = new GUIContent(info.FindProperty("name").stringValue);
        window.maxSize = new Vector2(800f, 800f);
        window.minSize = window.maxSize;
        refreshPreview = true;
    }
}
#endif