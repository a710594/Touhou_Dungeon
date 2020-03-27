using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class InteractiveButton : MonoBehaviour
{
    public Button Button;
    public Action<object, InteractiveButton> OnClickHandler;

    protected object _data;

    public virtual void SetData(object obj)
    {
        _data = obj;
    }

    protected virtual void OnClick()
    {
        if (OnClickHandler != null)
        {
            OnClickHandler(_data, this);
        }
    }

    void Awake()
    {
        Button.onClick.AddListener(OnClick);
    }
}
