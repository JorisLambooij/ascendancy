using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class StoredResearchPointsUpdater : MonoBehaviour, PropertySubscriber<float>
{
    private Text textField;

    // Start is called before the first frame update
    void Start()
    {
        textField = GetComponent<Text>();

        Debug.Log(GetComponentInParent<PlayerTechScreen>().playerTechLevel);
        GetComponentInParent<PlayerTechScreen>().playerTechLevel.storedResearch.Subscribe(this);
    }
    
    void PropertySubscriber<float>.Callback(float updatedValue)
    {
        textField.text = updatedValue.ToString();
    }
}
