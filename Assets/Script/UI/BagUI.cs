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
    public Image ItemImage;
    public Button CloseButton;
    public Button AllButton;
    public Button MaterialButton;
    public Button FoodButton;
    public Button MedicineButton;
    public Button EquipButton;
    public Button DiscardButton;
    public Button UseButton;
    public BagSelectCharacterGroup SelectCharacterGroup;
    public SetAmountGroup SetAmountGroup;
    public EquipComment EquipComment;

    private ItemManager.Type _managerType;
    private ItemData.TypeEnum _itemType = ItemData.TypeEnum.All;
    private ItemData.RootObject _selectedItem;
    private Equip _selectedEquip;
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

    public static void Open(ItemManager.Type type, TeamMember teamMember, List<Equip> equipList)
    {
        ExploreController.Instance.StopEnemy();
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("BagUI", ResourceManager.Type.UI).GetComponent<BagUI>();
        }

        Instance.Init(type, teamMember, equipList);
    }

    public static void Close()
    {
        ExploreController.Instance.ContinueEnemy();
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Init(ItemManager.Type type)
    {
        _managerType = type;
        _selectedMember = null;

        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        KeyLabel.text = ItemManager.Instance.KeyAmount.ToString();
        SetScrollView(ItemData.TypeEnum.All);
        SetVolume();
        ClearInfo();

        if (type == ItemManager.Type.Bag)
        {
            SelectCharacterGroup.SetData();
        }
    }

    private void Init(ItemManager.Type type, TeamMember member, List<Equip> equipList)
    {
        _managerType = type;
        _selectedMember = member;

        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        KeyLabel.text = ItemManager.Instance.KeyAmount.ToString();
        SetScrollView(equipList);
        SetVolume();
        ClearInfo();
        //SelectCharacterGroup.SetData();
    }

    private void SetScrollView(ItemData.TypeEnum type)
    {
        List<Item> itemList = new List<Item>();
        if (_managerType == ItemManager.Type.Bag)
        {
            itemList = ItemManager.Instance.GetItemListByType(ItemManager.Type.Bag, type);
        }
        else
        {
            itemList = ItemManager.Instance.GetItemListByType(ItemManager.Type.Warehouse, type);
        }
        ScrollView.SetData(new ArrayList(itemList));
        ScrollView.AddClickHandler(IconOnClick);
    }

    private void SetScrollView(List<Equip> list)
    {
        ScrollView.SetData(new ArrayList(list));
        ScrollView.AddClickHandler(IconOnClick);
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
        ItemImage.gameObject.SetActive(false);
        DiscardButton.gameObject.SetActive(false);
        UseButton.gameObject.SetActive(false);
    }

    private void IconOnClick(object obj)
    {
        if (obj is Equip)
        {
            _selectedEquip = (Equip)obj;
            _selectedItem = ItemData.GetData(_selectedEquip.ID); ;
            NameLabel.text = _selectedEquip.Name;
            CommentLabel.text = _selectedEquip.Comment;
            EquipComment.SetData(_selectedEquip.ID);
            EquipComment.gameObject.SetActive(true);
            VolumeLabel.text = "體積：" + _selectedEquip.Volume;
            ItemImage.gameObject.SetActive(true);
            ItemImage.overrideSprite = Resources.Load<Sprite>("Image/" + _selectedEquip.Icon);

            if (_managerType == ItemManager.Type.Bag && _selectedMember == null)
            {
                DiscardButton.gameObject.SetActive(true);
            }
            else
            {
                DiscardButton.gameObject.SetActive(false);
            }

            if (_selectedMember != null)
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
            _selectedItem = ItemData.GetData(((Item)obj).ID);
            _selectedEquip = null;
            NameLabel.text = _selectedItem.GetName();
            CommentLabel.text = _selectedItem.GetComment();
            EquipComment.gameObject.SetActive(false);
            VolumeLabel.text = "體積：" + _selectedItem.Volume;
            ItemImage.gameObject.SetActive(true);
            ItemImage.overrideSprite = Resources.Load<Sprite>("Image/" + _selectedItem.Icon);

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

            if (_selectedItem.Type == ItemData.TypeEnum.Food || _selectedItem.Type == ItemData.TypeEnum.Medicine || _selectedItem.Type == ItemData.TypeEnum.GoHome)
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
        SetScrollView(ItemData.TypeEnum.All);
        ClearInfo();
        _itemType = ItemData.TypeEnum.All;
    }

    private void MaterialOnClick()
    {
        SetScrollView(ItemData.TypeEnum.Material);
        ClearInfo();
        _itemType = ItemData.TypeEnum.Material;
    }

    private void FoodOnClick()
    {
        SetScrollView(ItemData.TypeEnum.Food);
        ClearInfo();
        _itemType = ItemData.TypeEnum.Food;
    }

    private void MedicineOnClick()
    {
        SetScrollView(ItemData.TypeEnum.Medicine);
        ClearInfo();
        _itemType = ItemData.TypeEnum.Medicine;
    }

    private void EquipOnClick()
    {
        SetScrollView(ItemData.TypeEnum.Equip);
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
                ItemManager.Instance.AddEquip(_managerType, oldEquip);
            }
            ItemManager.Instance.MinusEquip(_managerType, _selectedEquip);

            Close();
            TeamUI.Instance.SetEquipData();
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
                ItemManager.Instance.MinusItem(_selectedItem.ID, amount, _managerType);
            }
            SetVolume();
            SetScrollView(_itemType);
        });
    }

    private void CharacterOnClick(TeamMember member)
    {
        //if (_selectedItem.Type == ItemData.TypeEnum.Food || _selectedItem.Type == ItemData.TypeEnum.Medicine)
        //{
            ItemEffectData.RootObject itemEffectData = ItemEffectData.GetData(_selectedItem.ID);
            if (itemEffectData.AddHP != 0)
            {
                member.AddHP(itemEffectData.AddHP);
                SelectCharacterGroup.HPBarTween(member);
            }
            if (itemEffectData.AddMP != 0)
            {
                member.AddMP(itemEffectData.AddMP);
                SelectCharacterGroup.MPBarTween(member);
            }
            if (itemEffectData.HasBuff)
            {
                member.SetFoodBuff(itemEffectData);
            }
            else
            {
                member.ClearFoodBuff();
            }
            ItemManager.Instance.MinusItem(_selectedItem.ID, 1, _managerType);
        //}

        SetVolume();
        SetScrollView(_itemType);
    }


    void Awake()
    {
        DiscardButton.gameObject.SetActive(false);
        UseButton.gameObject.SetActive(false);
        SelectCharacterGroup.gameObject.SetActive(false);

        CloseButton.onClick.AddListener(CloseOnClick);
        AllButton.onClick.AddListener(AllOnClick);
        MaterialButton.onClick.AddListener(MaterialOnClick);
        FoodButton.onClick.AddListener(FoodOnClick);
        MedicineButton.onClick.AddListener(MedicineOnClick);
        EquipButton.onClick.AddListener(EquipOnClick);
        UseButton.onClick.AddListener(UseOnClick);
        DiscardButton.onClick.AddListener(DiscardOnClick);
        SelectCharacterGroup.CharacterOnClickHandler = CharacterOnClick;
    }
}
