﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuHandler : MonoBehaviour
{

    private Vector3[] conMenuButtonPos;
    Image centerImage;
    Button[] buttons;


    // Start is called before the first frame update
    void Start()
    {
        conMenuButtonPos = new Vector3[8];

        centerImage = GetComponentInChildren<Image>();

        buttons = GetComponentsInChildren<Button>(true);

        for (int i = 0; i < buttons.Length; i++)
        {

            buttons[i].image.color = new Color(255f, 255f, 255f, .2f);
            buttons[i].image.alphaHitTestMinimumThreshold = 0.5f;

            conMenuButtonPos[i] = buttons[i].transform.position - new Vector3(GetComponentInParent<Canvas>().pixelRect.width, GetComponentInParent<Canvas>().pixelRect.height) / 2;
        }

        centerImage.color = new Color(255f, 255f, 255f, .1f);



        Hide();
    }

    public void Show(int numberOfButtons)
    {

        //Debug.Log("Show(" + numberOfButtons + ")");

        //first, move the context menu to cursor position
       

        gameObject.SetActive(true);

        centerImage.transform.position = Input.mousePosition;

        //move all the buttons
        //deactivate some
        buttons = GetComponentsInChildren<Button>(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            {
                buttons[i].transform.position = conMenuButtonPos[i] + Input.mousePosition;

                if (numberOfButtons <= i)
                {
                    buttons[i].gameObject.SetActive(false);
                    //Debug.Log("numberofbuttons["+ numberOfButtons + "] <= i[" + i + "] deactivate:" + i);
                }
                else
                {
                    buttons[i].gameObject.SetActive(true);
                    //Debug.Log("activate:" + i);
                }
            }
        }

            //gameObject.SetActive(true);
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