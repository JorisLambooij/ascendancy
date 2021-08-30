using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class StoredResearchPointsUpdater : MonoBehaviour
{
    private Text textField;

    void Start()
    {
        textField = GetComponent<Text>();
        GetComponentInParent<PlayerTechScreen>().playerTechLevel.storedResearchUpdate.AddListener(Callback);
        textField.text = GetComponentInParent<PlayerTechScreen>().playerTechLevel.storedResearch.ToString();
    }
    
    void OnEnable()
    {
        if (textField == null)
            textField = GetComponent<Text>();

        if (GetComponentInParent<PlayerTechScreen>().playerTechLevel == null)
            return;
        
        textField.text = GetComponentInParent<PlayerTechScreen>().playerTechLevel.storedResearch.ToString();
    }

    void Callback(float updatedValue)
    {
        textField.text = updatedValue.ToString();
    }
}
