using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ResearchProductionUpdater : MonoBehaviour
{
    private Text textField;
    private const float updateFrequency = 0.5f;

    private IEnumerator UpdateRoutine()
    {
        textField = GetComponent<Text>();
        while (enabled)
        {
            float production = GetComponentInParent<PlayerTechScreen>().playerTechLevel?.averageResearchProduction.Calculate() ?? -1;
            //Debug.Log(production);
            textField.text = production.ToString();

            yield return new WaitForSeconds(updateFrequency);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateRoutine());
    }
}
