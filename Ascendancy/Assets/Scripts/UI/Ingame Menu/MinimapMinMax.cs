using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapMinMax : MonoBehaviour
{
    // Start is called before the first frame update
    void ToggleMinimapSize()
    {
        GameObject minimapImage;

        void Start()
        {
            minimapImage = GameObject.Find("Image_Minimap");
        }

        void ToggleMinimapSize()
        {
            minimapImage.transform.position = new Vector3(100, 200, 100); //ToDo: Resize Properly, check toggle
        }
    }
}
