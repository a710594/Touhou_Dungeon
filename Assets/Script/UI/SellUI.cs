﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellUI : MonoBehaviour
{
    public static SellUI Instance;

    public Text MoneyLabel;
    public Text NameLabel;
    public Text AmountLabel;
    public Text CommentLabel;
    public Button CloseButton;
    public Button AllButton;
    public Button MaterialButton;
    public Button FoodButton;
    public Button MedicineButton;
    public Button EquipButton;
    public ButtonPlus SellButton;
    public LoopScrollView ScrollView;
    public SetAmountGroup SetAmountGroup;
    public EquipComment EquipComment;

    private Item _selectedItem = null;
    private ItemData.TypeEnum _itemType = ItemData.TypeEnum.All;

    public static void Open()
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("SellUI", ResourceManager.Type.UI).GetComponent<SellUI>();
        }
        Instance.Init();
    }

    public void Close()
    {
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Init()
    {
        _selectedItem = null;
        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        SetScrollView(ItemData.TypeEnum.All, false);
    }

    private void SetData()
    {
        ItemData.RootObject data;
        data = ItemData.GetData(_selectedItem.ID);

        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        NameLabel.text = data.GetName();
        AmountLabel.text = "庫存：" + ItemManager.Instance.GetItemAmount(data.ID, ItemManager.Type.Warehouse);
        CommentLabel.text = data.GetComment();
        if (_selectedItem is Equip)
        {
            EquipComment.gameObject.SetActive(true);
            EquipComment.SetData((Equip)_selectedItem);
        }
        else
        {
            EquipComment.gameObject.SetActive(false);
        }
        SellButton.gameObject.SetActive(true);
    }

    private void SetScrollView(ItemData.TypeEnum type, bool isRefresh)
    {
        ItemData.RootObject data;
        List<Item> itemList = ItemManager.Instance.GetItemListByType(ItemManager.Type.Warehouse, type);
        List<KeyValuePair<Item, int>> sellDic = new List<KeyValuePair<Item, int>>(); //Item, price

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].ID == 0)
            {
                itemList.RemoveAt(i); //移除緊急逃脫裝置,那是非賣品
                i--;
            }
        }

        foreach (Item item in itemList)
        {
            data = ItemData.GetData(item.ID);
            sellDic.Add(new KeyValuePair<Item, int> (item, data.Price / 2));
        }

        if (isRefresh)
        {
            ScrollView.Refresh(new ArrayList(sellDic));
        }
        else
        {
            ScrollView.SetData(new ArrayList(sellDic));
        }
    }

    private void ClearInfo()
    {
        NameLabel.text = "";
        CommentLabel.text = "";
        AmountLabel.text = "";
        EquipComment.gameObject.SetActive(false);
        SellButton.gameObject.SetActive(false);
    }

    private void MenuOnClick(object obj)
    {
        _selectedItem = (Item)obj;
        SetData();
    }

    private void SellOnClick(object obj)
    {
        int sellPrice;
        int maxAmount;

        sellPrice = ItemData.GetData(_selectedItem.ID).Price / 2;
        maxAmount = ItemManager.Instance.GetItemAmount(_selectedItem.ID, ItemManager.Type.Warehouse);

        SetAmountGroup.Open(maxAmount, "要賣幾個？", (amount) =>
        {
            ItemManager.Instance.AddMoney(sellPrice * amount);
            ItemManager.Instance.MinusItem(_selectedItem, amount, ItemManager.Type.Warehouse);

            SetData();
            SetScrollView(_itemType, true);
        }, sellPrice);
    }

    private void AllOnClick()
    {
        SetScrollView(ItemData.TypeEnum.All, false);
        ClearInfo();
        _itemType = ItemData.TypeEnum.All;
    }

    private void MaterialOnClick()
    {
        SetScrollView(ItemData.TypeEnum.Material, false);
        ClearInfo();
        _itemType = ItemData.TypeEnum.Material;
    }

    private void FoodOnClick()
    {
        SetScrollView(ItemData.TypeEnum.Food, false);
        ClearInfo();
        _itemType = ItemData.TypeEnum.Food;
    }

    private void MedicineOnClick()
    {
        SetScrollView(ItemData.TypeEnum.Medicine, false);
        ClearInfo();
        _itemType = ItemData.TypeEnum.Medicine;
    }

    private void EquipOnClick()
    {
        SetScrollView(ItemData.TypeEnum.Equip, false);
        ClearInfo();
        _itemType = ItemData.TypeEnum.Equip;
    }

    void Awake()
    {
        NameLabel.text = string.Empty;
        AmountLabel.text = string.Empty;
        CommentLabel.text = string.Empty;

        EquipComment.gameObject.SetActive(false);
        SellButton.gameObject.SetActive(false);
        SellButton.ClickHandler = SellOnClick;
        CloseButton.onClick.AddListener(Close);
        AllButton.onClick.AddListener(AllOnClick);
        MaterialButton.onClick.AddListener(MaterialOnClick);
        FoodButton.onClick.AddListener(FoodOnClick);
        MedicineButton.onClick.AddListener(MedicineOnClick);
        EquipButton.onClick.AddListener(EquipOnClick);
        ScrollView.AddClickHandler(MenuOnClick);
    }
}
