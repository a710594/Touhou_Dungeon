using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAmountGroup : MonoBehaviour
{
    public InputField InputField;
    public Button AddButton;
    public Button MinusButton;
    public Button ConfirmButton;
    public Button CancelButton;
    public Text CommentLabel;
    public Text PriceLabel;

    private int _amount;
    private int _maxAmount;
    private int _price;
    private Action<int> _onConfirmHandler;

    public void Open(int maxAmoount, string comment, Action<int> onConfirmHandler, int price = 0)
    {
        gameObject.SetActive(true);
        _amount = 1;
        _maxAmount = maxAmoount;
        _price = price;
        _onConfirmHandler = onConfirmHandler;
        InputField.text = _amount.ToString();
        CommentLabel.text = comment;

        MinusButton.interactable = false;
        if (maxAmoount == 1)
        {
            AddButton.interactable = false;
        }
        else
        {
            AddButton.interactable = true;
        }

        if (PriceLabel != null)
        {
            PriceLabel.text = (_price * _amount).ToString();
        }
    }

    private void CheckValueRange()
    {
        if (_amount > _maxAmount)
        {
            _amount = _maxAmount;
        }
        else if (_amount < 1)
        {
            _amount = 1;
        }
        InputField.text = _amount.ToString();

        if (_amount == _maxAmount)
        {
            AddButton.interactable = false;
        }
        else
        {
            AddButton.interactable = true;
        }

        if (_amount == 1)
        {
            MinusButton.interactable = false;
        }
        else
        {
            MinusButton.interactable = true;
        }

        if (PriceLabel != null)
        {
            PriceLabel.text = (_price * _amount).ToString();
        }
    }

    private void OnValueChange(string text)
    {
        _amount = int.Parse(text);
        CheckValueRange();
    }

    private void AddOnClick()
    {
        _amount = int.Parse(InputField.text);
        _amount++;
        CheckValueRange();
    }

    private void MinusOnClick()
    {
        _amount = int.Parse(InputField.text);
        _amount--;
        CheckValueRange();
    }

    private void ConfirmOnClick()
    {
        gameObject.SetActive(false);
        if (_onConfirmHandler != null)
        {
            _onConfirmHandler(_amount);
            _onConfirmHandler = null;
        }
    }

    private void CancelOnClick()
    {
        gameObject.SetActive(false);
    }

    void Awake()
    {
        InputField.text = "1";

        InputField.onValueChanged.AddListener(OnValueChange);
        AddButton.onClick.AddListener(AddOnClick);
        MinusButton.onClick.AddListener(MinusOnClick);
        ConfirmButton.onClick.AddListener(ConfirmOnClick);
        CancelButton.onClick.AddListener(CancelOnClick);
    }
}
