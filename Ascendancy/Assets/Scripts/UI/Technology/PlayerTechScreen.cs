using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTechScreen : MonoBehaviour
{
    public int scale = TechTreeEditor.GRID_SNAP;
    public GameObject techFieldPrefab;
    public Transform techFieldsParent;

    public int playerID;
    public TechnologyLevel playerTechLevel;

    public Color colorIfNotResearchable;
    public Color colorIfResearchable;
    public Color colorIfResearching;
    public Color colorIfResearched;

    void Start()
    {
        Player playerScript = GameObject.Find("Game Manager").GetComponent<GameManager>().playerScript;
        playerTechLevel = playerScript.transform.GetComponent<TechnologyLevel>();

        SetUpTechScreen();
    }

    private void SetUpTechScreen()
    {
        TechnologyTree techTree = playerTechLevel.techTree;
        
        foreach (KeyValuePair<int, Technology> kvp in TechTree.techDictionary)
        {
            Vector2 screenPosition = TechTree.techPosition[kvp.Key];
            screenPosition.y *= -1.2f;
            InstantiateTechField(kvp.Value, screenPosition);
        }
    }

    private void InstantiateTechField(Technology tech, Vector2 screenPosition)
    {
        GameObject techField = Instantiate(techFieldPrefab, techFieldsParent);
        techField.GetComponent<TechField>().SetTechnology(tech);
        techField.transform.localPosition = screenPosition;
    }


    public TechnologyTree TechTree
    {
        get { return playerTechLevel.techTree; }
    }
    
    public void Subscribe(int techID, System.Action<int> callback)
    {
        playerTechLevel.techTree.Subscribe(techID, callback);
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
}
