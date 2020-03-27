using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipIcon : MonoBehaviour
{
    public static EquipIcon _lastSelect;

    public Action<Equip> OnClickHandler;

    public Image Icon;
    public Text Label;
    public GameObject SelectImage;
    public Button Button;

    private Equip _equip;

    public void SetData(Equip equip)
    {
        _equip = equip;
        Label.text = equip.Name;

        if (equip.Icon != null)
        {
            Icon.overrideSprite = Resources.Load<Sprite>("Image/" + equip.Icon);
            Icon.color = Color.white;
        }
        else
        {
            Icon.color = Color.clear;
        }
    }

    public void SetSelect(bool isSelected)
    {
        SelectImage.SetActive(isSelected);
    }

    private void OnClick()
    {
        if (OnClickHandler != null)
        {
            OnClickHandler(_equip);
        }

        if (_lastSelect != null)
        {
            _lastSelect.SelectImage.SetActive(false);
        }
        SelectImage.SetActive(true);
        _lastSelect = this;
    }

    private void Awake()
    {
        Button.onClick.AddListener(OnClick);
    }
}
