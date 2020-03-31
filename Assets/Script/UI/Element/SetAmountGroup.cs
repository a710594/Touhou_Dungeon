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

    private int _amount;
    private int _maxAmount;
    private Action<int> _onConfirmHandler;

    public void Open(int maxAmoount, string comment, Action<int> onConfirmHandler)
    {
        gameObject.SetActive(true);
        _maxAmount = maxAmoount;
        _onConfirmHandler = onConfirmHandler;
        _amount = 1;
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
    }

    private void CheckValueRange(int amount)
    {
        if (amount > _maxAmount)
        {
            amount = _maxAmount;
        }
        else if (amount < 1)
        {
            amount = 1;
        }
        InputField.text = amount.ToString();

        if (amount == _maxAmount)
        {
            AddButton.interactable = false;
        }
        else
        {
            AddButton.interactable = true;
        }

        if (amount == 1)
        {
            MinusButton.interactable = false;
        }
        else
        {
            MinusButton.interactable = true;
        }
    }

    private void OnValueChange(string text)
    {
        _amount = int.Parse(text);
        CheckValueRange(_amount);
    }

    private void AddOnClick()
    {
        _amount = int.Parse(InputField.text);
        _amount++;
        CheckValueRange(_amount);
    }

    private void MinusOnClick()
    {
        _amount = int.Parse(InputField.text);
        _amount--;
        CheckValueRange(_amount);
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
