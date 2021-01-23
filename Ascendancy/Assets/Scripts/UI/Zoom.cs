using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zoom : MonoBehaviour
{
    public float scrollSensitivity;

    public float initialZoom = 1;
    public int maxZoomLevels = 5;

    private int currentZoomLevel = 0;
    private float scrollBuffer;

    private void Start()
    {
        transform.localScale = new Vector3(initialZoom, initialZoom, 1);
    }

    private void Update()
    {
        Scroll();
    }

    public void Scroll()
    {
        float scrollAmount = Input.mouseScrollDelta.y;
        float oldZoom = currentZoomLevel;

        currentZoomLevel = Mathf.Clamp(Mathf.RoundToInt(currentZoomLevel + scrollAmount), -maxZoomLevels, maxZoomLevels);
        if (currentZoomLevel == oldZoom)
            return;

        float oldScale = transform.localScale.x;
        float newScale = initialZoom * Mathf.Pow(1 + scrollSensitivity, currentZoomLevel);

        Vector3 halfRes = new Vector3(Screen.width, Screen.height, 0) * 0.5f;
        Vector3 mousePosInScreenSpace = Vector3.zero;

        Vector3 transformPosInScreenSpace = (transform.position - halfRes) / oldScale;
        //transformPosInScreenSpace = transformPosInScreenSpace - (scrollAmount * scrollSensitivity) * (mousePosInScreenSpace - transformPosInScreenSpace); //a + (b - a) * t;
        transform.position = transformPosInScreenSpace * newScale + halfRes;
        transform.localScale = new Vector3(newScale, newScale, 1);

        //Debug.Log(transformPosInScreenSpace + " | " + mousePosInScreenSpace);
    }
}
