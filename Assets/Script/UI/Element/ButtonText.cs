using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonText : MonoBehaviour
{
    public Action<string, object> OnClickHandler;

    public Text Label;
    public ButtonPlus Button;

    private string _text;
    private object _data;

    public void SetData(string text, object data)
    {
        _text = text;
        _data = data;
        Label.text = text;
    }

    private void OnClick(object data)
    {
        if (OnClickHandler != null)
        {
            OnClickHandler(_text, _data);
        }
    }

    private void Awake()
    {
        Button.ClickHandler = OnClick;
    }
}
