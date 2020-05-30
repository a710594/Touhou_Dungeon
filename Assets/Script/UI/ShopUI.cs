using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    public Text MoneyLabel;
    public Text NameLabel;
    public Text VolumeLabel;
    public Text CommentLabel;
    public Button CloseButton;
    public ButtonPlus BuyButton;
    public TipLabel TipLabel;
    public LoopScrollView ScrollView;

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

        ItemData.RootObject data;
        List<int> idList = ShopData.GetIDList();
        Dictionary<int, int> itemDic = new Dictionary<int, int>(); //id, price
        for (int i=0; i<idList.Count; i++) 
        {
            data = ItemData.GetData(idList[i]);
            itemDic.Add(data.ID, data.Price);
        }
        ScrollView.SetData(new ArrayList(itemDic));
        ScrollView.AddClickHandler(MenuOnClick);
    }

    private void SetData() 
    {
        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        NameLabel.text = _selectedData.GetName();
        VolumeLabel.text = "庫存：" + ItemManager.Instance.GetItemAmount(_selectedData.ID, ItemManager.Type.Warehouse);
        CommentLabel.text = _selectedData.GetComment();
        BuyButton.gameObject.SetActive(true);
        if (ItemManager.Instance.Money < _selectedData.Price)
        {
            BuyButton.SetColor(Color.grey);
            _canBuy = false;
            _tipText = "錢不足";
        }
        else
        {
            BuyButton.SetColor(Color.white);
            _canBuy = true;
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
            ItemManager.Instance.MinusMoney(_selectedData.Price);
            ItemManager.Instance.AddItem(_selectedData.ID, 1, ItemManager.Type.Warehouse);

            SetData();
        }
        else
        {
            TipLabel.SetLabel(_tipText);
        }
    }

    void Awake()
    {
        NameLabel.text = string.Empty;
        VolumeLabel.text = string.Empty;
        CommentLabel.text = string.Empty;

        BuyButton.gameObject.SetActive(false);
        BuyButton.ClickHandler = BuyOnClick;
        CloseButton.onClick.AddListener(Close);
    }
}
