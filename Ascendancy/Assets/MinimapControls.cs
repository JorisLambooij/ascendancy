using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For moving and zooming on the Minimap.
/// </summary>
public class MinimapControls : MonoBehaviour
{
    public float scrollSensitivity;

    private bool mouseOver;
    private Camera minimapCam;
    private float zoomDelta;

    // Start is called before the first frame update
    private void Start()
    {
        mouseOver = false;
        minimapCam = GetComponent<Camera>();
        zoomDelta = 0;
    }

    /// <summary>
    /// EventHandler function, DO NOT TOUCH OR USE PLEASE
    /// </summary>
    /// <param name="pMouseOver">Event parameter.</param>
    public void SetMouseOver(bool pMouseOver)
    {
        mouseOver = pMouseOver;
    }

    private void Update()
    {
        float mouseScroll = Input.mouseScrollDelta.y;
        if (mouseOver && mouseScroll != 0)
        {
            zoomDelta += mouseScroll;
            float newZoom = minimapCam.orthographicSize;
            if (zoomDelta < 0)
            {
                newZoom *= 1 + scrollSensitivity;
                zoomDelta = Mathf.Min(0, zoomDelta + 1);
            }
            else
            {
                newZoom /= 1 + scrollSensitivity;
                zoomDelta = Mathf.Max(0, zoomDelta - 1);
            }
            minimapCam.orthographicSize = Mathf.Clamp(newZoom, 16, GetComponent<MinimapCamera>().StandardSize);
        }
    }
}
