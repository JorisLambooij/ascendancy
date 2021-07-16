using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
public class EntityInfoEditor : EditorWindow
{
    private SerializedObject[] loadedEIs;
    private Object[] entityCategories;
    private string[] fileNames;
    private EntityInfo selectedEntityInfo;
    private string infoPath = "ScriptableObjects/";
    private int selectedIndex;
    private Vector2 scrollPos;

    private string searchAndOpen = "";
    private string nameChange = "";

    UnitDetails_Editor detailsWindow;

    #region Filters
    private string[] typeOptions = new string[3] { "Any", "Unit", "Building" };
    private int typeIndex = 0;

    private int catIndex = 0;
    private string[] catOptions;
    #endregion

    [MenuItem("Window/Entity Editor")]
    private static void OpenWindow()
    {
        EntityInfoEditor window = GetWindow<EntityInfoEditor>();
        window.minSize = new Vector2(250f, 600f);
        window.maxSize = new Vector2(250f, 600f);
        window.titleContent = new GUIContent("Entity Editor");
    }

    private void OnEnable()
    {
        Object[] entityInfoClasses = Resources.LoadAll(infoPath, typeof(EntityInfo));
        loadedEIs = new SerializedObject[entityInfoClasses.Length];
        fileNames = new string[entityInfoClasses.Length];

        for (int i = 0; i < entityInfoClasses.Length; i++)
        {
            loadedEIs[i] = new SerializedObject(entityInfoClasses[i]);
            fileNames[i] = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(entityInfoClasses[i])).Replace(".asset", "");
        }
        Debug.Log("Loaded " + loadedEIs.Length + " Entities");

        entityCategories = Resources.LoadAll(infoPath, typeof(EntityCategoryInfo));

        RefreshWindow();
    }

    void OnGUI()
    {


        //string name = selectedEntityInfo != null ? selectedEntityInfo.name : "-";
        #region button row 1
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Unit", GUILayout.Width(100)))
        {
            Debug.Log("New Unit...");
            createNewEntity(true);
        }
        if (GUILayout.Button("New Building", GUILayout.Width(100)))
        {
            Debug.Log("New Building...");
            createNewEntity(false);
        }
        GUILayout.EndHorizontal();
        #endregion

        #region button row 2
        GUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        typeIndex = EditorGUILayout.Popup("Type:", typeIndex, typeOptions);
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("Selected: " + typeOptions[typeIndex]);
            catIndex = 0;   //reset category index to zero
        }
        GUILayout.EndHorizontal();
        #endregion

        #region row 3
        GUILayout.BeginHorizontal();
        List<string> catAll = new List<string>();

        foreach (EntityCategoryInfo eci in entityCategories)
        {
            catAll.Add(eci.name);
        }

        List<string> catList = new List<string>();
        catList.Add("Any");

        switch (typeIndex)
        {
            case 0:
                //do nothing    
                foreach (string s in catAll)
                    catList.Add(s);

                catOptions = catList.ToArray();
                break;
            case 1: //unit
                foreach (string s in catAll)
                    if (s.Contains("U_"))
                        catList.Add(s);

                catOptions = catList.ToArray();
                break;

            case 2: //building
                foreach (string s in catAll)
                    if (s.Contains("B_"))
                        catList.Add(s);

                catOptions = catList.ToArray();
                break;
            default:
                catOptions = new string[] { };
                Debug.LogError("Index out of bounds: " + typeIndex + " is not a valid index!");
                break;
        }

        EditorGUI.BeginChangeCheck();
        catIndex = EditorGUILayout.Popup("Cat:", catIndex, catOptions);
        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("Selected: " + catOptions[catIndex]);
        }
        GUILayout.EndHorizontal();
        #endregion


        scrollPos = GUILayout.BeginScrollView(scrollPos, true, true, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));    //this one is bullying me! -.-
        GUILayout.BeginVertical();

        string categoryName;

        for (int i = 0; i < loadedEIs.Length; i++)
        {
            categoryName = "no Category";
            if (loadedEIs[i].FindProperty("category").objectReferenceValue != null)
                categoryName = loadedEIs[i].FindProperty("category").objectReferenceValue.name;

            if ((
                loadedEIs[i].FindProperty("construction_Method").enumValueIndex == 0 && (typeIndex == 0 || typeIndex == 1)  //unit
                ||
                loadedEIs[i].FindProperty("construction_Method").enumValueIndex == 1 && (typeIndex == 0 || typeIndex == 2)  //building
                ) && (
                catIndex == 0                                                                                               //category == any
                ||
                categoryName == catOptions[catIndex]                     //category == category
                ))
            {
                GUI.backgroundColor = (selectedIndex == i) ? Color.blue : Color.white;

                if (GUILayout.Button(fileNames[i], GUILayout.Width(220), GUILayout.Height(50)))
                {
                    selectedIndex = i;
                    Debug.Log("Button " + fileNames[i] + " clicked!");
                    detailsWindow = (UnitDetails_Editor)EditorWindow.GetWindow(typeof(UnitDetails_Editor), false, "Details of " + fileNames[i]);
                    detailsWindow.Initialize(loadedEIs[i]);
                }
            }

        }

        if (searchAndOpen != "")
        {
            for (int i = 0; i<fileNames.Length; i++)
            {
                if(fileNames[i] == searchAndOpen)
                {
                    selectedIndex = i;
                    Debug.Log("Found " + fileNames[i] + "!");

                    if (nameChange != "")
                    {
                        loadedEIs[i].FindProperty("name").stringValue = nameChange;
                        loadedEIs[i].ApplyModifiedProperties();
                    }

                    detailsWindow = (UnitDetails_Editor)EditorWindow.GetWindow(typeof(UnitDetails_Editor), false, "Details of " + fileNames[i]);
                    detailsWindow.Initialize(loadedEIs[i]);
                    break;
                }
            }

            searchAndOpen = "";
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        EditorGUIUtility.ExitGUI();
    }

    void RefreshWindow()
    {
        
    }

    private void createNewEntity(bool isUnit)
    {
        string assetPath;
        string type;

        if (isUnit)
        {
            assetPath = "Assets/Resources/ScriptableObjects/TemplateUnit.asset";
            type = "unit";
        }
        else
        {
            assetPath = "Assets/Resources/ScriptableObjects/TemplateBuilding.asset";
            type = "building";
        }

        string newPath;

        newPath = EditorUtility.SaveFilePanel(
            "Create new" + type,
            "Assets/Resources/ScriptableObjects/" + type + "s/",
            "new_" + type + ".asset",
            "asset");

        if (newPath == "")
        {
            Debug.Log("aborted new " + type);
            return;
        };

        if (newPath.StartsWith(Application.dataPath))
        {
            newPath = "Assets" + newPath.Substring(Application.dataPath.Length);
        }
        else
        {
            throw new System.ArgumentException("Full path does not contain the current project's Assets folder: |" + newPath + "|", "newPath");
        }

        if (!AssetDatabase.CopyAsset(assetPath, newPath))
            Debug.LogWarning("Failed to save at " + newPath);
        else
        {
            string name = Path.GetFileName(newPath).Replace(".asset", "");


            Debug.Log("bad path: " + newPath);
            //change name
            nameChange = name;

            //select new asset
            typeIndex = 0;
            catIndex = 0;

            OnEnable(); //refresh
            
            searchAndOpen = name;
            Debug.Log("searching for new asset " + searchAndOpen);
        }
    }

    //private EntityInfo LoadEntityInfo()
    //{
    //    string path = EditorUtility.OpenFilePanel("Load Entity Info", infoPath, "asset");
    //    int index = path.IndexOf(infoPath);
    //    path = path.Substring(index);
    //    EntityInfo info = (EntityInfo)AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

    //    if (info == null)
    //    {
    //        Debug.LogError("Not an EntityInfo");
    //        return null;
    //    }
    //    Debug.Log("Loaded " + info.name);
    //    return null;
    //}

    private void SaveEntityInfo(EntityInfo info)
    {
        //EntityInfo asset = CreateInstance<EntityInfo>();

        if (info == null)
        {
            Debug.LogError("Could not create new Entity!");
            return;
        }


        //this line is throws an exception because it crashes the gui stack. Exit gui first if you need it.
        string path = EditorUtility.SaveFilePanel("Save Entity", infoPath, selectedEntityInfo.name, "asset");

        int index = path.IndexOf(infoPath);
        path = path.Substring(index);

        AssetDatabase.CreateAsset(info, path);
        AssetDatabase.SaveAssets();

        //EditorUtility.FocusProjectWindow();

        //Selection.activeObject = asset;
    }
}
#endif
