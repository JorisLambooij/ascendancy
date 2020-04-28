using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class PlayerTechScreen : MonoBehaviour, DictionarySubscriber<int, float>
{
    // Should be the same as TechTreeEditor.GRID_SNAP
    public int scale = 100; 

    public GameObject techFieldPrefab;
    public GameObject linePrefab;
    public Transform techFieldsParent;
    public Transform linesParent;

    public int playerID;
    public TechnologyLevel playerTechLevel;

    public Color colorIfNotResearchable;
    public Color colorIfResearchable;
    public Color colorIfResearching;
    public Color colorIfResearched;

    private Dictionary<int, TechField> techFieldsDict;

    void Awake()
    {
        Player playerScript = GameObject.Find("Game Manager").GetComponent<GameManager>().playerScript;
        playerTechLevel = playerScript.transform.GetComponent<TechnologyLevel>();

        SetUpTechScreen();
    }

    void Update()
    {
        // Close the Tech Screen.
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GameManager.Instance.ui_Manager.SetScreen("Tech Screen", false);
            //GetComponentInParent<UI_Canvas>().CloseAllScreens();
            //gameObject.SetActive(false);
        }
    }

    private void SetUpTechScreen()
    {
        techFieldsDict = new Dictionary<int, TechField>();
        TechTree.techProgress.Subscribe(this);
        
        // Make a TechField for each Technology
        foreach (KeyValuePair<int, Technology> kvp in TechTree.techDictionary)
        {
            Vector2 screenPosition = TechTree.techPosition[kvp.Key];
            screenPosition *= 3f;
            screenPosition.y *= -1;
            InstantiateTechField(kvp.Value, screenPosition);
        }

        // Connect the fields to show dependencies
        foreach (KeyValuePair<int, Technology> kvp in TechTree.techDictionary)
            foreach (int dependency in kvp.Value.dependencies)
            {
                UILineRenderer line = Instantiate(linePrefab, linesParent).GetComponent<UILineRenderer>();

                Vector2 outPoint = techFieldsDict[dependency].outPoint.position;
                Vector2 inPoint = techFieldsDict[kvp.Value.id].inPoint.position;
                
                line.Points = new Vector2[] { outPoint, inPoint };

                techFieldsDict[dependency].outgoingLines.Add(line);
                techFieldsDict[dependency].SetRightColor();
            }
    }

    /// <summary>
    /// Create the UI Element for one Technology
    /// </summary>
    /// <param name="tech">The Technology in question.s</param>
    /// <param name="screenPosition">Where on the screen this field is located.</param>
    private void InstantiateTechField(Technology tech, Vector2 screenPosition)
    {
        TechField techField = Instantiate(techFieldPrefab, techFieldsParent).GetComponent<TechField>();
        techField.SetTechnology(tech);
        techField.transform.localPosition = screenPosition;
        techFieldsDict.Add(tech.id, techField);
    }

    /// <summary>
    /// Returns the TechnologyTree that this Screen is bound to.
    /// </summary>
    public TechnologyTree TechTree
    {
        get { return playerTechLevel.techTree; }
    }
    
    /// <summary>
    /// Set the currect Research Focus to the selected Technology.
    /// </summary>
    /// <param name="techID">The ID of the Technology to research now.</param>
    public void Focus(int techID)
    {
        TechField[] techFields = GetComponentsInChildren<TechField>();

        foreach(TechField field in techFields)
        {
            if (field.Tech.id == techID)
            {
                field.isCurrentFocus = true;
                field.SetRightColor();
            }
            else
            {
                field.isCurrentFocus = false;
                field.SetRightColor();
            }
        }
    }

    /// <summary>
    /// When a Technology's progress is upated, this function is called.
    /// </summary>
    /// <param name="key">The ID of the updated Tech.</param>
    /// <param name="newValue">The new progress of the updated Tech.</param>
    public void Callback(int key, float newValue)
    {
        techFieldsDict[key].OnProgressUpdate(newValue);

        foreach (int techID in TechTree.techDictionary[key].leadsToTechs)
            techFieldsDict[techID].OnDependencyProgressUpdate();
    }
}
