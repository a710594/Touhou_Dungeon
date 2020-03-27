using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StairsGroup : MonoBehaviour
{
    public Button CloseButton;
    public Button NextButton;
    public Text NextLabel;

    private int _floor;
    private Action _closeCallback;

    public void Open(int floor, Action closeCallback)
    {
        if (floor == 0)
        {
            NextLabel.text = "回村莊";
        }
        else
        {
            NextLabel.text = floor + "樓";
        }

        _floor = floor;
        _closeCallback = closeCallback;
        gameObject.SetActive(true);
    }

    private void NextOnClick()
    {
        if (_floor == 0)
        {
            ExploreController.Instance.BackToVilliage();
        }
        else
        {
            ExploreController.Instance.ChangeFloor(_floor);
        }
        gameObject.SetActive(false);
    }

    private void CloseOnClick()
    {
        if (_closeCallback != null)
        {
            _closeCallback();
        }

        gameObject.SetActive(false);
    }

    private void Awake()
    {
        gameObject.SetActive(false);

        NextButton.onClick.AddListener(NextOnClick);
        CloseButton.onClick.AddListener(CloseOnClick);
    }
}
