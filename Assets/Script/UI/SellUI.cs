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

    private ItemData.RootObject _selectedData = null;

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
        _selectedData = null;
        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        SetScrollView(ItemData.TypeEnum.All);
    }

    private void SetData()
    {
        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        NameLabel.text = _selectedData.GetName();
        AmountLabel.text = "庫存：" + ItemManager.Instance.GetItemAmount(_selectedData.ID, ItemManager.Type.Warehouse);
        CommentLabel.text = _selectedData.GetComment();
        if (_selectedData.Type == ItemData.TypeEnum.Equip)
        {
            EquipComment.gameObject.SetActive(true);
            EquipComment.SetData(_selectedData.ID);
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
        Dictionary<int, int> itemDic = new Dictionary<int, int>(); //id, price

        managerDic.Remove(0); //移除緊急逃脫裝置,那是非賣品
        foreach (KeyValuePair<object, int> item in managerDic)
        {
            if (item.Key is int)
            {
                data = ItemData.GetData((int)item.Key);
                itemDic.Add(data.ID, data.Price);
            }
            else if (item.Key is Equip)
            {
                equip = (Equip)item.Key;
                itemDic.Add(equip.ID, equip.Price);
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
        _selectedData = ItemData.GetData((int)obj);
        SetData();
    }

    private void SellOnClick(object obj)
    {
        int sellPrice = _selectedData.Price / 2;
        int maxAmount = ItemManager.Instance.GetItemAmount(_selectedData.ID, ItemManager.Type.Warehouse);

        SetAmountGroup.Open(maxAmount, "要買幾個？", (amount) =>
        {
            ItemManager.Instance.AddMoney(sellPrice * amount);
            ItemManager.Instance.MinusItem(_selectedData.ID, amount, ItemManager.Type.Warehouse);

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
