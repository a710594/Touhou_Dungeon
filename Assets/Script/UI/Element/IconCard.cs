using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class IconCard : MonoBehaviour {

    public Action<IconCard> CardHandler;

    public Image Icon;
    public Text AmountLabel;
    public ButtonPlus CardButton;
    public int ID;
    public int Amount;

    public void Init(int id)
    {
        ItemData.RootObject data = ItemData.GetData(id);
        Init(data);
    }

    public void Init(int id, int amount)
    {
        ItemData.RootObject data = ItemData.GetData(id);
        Init(data, amount);
    }

    public void Init(int id, int have, int need)
    {
        ItemData.RootObject data = ItemData.GetData(id);
        Init(data, have, need);
    }

    private void Init(ItemData.RootObject data)
    {
        Icon.gameObject.SetActive(true);
        Icon.overrideSprite = Resources.Load<Sprite>("Image/Item/" + data.Icon);
        AmountLabel.gameObject.SetActive(false);
        ID = data.ID;
        Amount = 0;
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
            AmountLabel.gameObject.SetActive(true);
            AmountLabel.text = amount.ToString();
            ID = data.ID;
            Amount = amount;
        }
    }

    public void Init(Equip equip)
    {
        Icon.gameObject.SetActive(true);
        Icon.overrideSprite = Resources.Load<Sprite>("Image/" + equip.Icon);
        AmountLabel.gameObject.SetActive(false);
        //AmountLabel.gameObject.SetActive(true);
        //AmountLabel.text = "Lv." + equip.Lv;
        //Amount = 0;
    }

    private void Init(ItemData.RootObject data, int have, int need)
    {
        Icon.gameObject.SetActive(true);
        Icon.overrideSprite = Resources.Load<Sprite>("Image/Item/" + data.Icon);
        AmountLabel.gameObject.SetActive(true);
        AmountLabel.text = have.ToString() + "/" + need.ToString();
        ID = data.ID;
        Amount = 0;
    }

    public void Clear()
    {
        //Icon.overrideSprite = null;
        Icon.gameObject.SetActive(false);
        AmountLabel.gameObject.SetActive(false);
        ID = 0;
        Amount = 0;
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
