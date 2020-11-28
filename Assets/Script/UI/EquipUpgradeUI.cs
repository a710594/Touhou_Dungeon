using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipUpgradeUI : MonoBehaviour
{
    public static EquipUpgradeUI Instance;

    public LoopScrollView ScrollView;

    private ItemManager.Type _managerType;

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
        SetScrollView(false);
    }

    private void SetScrollView(bool isRefresh)
    {
        List<Item> itemList = new List<Item>();
        if (_managerType == ItemManager.Type.Bag)
        {
            itemList = ItemManager.Instance.GetItemListByType(ItemManager.Type.Bag, ItemData.TypeEnum.Equip);
        }
        else
        {
            itemList = ItemManager.Instance.GetItemListByType(ItemManager.Type.Warehouse, ItemData.TypeEnum.Equip);
        }

        for (int i= TeamManager.Instance.MemberList.Count - 1; i>=0; i--) 
        {
            itemList.Insert(0, TeamManager.Instance.MemberList[0].Armor);
            itemList.Insert(0, TeamManager.Instance.MemberList[0].Weapon);
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
}
