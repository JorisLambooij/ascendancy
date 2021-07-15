using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class StoredResearchPointsUpdater : MonoBehaviour, PropertySubscriber<float>
{
    private Text textField;

    void Awake()
    {
        textField = GetComponent<Text>();
        GetComponentInParent<PlayerTechScreen>().playerTechLevel.storedResearch.Subscribe(this);
    }
    
    void OnEnable()
    {
        textField.text = GetComponentInParent<PlayerTechScreen>().playerTechLevel.storedResearch.Value.ToString();
    }

    void PropertySubscriber<float>.Callback(float updatedValue)
    {
        textField.text = updatedValue.ToString();
    }
}
