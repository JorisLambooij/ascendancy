using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //purging transparent areas of buttons
        Image[] images = transform.GetComponentsInChildren<Image>();

        foreach (Image img in images)
        {
            img.alphaHitTestMinimumThreshold = 0.5f;
        }
        Debug.Log("fixed it!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
