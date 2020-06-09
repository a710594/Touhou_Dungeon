using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSingle : MonoBehaviour
{
    public enum TypeEnum
    {
        GameObject,
        Color,
    }

    public Action<ButtonSingle> ClickHandler;

    public TypeEnum Type = TypeEnum.GameObject;
    public GameObject Select;
    public Color SelectColor;
    public Color NotSelectColor;
    public Button Button;

    public void SetSelected(bool isSelected)
    {
        if (Type == TypeEnum.GameObject)
        {
            Select.SetActive(isSelected);
        }
        else if(Type == TypeEnum.Color)
        {
            if (isSelected)
            {
                Button.image.color = SelectColor;
            }
            else
            {
                Button.image.color = NotSelectColor;
            }
        }
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
        if (Select != null)
        {
            Select.SetActive(false);
        }
        Button.onClick.AddListener(OnClick);
    }
}
