using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapMinMax : MonoBehaviour
{
    GameObject minimapImage;

    bool maxed = true;

    float imageWidth, imageHeight;

    // Start is called before the first frame update
    void Start()
    {
        minimapImage = GameObject.Find("Image_Minimap");    

        var minimapImageRectTransform = minimapImage.transform as RectTransform;

        imageWidth = minimapImageRectTransform.rect.width;
        imageHeight = minimapImageRectTransform.rect.height;
    }


    public void ToggleMinimapSize()
    {
        if (maxed) //minimap is already maxed
        {
            //scale: 1/2
            minimapImage.transform.localScale = (new Vector3(0.5f, 0.5f, 0.5f));

            //move to bottom left
            minimapImage.transform.localPosition = minimapImage.transform.localPosition + new Vector3(-imageWidth/4, -imageHeight/4, 0);

            //move button to bottom left
            transform.localPosition = transform.localPosition + new Vector3(-imageWidth / 2, -imageHeight / 2, 0);

            //change button text to plus
            GetComponentInChildren<Text>().text = "+";

            maxed = false;
        }
        else //minimap is not max size
        {
            //scale: 1
            minimapImage.transform.localScale = (new Vector3(1f, 1f, 1f));


            //move back to middle
            minimapImage.transform.localPosition = minimapImage.transform.localPosition + new Vector3(imageWidth / 4, imageHeight / 4, 0);

            //move button back to middle
            transform.localPosition = transform.localPosition + new Vector3(imageWidth / 2, imageHeight / 2, 0);

            //change button text to minus
            GetComponentInChildren<Text>().text = "-";

            maxed = true;
        }
    }
}
