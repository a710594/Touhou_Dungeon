using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ByTheTale.StateMachine;

public class BagUI : MonoBehaviour
{
    public static BagUI Instance;

    public LoopScrollView ScrollView;
    public Text TotalVolumeLabel;
    public Text MoneyLabel;
    public Text KeyLabel;
    public Text NameLabel;
    public Text CommentLabel;
    public Text VolumeLabel;
    public Button CloseButton;
    public Button AllButton;
    public Button MaterialButton;
    public Button FoodButton;
    public Button MedicineButton;
    public Button EquipButton;
    public Button DiscardButton;
    public Button UseButton;
    public GameObject ItemTypeGroup;
    public GameObject UsingLabel;
    public BagSelectCharacterGroup SelectCharacterGroup;
    public SetAmountGroup SetAmountGroup;
    public EquipComment EquipComment;

    private ItemManager.Type _managerType;
    private ItemData.TypeEnum _itemType = ItemData.TypeEnum.All;
    private Item _selectedItem;
    private Equip _selectedEquip;
    private Equip _currentEquip;
    private TeamMember _selectedMember;

    public static void Open(ItemManager.Type type)
    {
        ExploreController.Instance.StopEnemy();
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("BagUI", ResourceManager.Type.UI).GetComponent<BagUI>();
        }

        Instance.Init(type);
    }

    public static void Open(ItemManager.Type type, TeamMember teamMember, Equip currentEquip)
    {
        ExploreController.Instance.StopEnemy();
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("BagUI", ResourceManager.Type.UI).GetComponent<BagUI>();
        }

        Instance.Init(type, teamMember, currentEquip);
    }

    public static void Close()
    {
        ExploreController.Instance.ContinueEnemy();
        ExploreUI.SetCanMove();
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Init(ItemManager.Type type) //一般背包用
    {
        _managerType = type;
        _selectedMember = null;

        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        KeyLabel.text = ItemManager.Instance.KeyAmount.ToString();
        SetScrollView(ItemData.TypeEnum.All, false);
        ItemTypeGroup.SetActive(true);
        SetVolume();
        ClearInfo();

        if (type == ItemManager.Type.Bag)
        {
            SelectCharacterGroup.SetData();
        }
    }

    private void Init(ItemManager.Type type, TeamMember member, Equip currentEquip) //換裝備用
    {
        _managerType = type;
        _selectedMember = member;
        _currentEquip = currentEquip;

        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        KeyLabel.text = ItemManager.Instance.KeyAmount.ToString();
        SetScrollView(ItemManager.Instance.GetEquipListByType(type, currentEquip.EquipType));
        ItemTypeGroup.SetActive(false);
        SetVolume();
        ClearInfo();
        //SelectCharacterGroup.SetData();
    }

    private void SetScrollView(ItemData.TypeEnum type, bool isRefresh)
    {
        List<Item> itemList = new List<Item>();
        itemList = ItemManager.Instance.GetItemListByType(_managerType, type);

        if (type == ItemData.TypeEnum.Equip)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                Debug.Log(((Equip)itemList[i]).EquipType);
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

    private void SetScrollView(List<Equip> list)
    {
        if (_currentEquip.ID != 0)
        {
            list.Insert(1, _currentEquip);
        }
        ScrollView.SetData(new ArrayList(list));
    }

    private void SetVolume()
    {
        if (_managerType == ItemManager.Type.Bag)
        {
            TotalVolumeLabel.gameObject.SetActive(true);
            if (ItemManager.Instance.CurrentBagVolume <= ItemManager.Instance.MaxBagVolume)
            {
                TotalVolumeLabel.text = ItemManager.Instance.CurrentBagVolume + "/" + ItemManager.Instance.MaxBagVolume;
            }
            else
            {
                TotalVolumeLabel.text = "<color=red>" + ItemManager.Instance.CurrentBagVolume + "</color>/" + ItemManager.Instance.MaxBagVolume;
            }
        }
        else
        {
            TotalVolumeLabel.gameObject.SetActive(false);
        }
    }

    private void ClearInfo()
    {
        NameLabel.text = "";
        CommentLabel.text = "";
        VolumeLabel.text = "";
        EquipComment.gameObject.SetActive(false);
        DiscardButton.gameObject.SetActive(false);
        UseButton.gameObject.SetActive(false);
    }

    private void IconOnClick(object obj)
    {
        if (obj is Equip)
        {
            _selectedEquip = (Equip)obj;
            _selectedItem = _selectedEquip;
            NameLabel.text = _selectedEquip.Name;
            CommentLabel.text = _selectedEquip.Comment;
            EquipComment.SetData(_selectedEquip);
            EquipComment.gameObject.SetActive(true);
            VolumeLabel.text = "體積：" + _selectedEquip.Volume;
            UsingLabel.SetActive(_selectedEquip == _currentEquip);

            if (_managerType == ItemManager.Type.Bag && _selectedMember == null)
            {
                DiscardButton.gameObject.SetActive(true);
            }
            else
            {
                DiscardButton.gameObject.SetActive(false);
            }

            if (_selectedMember != null && _selectedEquip != _currentEquip)
            {
                UseButton.gameObject.SetActive(true);
            }
            else
            {
                UseButton.gameObject.SetActive(false);
            }
        }
        else if (obj is Item)
        {
            _selectedItem = (Item)obj;
            _selectedEquip = null;
            NameLabel.text = _selectedItem.Name;
            CommentLabel.text = _selectedItem.Comment;
            EquipComment.gameObject.SetActive(false);
            VolumeLabel.text = "體積：" + _selectedItem.Volume;

            if (_managerType == ItemManager.Type.Bag)
            {
                if (_selectedItem.ID == 0) //緊急逃脫裝置不可丟棄
                {
                    DiscardButton.gameObject.SetActive(false);
                }
                else
                {
                    DiscardButton.gameObject.SetActive(true);
                }
            }
            else
            {
                DiscardButton.gameObject.SetActive(false);
            }

            if (_managerType == ItemManager.Type.Bag &&
                (_selectedItem.Type == ItemData.TypeEnum.Food || _selectedItem.Type == ItemData.TypeEnum.Medicine || _selectedItem.Type == ItemData.TypeEnum.GoHome))
            {
                UseButton.gameObject.SetActive(true);
            }
            else
            {
                UseButton.gameObject.SetActive(false);
            }
        }
    }

    private void CloseOnClick()
    {
        if (ItemManager.Instance.CurrentBagVolume > ItemManager.Instance.MaxBagVolume)
        {
            ConfirmUI.Open("背包已滿！請選擇要丟棄的道具。", "確定", null);
        }
        else
        {
            Close();
        }
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

    private void UseOnClick()
    {
        if (_selectedMember != null) //從成員UI來的,換裝備
        {
            Equip oldEquip;
            _selectedMember.SetEquip(_selectedEquip, out oldEquip);
            if (oldEquip != null && oldEquip.ID != 0)
            {
                ItemManager.Instance.AddItem(oldEquip, 1, _managerType);
            }
            ItemManager.Instance.MinusEquip(_managerType, _selectedEquip);

            Close();
            TeamUI.Instance.SetEquipData();
            TeamUI.Instance.SetCharacterData();
        }
        else if (_selectedItem != null) //使用道具
        {
            if (_selectedItem.Type == ItemData.TypeEnum.GoHome)
            {
                ExploreController.Instance.BackToVilliage();
                Close();
            }
            else
            {
                SelectCharacterGroup.gameObject.SetActive(true);
            }
        }
    }

    private void DiscardOnClick()
    {
        int maxAmount = 0; //最大的可丟棄數量,也就是該種物品的目前數量
        if (_selectedItem != null)
        {
            maxAmount = ItemManager.Instance.GetItemAmount(_selectedItem.ID, _managerType);
        }
        else if (_selectedEquip != null)
        {
            maxAmount = 1;
        }
        SetAmountGroup.Open(maxAmount, "要丟棄幾個？", (amount)=> 
        {
            if (_selectedEquip != null)
            {
                ItemManager.Instance.MinusEquip(_managerType, _selectedEquip);
            }
            else
            {
                ItemManager.Instance.MinusItem(_selectedItem, amount, _managerType);
            }
            SetVolume();
            SetScrollView(_itemType, true);
        });
    }

    private void CharacterOnClick(TeamMember member)
    {
        Food food = (Food)_selectedItem;
        if (food.AddHP != 0)
        {
            member.AddHP(food.AddHP);
            SelectCharacterGroup.HPBarTween(member);
        }
        if (food.AddMP != 0)
        {
            member.AddMP(food.AddMP);
            SelectCharacterGroup.MPBarTween(member);
        }

        ItemManager.Instance.MinusItem(_selectedItem, 1, _managerType);

        SetVolume();
        SetScrollView(_itemType, false);
    }


    void Awake()
    {
        DiscardButton.gameObject.SetActive(false);
        UseButton.gameObject.SetActive(false);
        SelectCharacterGroup.gameObject.SetActive(false);
        UsingLabel.SetActive(false);

        CloseButton.onClick.AddListener(CloseOnClick);
        AllButton.onClick.AddListener(AllOnClick);
        MaterialButton.onClick.AddListener(MaterialOnClick);
        FoodButton.onClick.AddListener(FoodOnClick);
        MedicineButton.onClick.AddListener(MedicineOnClick);
        EquipButton.onClick.AddListener(EquipOnClick);
        UseButton.onClick.AddListener(UseOnClick);
        DiscardButton.onClick.AddListener(DiscardOnClick);
        SelectCharacterGroup.CharacterOnClickHandler = CharacterOnClick;
        ScrollView.AddClickHandler(IconOnClick);
    }
}
