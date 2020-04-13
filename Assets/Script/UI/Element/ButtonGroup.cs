using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    public ButtonSingle[] Button;

    private void ButtonOnClick(ButtonSingle button)
    {
        for (int i=0; i<Button.Length; i++)
        {
            if (Button[i] == button)
            {
                Button[i].SetSelected(true);
            }
            else
            {
                Button[i].SetSelected(false);
            }
        }
    }

    private void Awake()
    {
        for (int i=0; i<Button.Length; i++)
        {
            Button[i].ClickHandler = ButtonOnClick;
        }
    }
}
