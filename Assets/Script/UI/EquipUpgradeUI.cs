using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipUpgradeUI : MonoBehaviour
{
    public static EquipUpgradeUI Instance;

    public LoopScrollView ScrollView;
    public Text MoneyLabel;
    public Text NameLabel;
    public Text CommentLabel;
    public Text NeedMoneyLabel;
    public Text OwnerLabel;
    public EquipComment EquipComment;
    public Button CloseButton;
    public TextButton UpgradeButton;
    public TipLabel TipLabel;

    private bool _canUpgrade = false;
    private string _tipText;
    private ItemManager.Type _managerType;
    private Equip _selectedEquip = null;

    public static void Open(ItemManager.Type type)
    {
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("EquipUpgradeUI", ResourceManager.Type.UI).GetComponent<EquipUpgradeUI>();
        }

        Instance.Init(type);
    }

    public static void Close()
    {
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Init(ItemManager.Type type)
    {
        _managerType = type;
        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        SetScrollView(false);
    }

    private void SetScrollView(bool isRefresh)
    {
        List<Item> itemList = new List<Item>(ItemManager.Instance.GetItemListByType(_managerType, ItemData.TypeEnum.Equip));
        List<TeamMember> memberList = TeamManager.Instance.GetAttendList();

        for (int i= memberList.Count - 1; i>=0; i--) 
        {
            if (memberList[i].Armor.ID != 0)
            {
                itemList.Insert(0, memberList[i].Armor);
            }
            if (memberList[i].Weapon.ID != 0)
            {
                itemList.Insert(0, memberList[i].Weapon);
            }
        }

        if (isRefresh)
        {
            ScrollView.Refresh(new ArrayList(itemList));
        }
        else
        {
            ScrollView.SetData(new ArrayList(itemList));
        }
    }

    private void SetData()
    {
        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        NameLabel.text = _selectedEquip.Name;
        CommentLabel.text = _selectedEquip.Comment;
        NeedMoneyLabel.text = "價格：" + _selectedEquip.UpgradePrice;
        if (_selectedEquip.Owner != string.Empty)
        {
            OwnerLabel.text = "持有者：" + _selectedEquip.Owner;
        }
        else
        {
            OwnerLabel.text = string.Empty;
        }
        EquipComment.gameObject.SetActive(true);
        EquipComment.SetData(_selectedEquip);
        UpgradeButton.gameObject.SetActive(true);

        _canUpgrade = true;

        if (ItemManager.Instance.Money < _selectedEquip.UpgradePrice)
        {
            _canUpgrade = false;
            _tipText = "錢不足";
        }

        if (_selectedEquip.Lv == Equip.MaxLv)
        {
            _canUpgrade = false;
            _tipText = "已達等級上限";
        }

        if (_canUpgrade)
        {
            UpgradeButton.SetColor(Color.white);
        }
        else
        {
            UpgradeButton.SetColor(Color.grey);
        }
    }

    private void ClearInfo()
    {
        NameLabel.text = string.Empty;
        CommentLabel.text = string.Empty;
        NeedMoneyLabel.text = string.Empty;
        OwnerLabel.text = string.Empty;
        EquipComment.gameObject.SetActive(false);
        UpgradeButton.gameObject.SetActive(false);
    }

    private void EquipOnClick(object obj)
    {
        _selectedEquip = (Equip)obj;
        SetData();
    }

    private void UpgradeOnClick(object obj)
    {
        if (_canUpgrade)
        {
            ItemManager.Instance.MinusMoney(_selectedEquip.UpgradePrice);
            _selectedEquip.LvUp();
            SetData();
            SetScrollView(true);

        }
        else
        {
            TipLabel.SetLabel(_tipText);
        }
    }

    void Awake()
    {
        ClearInfo();

        UpgradeButton.OnClickHandler = UpgradeOnClick;
        CloseButton.onClick.AddListener(Close);
        ScrollView.AddClickHandler(EquipOnClick);
    }
}
