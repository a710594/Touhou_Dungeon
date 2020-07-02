using System.Collections;
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
    public ButtonPlus SellButton;
    public LoopScrollView ScrollView;
    public SetAmountGroup SetAmountGroup;
    public EquipComment EquipComment;

    private object _selectedItem = null;

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
        SetScrollView(ItemData.TypeEnum.All);
    }

    private void SetData()
    {
        ItemData.RootObject data;
        if (_selectedItem is int)
        {
            data = ItemData.GetData((int)_selectedItem);
        }
        else
        {
            data = ItemData.GetData(((Equip)_selectedItem).ID);
        }

        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        NameLabel.text = data.GetName();
        AmountLabel.text = "庫存：" + ItemManager.Instance.GetItemAmount(data.ID, ItemManager.Type.Warehouse);
        CommentLabel.text = data.GetComment();
        if (data.Type == ItemData.TypeEnum.Equip)
        {
            EquipComment.gameObject.SetActive(true);
            EquipComment.SetData(data.ID);
        }
        else
        {
            EquipComment.gameObject.SetActive(false);
        }
        SellButton.gameObject.SetActive(true);
    }

    private void SetScrollView(ItemData.TypeEnum type)
    {
        ItemData.RootObject data;
        Equip equip;
        Dictionary<object, int> managerDic = ItemManager.Instance.GetItemDicByType(ItemManager.Type.Warehouse, type);
        Dictionary<object, int> itemDic = new Dictionary<object, int>(); //id or Equip, price

        managerDic.Remove(0); //移除緊急逃脫裝置,那是非賣品
        foreach (KeyValuePair<object, int> item in managerDic)
        {
            if (item.Key is int)
            {
                data = ItemData.GetData((int)item.Key);
                itemDic.Add(data.ID, data.Price / 2);
            }
            else if (item.Key is Equip)
            {
                equip = (Equip)item.Key;
                itemDic.Add(equip, equip.Price / 2);
            }
        }
        ScrollView.SetData(new ArrayList(itemDic));
        ScrollView.AddClickHandler(MenuOnClick);
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
        _selectedItem = obj;
        SetData();
    }

    private void SellOnClick(object obj)
    {
        int id;
        int sellPrice;
        int maxAmount;

        if (_selectedItem is int)
        {
            id = (int)_selectedItem;
            sellPrice = ItemData.GetData(id).Price;
            maxAmount = ItemManager.Instance.GetItemAmount(id, ItemManager.Type.Warehouse);
        }
        else
        {
            Equip equip = (Equip)_selectedItem;
            id = equip.ID;
            sellPrice = equip.Price / 2;
            maxAmount = 1;
        }

        SetAmountGroup.Open(maxAmount, "要賣幾個？", (amount) =>
        {
            ItemManager.Instance.AddMoney(sellPrice * amount);
            ItemManager.Instance.MinusItem(_selectedItem, amount, ItemManager.Type.Warehouse);

            SetData();
            SetScrollView(ItemData.TypeEnum.All);
        }, sellPrice);
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
    }
}
