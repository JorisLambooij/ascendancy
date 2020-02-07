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

    public Text pointsUI;

    private Dictionary<int, TechField> techFieldsDict;

    void Start()
    {
        Player playerScript = GameObject.Find("Game Manager").GetComponent<GameManager>().playerScript;
        playerTechLevel = playerScript.transform.GetComponent<TechnologyLevel>();

        SetUpTechScreen();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GetComponentInParent<UI_Canvas>().CloseAllScreens();
            gameObject.SetActive(false);
    }
    }
    private void SetUpTechScreen()
    {
        techFieldsDict = new Dictionary<int, TechField>();
        TechnologyTree techTree = playerTechLevel.techTree;
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
            foreach (int dependendy in kvp.Value.dependencies)
            {
                UILineRenderer line = Instantiate(linePrefab, linesParent).GetComponent<UILineRenderer>();

                Vector2 outPoint = techFieldsDict[dependendy].outPoint.position;
                Vector2 inPoint = techFieldsDict[kvp.Value.id].inPoint.position;
                
                line.Points = new Vector2[] { outPoint, inPoint };
            }
    }

    private void InstantiateTechField(Technology tech, Vector2 screenPosition)
    {
        TechField techField = Instantiate(techFieldPrefab, techFieldsParent).GetComponent<TechField>();
        techField.SetTechnology(tech);
        techField.transform.localPosition = screenPosition;
        techFieldsDict.Add(tech.id, techField);
    }


    public TechnologyTree TechTree
    {
        get { return playerTechLevel.techTree; }
    }
    
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

    public void Callback(int key, float newValue)
    {
        techFieldsDict[key].OnProgressUpdate(newValue);

        foreach (int techID in TechTree.techDictionary[key].leadsToTechs)
        { 
            //Debug.Log("Checking " + TechTree.techDictionary)
            techFieldsDict[techID].OnDependencyProgressUpdate();
        }
    }
}
