using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconConfirmUI : MonoBehaviour
{
    public IconCard[] Icon;
    public ButtonPlus ConfirmButton;

    private Action _onFinishHandler; 

    private static IconConfirmUI _instance;

    public static void Open(List<int> idList, Action callback = null)
    {
        if (_instance == null)
        {
            _instance = ResourceManager.Instance.Spawn("IconConfirmUI", ResourceManager.Type.UI).GetComponent<IconConfirmUI>();
        }
        _instance._onFinishHandler = callback;
        _instance.Init(idList);
    }

    private void Close()
    {
        Destroy(_instance.gameObject);
        _instance = null;
    }

    private void Init(List<int> idList)
    {
        for (int i=0; i<Icon.Length; i++)
        {
            if (i < idList.Count)
            {
                Icon[i].Init(idList[i]);
            }
            else
            {
                Icon[i].Clear();
            }
        }
    }

    private void ConfirmOnClick(object data)
    {
        _instance.Close();
        if (_onFinishHandler != null)
        {
            _onFinishHandler();
            _onFinishHandler = null;
        }
    }

    void Awake()
    {
        ConfirmButton.ClickHandler = ConfirmOnClick;
    }
}
