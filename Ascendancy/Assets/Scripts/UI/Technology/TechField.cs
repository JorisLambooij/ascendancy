using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechField : MonoBehaviour
{
    public PlayerTechScreen playerTechScreen;
    public Transform inPoint;
    public Transform outPoint;
    public bool isCurrentFocus = false;

    private Text labelName;
    private Text labelCost;
    private Slider progressBar;
    private Image icon;
    private Image backGround;
    
    private Technology tech;

    public Technology Tech { get => tech; private set => tech = value; }

    public void SetTechnology(Technology tech)
    {
        this.tech = tech;
        playerTechScreen = transform.parent.GetComponentInParent<PlayerTechScreen>();

        Text[] texts = GetComponentsInChildren<Text>();
        labelName = texts[0];
        labelCost = texts[1];

        progressBar = GetComponentInChildren<Slider>();
        Image[] images = GetComponentsInChildren<Image>();
        backGround = images[0];
        icon = images[1];

        labelName.text = tech.name.ToString();
        labelCost.text = tech.cost.ToString();
        progressBar.value = tech.startTech ? 1 : 0;
        icon.sprite = tech.icon;

        playerTechScreen.Subscribe(tech.id, OnProgressUpdate);

        foreach (int dependency in tech.dependencies)
            playerTechScreen.Subscribe(dependency, OnDependencyProgressUpdate);
        
        SetRightColor();
    }

    #region Color Management
    public void SetRightColor()
    {
        switch (playerTechScreen.TechTree.TechResearchability(Tech.id))
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

    private void SetNotResearchable()
    {
        Button button = GetComponent<Button>();
        button.interactable = false;
        ColorBlock colors = button.colors;
        colors.disabledColor = playerTechScreen.colorIfNotResearchable;
        button.colors = colors;
        
        //icon.color = playerTechScreen.colorIfNotResearchable;
    }
    private void SetResearchable()
    {
        Button button = GetComponent<Button>();
        button.interactable = true;

        if (isCurrentFocus)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = playerTechScreen.colorIfResearching;
            button.colors = colors;

            //icon.color = playerTechScreen.colorIfResearching;
        }
        else
        {
            ColorBlock colors = button.colors;
            colors.normalColor = playerTechScreen.colorIfResearchable;
            button.colors = colors;

            //icon.color = playerTechScreen.colorIfResearchable;
        }
    }
    private void SetResearched()
    {
        Button button = GetComponent<Button>();
        button.interactable = false;
        ColorBlock colors = button.colors;
        colors.disabledColor = playerTechScreen.colorIfResearched;
        button.colors = colors;

        //icon.color = playerTechScreen.colorIfResearched;
    }
    #endregion

    /// <summary>
    /// When a dependency of this Technology is updated, check whether this Technology has become researchable as a result.
    /// </summary>
    /// <param name="newProgress">New Progress of the dependency. Not relevant in this function.</param>
    private void OnDependencyProgressUpdate(int newProgress)
    {
        if (playerTechScreen.TechTree.TechResearchability(Tech.id) == Researchability.Researchable)
            SetResearchable();
    }

    public void OnClick()
    {
        playerTechScreen.playerTechLevel.ResearchFocus(Tech.id);

        playerTechScreen.Focus(Tech.id);
    }

    private void OnProgressUpdate(int newProgress)
    {
        progressBar.value = Mathf.Clamp01((float)newProgress / Tech.cost);

        if (playerTechScreen.TechTree.TechResearchability(Tech.id) == Researchability.Researched)
            SetResearched();
    }
}
