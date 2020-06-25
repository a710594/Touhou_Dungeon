using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class IconCard : MonoBehaviour {

    public Action<IconCard> CardHandler;

    public Image Icon;
    public Text NameLabel;
    public Text AmountLabel;
    public ButtonPlus CardButton;

    public void Init(int id)
    {
        if (id >= 0)
        {
            ItemData.RootObject data = ItemData.GetData(id);
            Init(data);
        }
        else
        {
            SetMoney(id * -1);
        }
    }

    public void Init(int id, int amount)
    {
        if (id >= 0)
        {
            ItemData.RootObject data = ItemData.GetData(id);
            Init(data, amount);
        }
        else
        {
            SetMoney(id * -1);
        }
    }

    public void Init(int id, int have, int need)
    {
        if (id >= 0)
        {
            ItemData.RootObject data = ItemData.GetData(id);
            Init(data, have, need);
        }
        else
        {
            SetMoney(id * -1);
        }
    }

    private void Init(ItemData.RootObject data)
    {
        Icon.gameObject.SetActive(true);
        Icon.overrideSprite = Resources.Load<Sprite>("Image/Item/" + data.Icon);
        NameLabel.text = data.GetName();
        AmountLabel.gameObject.SetActive(false);
    }

    private  void Init(ItemData.RootObject data, int amount)
    {
        if (amount == 0)
        {
            Clear();
        }
        else
        {
            Icon.gameObject.SetActive(true);
            Icon.overrideSprite = Resources.Load<Sprite>("Image/Item/" + data.Icon);
            NameLabel.text = data.GetName();
            AmountLabel.gameObject.SetActive(true);
            AmountLabel.text = amount.ToString();
        }
    }

    public void Init(Equip equip)
    {
        Icon.gameObject.SetActive(true);
        Icon.overrideSprite = Resources.Load<Sprite>("Image/" + equip.Icon);
        NameLabel.text = equip.Name;
        AmountLabel.gameObject.SetActive(false);
        //AmountLabel.gameObject.SetActive(true);
        //AmountLabel.text = "Lv." + equip.Lv;
        //Amount = 0;
    }

    private void Init(ItemData.RootObject data, int have, int need)
    {
        Icon.gameObject.SetActive(true);
        Icon.overrideSprite = Resources.Load<Sprite>("Image/Item/" + data.Icon);
        NameLabel.text = data.GetName();
        AmountLabel.gameObject.SetActive(true);
        AmountLabel.text = have.ToString() + "/" + need.ToString();
    }

    private void SetMoney(int money)
    {
        Icon.gameObject.SetActive(true);
        Icon.overrideSprite = Resources.Load<Sprite>("Image/Item/Coin");
        NameLabel.text = LanguageData.GetText(12, LanguageSystem.Instance.CurrentLanguage);
        AmountLabel.gameObject.SetActive(true);
        AmountLabel.text = money.ToString();
    }

    public void Clear()
    {
        //Icon.overrideSprite = null;
        Icon.gameObject.SetActive(false);
        AmountLabel.gameObject.SetActive(false);
    }

    private void CardOnClick(object data)
    {
        if (CardHandler != null)
        {
            CardHandler(this);
        }
    }

    void Awake()
    {
        //AmountLabel.gameObject.SetActive(false);

        if (CardButton != null)
        {
            CardButton.ClickHandler = CardOnClick;
        }
    }
}
