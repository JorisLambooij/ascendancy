using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zoom : MonoBehaviour
{
    public CanvasScaler zoomTarget;
    public float scrollSensitivity;

    private float scrollBuffer;

    private void Update()
    {
        if (scrollBuffer > 0)
        {
            zoomTarget.scaleFactor *= 1 + scrollSensitivity;
            scrollBuffer = Mathf.Max(0, scrollBuffer - 1);
        }
        else if (scrollBuffer < 0)
        {
            zoomTarget.scaleFactor /= 1 + scrollSensitivity;
            scrollBuffer = Mathf.Min(0, scrollBuffer + 1);
        }
    }

    public void OnScroll()
    {
        float scrollAmount = Input.mouseScrollDelta.y;
        scrollBuffer += scrollAmount;
    }
}
