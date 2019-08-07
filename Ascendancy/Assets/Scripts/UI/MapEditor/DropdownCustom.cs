using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownCustom : MonoBehaviour
{
    Button[] buttons;
    private int mode = 1;
    Image bgImage;
    bool open = false;
    public bool imageSelector;

    public DropdownCustom(bool imageSelector)
    {
        this.imageSelector = imageSelector;
    }


    void Start()
    {

        foreach(Image i in GetComponentsInChildren<Image>())
            if (i.name == "Image_BG")
                bgImage = i;

        if (bgImage == null)
            Debug.LogError("Could not find 'Image_BG' for DropdownCustom '" + this.name + "'");

        buttons = GetComponentsInChildren<Button>(true);

        for (int i = 1; i < buttons.Length; i++) //start at 1 so buttons[0] stays enabled
        {
            SetButton(buttons[i], false);
        }

        ChangeMode(1);
    }

    private void ChangeMode(int newMode)
    {
        if (imageSelector)
            bgImage.sprite = buttons[newMode].spriteState.pressedSprite;

        mode = newMode;
    }

    private void SetButton(Button b, bool on)
{
    b.gameObject.SetActive(on);
    //b.enabled = on;
    //b.image.gameObject.SetActive(on);
}

    public void OnButtonClick(int buttonID)
    {
        if (!open)
        {
            foreach (Button b in buttons)
                SetButton(b, true);

            open = true;
        }
        else
        {
            foreach (Button b in buttons)
                SetButton(b, false);

            SetButton(buttons[0], true);

            open = false;
        }

        if (buttonID == 0)
        {

        }
        else
        {
            ChangeMode(buttonID);
        }

        Debug.Log("CLICK BUTTON " + buttonID);
    }
}


