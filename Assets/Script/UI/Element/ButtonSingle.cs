using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSingle : MonoBehaviour
{
    public Action<ButtonSingle> ClickHandler;

    public GameObject Select;
    public Button Button;

    public void SetSelected(bool isSelected)
    {
        Select.SetActive(isSelected);
    }

    void OnClick()
    {
        if (ClickHandler != null)
        {
            ClickHandler(this);
        }
    }

    private void Awake()
    {
        Select.SetActive(false);
        Button.onClick.AddListener(OnClick);
    }
}
