using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldScript : MonoBehaviour
{
    public Button submitButton;
    public InputField inputField;

    public void CheckEndEdit()
    {
        if (Input.GetButtonDown("Submit"))
        {
            submitButton.GetComponent<ButtonTextSend>().OnClick();
            inputField.ActivateInputField();
        }
    }
}
