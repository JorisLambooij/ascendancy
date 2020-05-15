using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Screen : MonoBehaviour
{
    /// <summary>
    /// Toggle the state of this ui element.
    /// </summary>
    public void Toggle()
    {
        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);
    }

    public void SetStatus(bool on)
    {
        gameObject.SetActive(on);
    }

    public bool GetStatus
    {
        get => gameObject.activeSelf;
    }
}
