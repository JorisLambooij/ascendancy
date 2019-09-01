using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologyLevel : MonoBehaviour
{
    public TechnologyTree techTree { get; private set; }
    public int currentFocus;

    // Start is called before the first frame update
    void Awake()
    {
        techTree = TechTreeReader.LoadTechTree();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            techTree.AddProgress(currentFocus, 100);
        }
    }

    public void ResearchFocus(int techID)
    {
        if (techTree.TechResearchability(techID) == Researchability.Researchable)
            currentFocus = techID;
        else
            Debug.Log("Technology not researchable");
    }
    
}
