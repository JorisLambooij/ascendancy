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

    void OnMouseOver()
    {
        Debug.Log("yeayy");
    }

    /// <summary>
    /// EventHandler function, DO NOT TOUCH OR USE PLEASE
    /// </summary>
    /// <param name="pMouseOver">Event parameter.</param>
    public void SetMouseOver(bool pMouseOver)
    {
        mouseOver = pMouseOver;
    }

    public void GoToClick()
    {
        Vector2 mousePos = Input.mousePosition;
        RectTransform rt = transform as RectTransform;
        CameraScript cam = CameraScript.instance;

        Vector2 localPoint;
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out localPoint);

        localPoint = localPoint / rt.rect.size + Vector2.one * 0.5f;
        Vector2 worldPos = localPoint * World.Instance.worldSize * World.Instance.tileSize;

        Vector3 cameraPos = new Vector3(worldPos.x, 0, worldPos.y);
        cameraPos.y = World.Instance.GetTile(cameraPos)?.height ?? 0;
        cameraPos.y += cam.targetHeight;
        cameraPos.y *= 0.5f;

        cam.MoveCam(cameraPos, false);
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
            //minimapCam.orthographicSize = Mathf.Clamp(newZoom, 16, GetComponent<Camera>().StandardSize);
        }
    }
}
