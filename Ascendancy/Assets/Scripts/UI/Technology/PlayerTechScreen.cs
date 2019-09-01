using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTechScreen : MonoBehaviour
{
    public int playerID;
    public TechnologyLevel playerTechLevel;

    void Start()
    {
        Player playerScript = GameObject.Find("Game Manager").GetComponent<GameManager>().playerScript;
        playerTechLevel = playerScript.transform.GetComponent<TechnologyLevel>();
    }

    public TechnologyTree TechTree
    {
        get { return playerTechLevel.techTree; }
    }
    
    public void Subscribe(int techID, System.Action<int> callback)
    {
        playerTechLevel.techTree.Subscribe(techID, callback);
    }
}
