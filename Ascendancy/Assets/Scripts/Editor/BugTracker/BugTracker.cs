using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
public class BugTracker : EditorWindow
{
    SerializedObject[] loadedBugs;
    Vector2 scrollPos;
    int selectedIndex;
    string selectedBug;
    BugDetails detailsWindow;
    private string searchAndOpen = "";
    bool prioritySort = false;

    [MenuItem("Window/Bug Tracker")]
    private static void OpenWindow()
    {
        GetWindow<BugDetails>().Close();
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

        for (int i = 0; i < bugInfo.Length; i++)
        {
            loadedBugs[i] = new SerializedObject(bugInfo[i]);
        }


        if (prioritySort)
        {
            string indexName = loadedBugs[selectedIndex].FindProperty("name").stringValue;


            System.Array.Sort(loadedBugs, delegate (SerializedObject bug1, SerializedObject bug2)
            {
                return bug1.FindProperty("priority").intValue.CompareTo(bug2.FindProperty("priority").intValue);
            });
        }

        Debug.Log("Loaded " + loadedBugs.Length + " bugs");

        if (loadedBugs.Length > 100)
            Debug.LogError("Fuck, more than 100!?!?!? Time to hunt bugs!");

        //OnEnable(); //refresh
    }

    void OnGUI()
    {
        #region button row 1
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            Debug.Log("New bug...");
            CreateNewBug();
        }
        else if (GUILayout.Button("-", GUILayout.Width(50)))
        {
            Debug.Log("Kill bug...");
            DeleteBug(selectedBug);
        }

        GUILayout.EndHorizontal();
        #endregion

        #region button row 2
        GUILayout.BeginHorizontal();

        if (prioritySort)
        {
            if (GUILayout.Button("Sort by: Index", GUILayout.Width(100)))
            {
                prioritySort = false;
                OnEnable(); //refresh
            }
        }
        else
        {
            if (GUILayout.Button("Sort by: Priority", GUILayout.Width(100)))
            {
                prioritySort = true;
                OnEnable(); //refresh
            }
        }

        GUILayout.EndHorizontal();
        #endregion

        #region scroll area

        scrollPos = GUILayout.BeginScrollView(scrollPos, true, true, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));    //this one is bullying me! -.-
        GUILayout.BeginVertical();

        for (int i = 0; i < loadedBugs.Length; i++)
        {

            GUI.backgroundColor = (selectedIndex == i) ? Color.blue : Color.white;

            string buttonName = loadedBugs[i].FindProperty("name").stringValue + " " + loadedBugs[i].FindProperty("priority").intValue + "/10";

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(buttonName, GUILayout.Width(200), GUILayout.Height(50)))
            {
                selectedIndex = i;
                selectedBug = loadedBugs[i].FindProperty("name").stringValue;
                Debug.Log("Button " + buttonName + " clicked!");
            }

            if (GUILayout.Button("->", GUILayout.Width(20), GUILayout.Height(50)))
            {
                detailsWindow = (BugDetails)EditorWindow.GetWindow(typeof(BugDetails), false, "Details of " + loadedBugs[i].FindProperty("name").stringValue);
                detailsWindow.Initialize(loadedBugs[i]);
                this.Close();
            }
            GUILayout.EndHorizontal();
        }
        


        if (searchAndOpen != "")
        {
            for (int i = 0; i < loadedBugs.Length; i++)
            {
                if (loadedBugs[i].FindProperty("name").stringValue == searchAndOpen)
                {
                    selectedIndex = i;
                    Debug.Log("Found " + loadedBugs[i].FindProperty("name").stringValue + "!");

                    detailsWindow = (BugDetails)EditorWindow.GetWindow(typeof(BugDetails), false, "Details of " + loadedBugs[i].FindProperty("name").stringValue);
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
        if (detailsWindow != null)
            detailsWindow.Close();

        Object[] bugInfo = Resources.LoadAll("Bugs");    //, typeof(Bug));
        Debug.Log("Loaded " + bugInfo.Length + " bugs123");

        loadedBugs = new SerializedObject[bugInfo.Length];

        string fileName = "";

        for (int i = 0; i < bugInfo.Length; i++)
        {
            loadedBugs[i] = new SerializedObject(bugInfo[i]);
            if (loadedBugs[i].FindProperty("name").stringValue == bugName)
                fileName = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(bugInfo[i])).Replace(".asset", "");
        }

        if (fileName != "")
            if (!AssetDatabase.DeleteAsset("Assets/Resources/Bugs/" + fileName + ".asset"))
            {
                Debug.LogError("Bug " + bugName + " could not be deleted. Ironic.");
            }

        OnEnable(); //refresh
        selectedIndex = 0;
    }

    public void RefreshWindow()
    {
        OnEnable();
    }

}
#endif