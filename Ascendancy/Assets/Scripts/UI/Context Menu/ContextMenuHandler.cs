using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuHandler : MonoBehaviour
{

    private Vector3[] conMenuButtonPos;


    // Start is called before the first frame update
    void Start()
    {
        
        conMenuButtonPos = new Vector3[8];
        int i = 0;
        foreach (Button g in GetComponentsInChildren<Button>())
        {
            g.image.color = new Color(255f, 255f, 255f, .2f);
            g.image.alphaHitTestMinimumThreshold = 0.5f;


            //conMenuButtonPos[i] = g.transform.position - new Vector3(GetComponent<Canvas>().pixelRect.width, GetComponent<Canvas>().pixelRect.height) / 2;
            conMenuButtonPos[i] = g.transform.position - new Vector3(GetComponentInParent<Canvas>().pixelRect.width, GetComponentInParent<Canvas>().pixelRect.height) / 2;
            i++;
        }

        GetComponentInChildren<Image>().color = new Color(255f, 255f, 255f, .1f);

        //gameObject.SetActive(false);
    }

    public void Show(int numberOfButtons)
    {
        //first, move the context menu to cursor position
        GetComponentInChildren<Image>().transform.position = Input.mousePosition;

        //move all the buttons
        //deactivate some
        int i = 0;
        foreach (Component g in GetComponentsInChildren<Button>())
        {
            g.transform.position = conMenuButtonPos[i] + Input.mousePosition;

            if (numberOfButtons <= i)
                g.gameObject.SetActive(false);
            else
                g.gameObject.SetActive(true);
            i++;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }


}
