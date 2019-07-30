using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        switch (name)
        {
            case "Button_1":
                
                break;
            case "Button_2":

                break;
            case "Button_3":

                break;
            case "Button_4":

                break;
            case "Button_5":

                break;
            case "Button_6":

                break;
            case "Button_7":

                break;
            case "Button_8":

                break;
            default:
                Debug.LogError("No function for Button " + name + " in ContextMenuButtons-script");
                break;
        }


        GetComponentInParent<Canvas>().gameObject.SetActive(false);
    }
}
