using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonPlus : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Action<object> ClickHandler;
    public Action<object> PressHandler;
    public Action<object> DownHandler;
    public Action<object> UpHandler;

    public float DownThreshold; //開始 Down 事件
    public float PressDuration; //Down 之後執行 Press 的週期

    private bool _isPointerDown = false;
    private bool _longPressTriggered = false;
    private float _startDownTime;
    private float _startPressTime;
    private object _data = null;

    public void SetData(object data)
    {
        _data = data;
    }

    private void Update()
    {
        if (_isPointerDown)
        {
            if (!_longPressTriggered)
            {
                if (Time.time - _startDownTime > DownThreshold)
                {
                    _longPressTriggered = true;
                    _startPressTime = Time.time;
                    if (DownHandler != null)
                    {
                        DownHandler(_data);
                    }
                    Debug.Log("Down");
                }
            }
            else if(Time.time -_startPressTime > PressDuration)
            {
                _startPressTime = Time.time;
                if (PressHandler != null)
                {
                    PressHandler(_data);
                }
                Debug.Log("Press");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _startDownTime = Time.time;
        _isPointerDown = true;
        _longPressTriggered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerDown = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_longPressTriggered)
        {
            if (ClickHandler != null)
            {
                ClickHandler(_data);
            }
            Debug.Log("Clcik");
        }
    }
}
