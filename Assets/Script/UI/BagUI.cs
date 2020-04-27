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
    public Button ItemButton;
    public Button EquipButton;
    public Button DiscardButton;
    public Button UseButton;
    public BagSelectCharacterGroup SelectCharacterGroup;
    public SetAmountGroup SetAmountGroup;

    private ItemManager.Type _type;
    private ItemData.RootObject _selectedItem;
    private Equip _selectedEquip;
    private TeamMember _selectedMember;

    public static void Open(ItemManager.Type type)
    {
        Time.timeScale = 0;
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("BagUI", ResourceManager.Type.UI).GetComponent<BagUI>();
        }

        Instance.Init(type);
    }

    public static void Open(ItemManager.Type type, TeamMember teamMember, List<Equip> equipList)
    {
        Time.timeScale = 0;
        if (Instance == null)
        {
            Instance = ResourceManager.Instance.Spawn("BagUI", ResourceManager.Type.UI).GetComponent<BagUI>();
        }

        Instance.Init(type, teamMember, equipList);
    }

    public static void Close()
    {
        Time.timeScale = 1;
        Destroy(Instance.gameObject);
        Instance = null;
    }

    private void Init(ItemManager.Type type)
    {
        _type = type;
        _selectedMember = null;

        SetData();

        if (type == ItemManager.Type.Bag)
        {
            SelectCharacterGroup.SetData();
        }
    }

    private void Init(ItemManager.Type type, TeamMember member, List<Equip> equipList)
    {
        _type = type;
        _selectedMember = member;

        SetData(equipList);
        SelectCharacterGroup.SetData();
    }

    private void SetData()
    {
        ClearInfo();

        Dictionary<object, int> itemDic = new Dictionary<object, int>();
        if (_type == ItemManager.Type.Bag)
        {
            itemDic = ItemManager.Instance.GetItemDicByType(ItemManager.Type.Bag, ItemData.TypeEnum.All);
        }
        else
        {
            itemDic = ItemManager.Instance.GetItemDicByType(ItemManager.Type.Warehouse, ItemData.TypeEnum.All);
        }
        ScrollView.SetData(new ArrayList(itemDic));

        ScrollView.AddClickHandler(IconOnClick);

        if (_type == ItemManager.Type.Bag)
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
        MoneyLabel.text = ItemManager.Instance.Money.ToString();
        KeyLabel.text = ItemManager.Instance.KeyAmount.ToString();
    }

    private void SetData(List<Equip> list)
    {
        ClearInfo();
        ScrollView.SetData(new ArrayList(list));
        ScrollView.AddClickHandler(IconOnClick);
    }

    private void ClearInfo()
    {
        NameLabel.text = "";
        CommentLabel.text = "";
        VolumeLabel.text = "";
        ItemImage.gameObject.SetActive(false);
        DiscardButton.gameObject.SetActive(false);
        UseButton.gameObject.SetActive(false);
    }

    private void IconOnClick(object obj)
    {
        if (obj is int)
        {
            _selectedItem = ItemData.GetData((int)obj);
            _selectedEquip = null;
            NameLabel.text = _selectedItem.GetName();
            CommentLabel.text = _selectedItem.GetComment();
            VolumeLabel.text = "體積：" + _selectedItem.Volume;
            ItemImage.gameObject.SetActive(true);
            ItemImage.overrideSprite = Resources.Load<Sprite>("Image/" + _selectedItem.Icon);

            if (_type == ItemManager.Type.Bag)
            {
                DiscardButton.gameObject.SetActive(true);
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
        else if (obj is Equip)
        {
            _selectedEquip = (Equip)obj;
            _selectedItem = ItemData.GetData(_selectedEquip.ID); ;
            NameLabel.text = _selectedEquip.Name;
            CommentLabel.text = _selectedEquip.Comment;
            VolumeLabel.text = "體積：" + _selectedEquip.Volume;
            ItemImage.gameObject.SetActive(true);
            ItemImage.overrideSprite = Resources.Load<Sprite>("Image/" + _selectedEquip.Icon);

            if (_type == ItemManager.Type.Bag && _selectedMember == null)
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

    private void ItemOnClick()
    {
        SetData();
    }

    private void EquipOnClick()
    {
        SetData();
    }

    private void UseOnClick()
    {
        if (_selectedMember != null) //從成員UI來的,換裝備
        {
            Equip oldEquip;
            _selectedMember.SetEquip(_selectedEquip, out oldEquip);
            if (oldEquip != null && oldEquip.ID != 0)
            {
                ItemManager.Instance.AddItem(oldEquip, 1, _type);
            }
            ItemManager.Instance.MinusItem(_selectedEquip, 1, _type);

            Close();
            TeamUI.Instance.SetEquipData();
        }
        else if (_selectedItem != null) //使用道具
        {
            if (_selectedItem.Type == ItemData.TypeEnum.GoHome)
            {
                ExploreController.Instance.BackToVilliage();
                Time.timeScale = 1;
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
            maxAmount = ItemManager.Instance.GetItemAmount(_selectedItem.ID, _type);
        }
        else if (_selectedEquip != null)
        {
            maxAmount = 1;
        }
        SetAmountGroup.Open(maxAmount, "要丟棄幾個？", (amount)=> 
        {
            if (_selectedEquip != null)
            {
                ItemManager.Instance.MinusItem(_selectedEquip, 1, _type);
            }
            else
            {
                ItemManager.Instance.MinusItem(_selectedItem.ID, amount, _type);
            }
            SetData();
        });
    }

    private void CharacterOnClick(TeamMember member)
    {
        if (_selectedItem.Type == ItemData.TypeEnum.Food || _selectedItem.Type == ItemData.TypeEnum.Medicine)
        {
            ItemEffectData.RootObject foodData = ItemEffectData.GetData(_selectedItem.ID);
            if (foodData.AddHP != 0)
            {
                member.AddHP(foodData.AddHP);
                SelectCharacterGroup.HPBarTween(member);
            }
            if (foodData.AddMP != 0)
            {
                member.AddMP(foodData.AddMP);
                SelectCharacterGroup.MPBarTween(member);
            }
            if (foodData.HasBuff)
            {
                member.SetFoodBuff(foodData.ID);
            }
            else
            {
                member.ClearFoodBuff();
            }
            ItemManager.Instance.MinusItem(_selectedItem.ID, 1, _type);
        }
        else if (_selectedItem.Type == ItemData.TypeEnum.Equip)
        {
            Equip oldEquip;
            member.SetEquip(_selectedEquip, out oldEquip);
            if (oldEquip != null && oldEquip.ID != 0)
            {
                ItemManager.Instance.AddItem(oldEquip, 1, _type);
            }
            ItemManager.Instance.MinusItem(_selectedEquip, 1, _type);
        }

        SetData();
    }


    void Awake()
    {
        DiscardButton.gameObject.SetActive(false);
        UseButton.gameObject.SetActive(false);
        SelectCharacterGroup.gameObject.SetActive(false);

        CloseButton.onClick.AddListener(CloseOnClick);
        ItemButton.onClick.AddListener(ItemOnClick);
        EquipButton.onClick.AddListener(EquipOnClick);
        UseButton.onClick.AddListener(UseOnClick);
        DiscardButton.onClick.AddListener(DiscardOnClick);
        SelectCharacterGroup.CharacterOnClickHandler = CharacterOnClick;
    }
}
