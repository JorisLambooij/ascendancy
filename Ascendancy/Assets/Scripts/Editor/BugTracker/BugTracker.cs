using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
public class BugTracker : EditorWindow
{
    SerializedObject[] loadedBugs;
    string[] fileNames;
    Vector2 scrollPos;
    int selectedIndex;
    string selectedBug;
    BugDetails detailsWindow;
    private string searchAndOpen = "";

    [MenuItem("Window/Bug Tracker")]
    private static void OpenWindow()
    {
        BugTracker window = GetWindow<BugTracker>();
        window.minSize = new Vector2(250f, 600f);
        window.maxSize = new Vector2(250f, 600f);
        window.titleContent = new GUIContent("Bug Tracker");
    }
    private void OnEnable()
    {
        Object[] bugInfo = Resources.LoadAll("Bugs");    //, typeof(Bug));
        Debug.Log("Loaded " + bugInfo.Length + " bugs123");

        loadedBugs = new SerializedObject[bugInfo.Length];
        fileNames = new string[bugInfo.Length];

        for (int i = 0; i < bugInfo.Length; i++)
        {
            loadedBugs[i] = new SerializedObject(bugInfo[i]);
            fileNames[i] = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(bugInfo[i])).Replace(".asset", "");
        }
        Debug.Log("Loaded " + loadedBugs.Length + " bugs");

        if (loadedBugs.Length > 100)
            Debug.LogError("Fuck, more than 100!?!?!? Time to hunt bugs!");

        RefreshWindow();
    }

    void OnGUI()
    {
        #region button row 1
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(100)))
        {
            Debug.Log("New bug...");
            CreateNewBug();
        }
        if (GUILayout.Button("-", GUILayout.Width(100)))
        {
            Debug.Log("Kill bug...");
            DeleteBug(selectedBug);
        }
        GUILayout.EndHorizontal();
        #endregion

        #region scroll area

        scrollPos = GUILayout.BeginScrollView(scrollPos, true, true, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));    //this one is bullying me! -.-
        GUILayout.BeginVertical();

        for (int i = 0; i < loadedBugs.Length; i++)
        {

            GUI.backgroundColor = (selectedIndex == i) ? Color.blue : Color.white;

            string buttonName = fileNames[i].Substring(fileNames[i].Length - 1) + ": " + loadedBugs[i].FindProperty("name").stringValue + " " + loadedBugs[i].FindProperty("priority").intValue + "/10";

            if (GUILayout.Button(buttonName, GUILayout.Width(220), GUILayout.Height(50)))
            {
                selectedIndex = i;
                selectedBug = fileNames[i];
                Debug.Log("Button " + buttonName + " clicked!");
                detailsWindow = (BugDetails)EditorWindow.GetWindow(typeof(BugDetails), false, "Details of " + fileNames[i]);
                detailsWindow.Initialize(loadedBugs[i]);
            }


        }

        if (searchAndOpen != "")
        {
            for (int i = 0; i < fileNames.Length; i++)
            {
                if (fileNames[i] == searchAndOpen)
                {
                    selectedIndex = i;
                    Debug.Log("Found " + fileNames[i] + "!");

                    detailsWindow = (BugDetails)EditorWindow.GetWindow(typeof(BugDetails), false, "Details of " + fileNames[i]);
                    detailsWindow.Initialize(loadedBugs[i]);
                    break;
                }
            }

            searchAndOpen = "";
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        #endregion
    }

    private void CreateNewBug()
    {
        int i = 0;
        while (File.Exists("Assets/Resources/Bugs/bug_" + i + ".asset"))
        {
            i++;
        }

        ScriptableObject bug = ScriptableObject.CreateInstance("Bug");

        bug.name = "bug_" + i;

        AssetDatabase.CreateAsset(bug, "Assets/Resources/Bugs/bug_" + i + ".asset");

        OnEnable(); //refresh

        searchAndOpen = bug.name;
        Debug.Log("searching for new asset " + searchAndOpen);
    }

    private void DeleteBug(string bugName)
    {
        detailsWindow.Close();

        if(!AssetDatabase.DeleteAsset("Assets/Resources/Bugs/" + bugName + ".asset"))
        {
            Debug.LogError("Bug " + bugName + " could not be deleted. Ironic.");
        }

        OnEnable(); //refresh
        selectedIndex = 0;
    }

    void RefreshWindow()
    {

    }
}
#endif