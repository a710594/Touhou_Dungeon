using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    public Text MoneyLabel;
    public Text NameLabel;
    public Text AmountLabel;
    public Text CommentLabel;
    public Button ItemButton;
    public Button EquipButton;
    public Button CloseButton;
    public ButtonPlus BuyButton;
    public TipLabel TipLabel;
    public LoopScrollView ScrollView;
    public SetAmountGroup SetAmountGroup;
    public EquipComment EquipComment;
    public Text[] MaterialNameLabel;
    public Text[] MaterialAmountLabel;

    private bool _canBuy = false;
    private string _tipText;
    private ItemData.RootObject _selectedData = null;

    public static void Open()
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("ShopUI", ResourceManager.Type.UI).GetComponent<ShopUI>();
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
        SetScrollView(ShopData.TypeEnum.Item);
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
        BuyButton.gameObject.SetActive(true);

        _canBuy = true;

        if (ItemManager.Instance.Money < _selectedData.Price)
        {
            BuyButton.SetColor(Color.grey);
            _canBuy = false;
            _tipText = "錢不足";
        }

        int have;
        int need;
        for (int i = 0; i < MaterialNameLabel.Length; i++)
        {
            if (i < _selectedData.MaterialList.Count)
            {
                have = ItemManager.Instance.GetItemAmount(_selectedData.MaterialList[i], ItemManager.Type.Warehouse);
                need = _selectedData.AmountList[i];
                MaterialNameLabel[i].gameObject.SetActive(true);
                MaterialNameLabel[i].text = ItemData.GetData(_selectedData.MaterialList[i]).GetName();
                MaterialAmountLabel[i].gameObject.SetActive(true);

                if (have < need)
                {
                    BuyButton.SetColor(Color.grey);
                    _canBuy = false;
                    _tipText = "素材不足";
                    MaterialAmountLabel[i].text = "<color=#FF0000>" + have.ToString() + "</color>/" + need.ToString();
                }
                else
                {
                    MaterialAmountLabel[i].text = have.ToString() + "/" + need.ToString();
                }
            }
            else
            {
                MaterialNameLabel[i].gameObject.SetActive(false);
                MaterialAmountLabel[i].gameObject.SetActive(false);
            }
        }

        if (_canBuy)
        {
            BuyButton.SetColor(Color.white);
        }
    }

    private void SetScrollView(ShopData.TypeEnum type)
    {
        ItemData.RootObject data;
        List<int> idList = ShopData.GetIDList(type);
        Dictionary<int, int> itemDic = new Dictionary<int, int>(); //id, price
        for (int i = 0; i < idList.Count; i++)
        {
            data = ItemData.GetData(idList[i]);
            itemDic.Add(data.ID, data.Price);
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
        BuyButton.gameObject.SetActive(false);

        for (int i = 0; i < MaterialNameLabel.Length; i++)
        {
            MaterialNameLabel[i].gameObject.SetActive(false);
            MaterialAmountLabel[i].gameObject.SetActive(false);
        }
    }

    private void MenuOnClick(object obj)
    {
        _selectedData = ItemData.GetData((int)obj);
        SetData();
    }

    private void BuyOnClick(object obj)
    {
        if (_canBuy)
        {
            int maxAmount = ItemManager.Instance.Money / _selectedData.Price;
            int have;
            int need;
            for (int i=0; i<_selectedData.MaterialList.Count; i++)
            {
                have = ItemManager.Instance.GetItemAmount(_selectedData.MaterialList[i], ItemManager.Type.Warehouse);
                need = _selectedData.AmountList[i];
                if (have / need < maxAmount)
                {
                    maxAmount = have / need;
                }
            }

            SetAmountGroup.Open(maxAmount, "要買幾個？", (amount) =>
            {
                ItemManager.Instance.MinusMoney(_selectedData.Price * amount);
                for (int i = 0; i < _selectedData.MaterialList.Count; i++)
                {
                    ItemManager.Instance.MinusItem(_selectedData.MaterialList[i], _selectedData.AmountList[i] * amount, ItemManager.Type.Warehouse);
                }
                ItemManager.Instance.AddItem(_selectedData.ID, amount, ItemManager.Type.Warehouse);

                SetData();
            }, _selectedData.Price);
        }
        else
        {
            TipLabel.SetLabel(_tipText);
        }
    }

    private void ItemOnClick()
    {
        SetScrollView(ShopData.TypeEnum.Item);
        ClearInfo();
    }

    private void EquipOnClick()
    {
        SetScrollView(ShopData.TypeEnum.Equip);
        ClearInfo();
    }

    void Awake()
    {
        ClearInfo();

        EquipComment.gameObject.SetActive(false);
        BuyButton.gameObject.SetActive(false);
        BuyButton.ClickHandler = BuyOnClick;
        ItemButton.onClick.AddListener(ItemOnClick);
        EquipButton.onClick.AddListener(EquipOnClick);
        CloseButton.onClick.AddListener(Close);
    }
}
