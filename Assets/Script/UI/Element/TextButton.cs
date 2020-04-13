using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextButton : MonoBehaviour
{
    public Action<object> OnClickHandler;

    public Text Label;
    public Image Image;
    public Button Button;

    private string _text;
    private object _data;

    public void SetData(string text, object data)
    {
        _text = text;
        _data = data;
        Label.text = text;
    }

    public void SetColor(Color color)
    {
        Image.color = color;
    }

    private void OnClick()
    {
        if (OnClickHandler != null)
        {
            OnClickHandler(_data);
        }
    }

    private void Awake()
    {
        Button.onClick.AddListener(OnClick);
    }
}
