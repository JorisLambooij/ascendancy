using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechField : MonoBehaviour
{
    public Color colorIfNotResearchable;
    public Color colorIfResearchable;
    public Color colorIfResearched;

    public bool isCurrentFocus;

    private Text labelName;
    private Text labelCost;
    private Slider progressBar;
    private Image icon;
    
    public int technologyID;
    private Technology tech;
    public PlayerTechScreen playerTechScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        isCurrentFocus = false;

        playerTechScreen = GetComponentInParent<PlayerTechScreen>();
        tech = playerTechScreen.TechTree.techDictionary[technologyID];

        Text[] texts = GetComponentsInChildren<Text>();
        labelName = texts[0];
        labelCost = texts[1];

        progressBar = GetComponentInChildren<Slider>();
        icon = GetComponentInChildren<Image>();

        labelName.text = tech.name.ToString();
        labelCost.text = tech.cost.ToString();
        progressBar.value = tech.startTech ? 1 : 0;

        playerTechScreen.Subscribe(tech.id, OnProgressUpdate);

        foreach (int dependency in tech.dependencies)
            playerTechScreen.Subscribe(dependency, OnDependencyProgressUpdate);

        
        switch (playerTechScreen.TechTree.TechResearchability(tech.id))
        {
            case Researchability.NotResearchable:
                SetNotResearchable();
                break;
            case Researchability.Researchable:
                SetResearchable();
                break;
            case Researchability.Researched:
                SetResearched();
                break;
            default:
                Debug.LogError("Unknown Researchability. Please check");
                break;
        }
    }

    #region Color Management
    private void SetNotResearchable()
    {
        GetComponent<Button>().interactable = false;
        GetComponent<Image>().color = colorIfNotResearchable;
    }
    private void SetResearchable()
    {
        GetComponent<Button>().interactable = true;
        GetComponent<Image>().color = colorIfResearchable;
    }
    private void SetResearched()
    {
        GetComponent<Button>().interactable = false;
        GetComponent<Image>().color = colorIfResearched;
    }
    #endregion

    /// <summary>
    /// When a dependency of this Technology is updated, check whether this Technology has become researchable as a result.
    /// </summary>
    /// <param name="newProgress">New Progress of the dependency. Not relevant in this function.</param>
    private void OnDependencyProgressUpdate(int newProgress)
    {
        if (playerTechScreen.TechTree.TechResearchability(tech.id) == Researchability.Researchable)
            SetResearchable();
    }

    public void OnClick()
    {
        playerTechScreen.playerTechLevel.ResearchFocus(tech.id);


    }

    private void OnProgressUpdate(int newProgress)
    {
        progressBar.value = Mathf.Clamp01((float)newProgress / tech.cost);

        if (playerTechScreen.TechTree.TechResearchability(tech.id) == Researchability.Researched)
            SetResearched();
    }
}
