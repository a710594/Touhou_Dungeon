using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
    public enum Type
    {
        Warehouse,
        Bag,
    }

    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemManager();
            }
            return _instance;
        }
    }

    private int _maxMoney = 99999;
    public readonly int MaxBagVolume = 100;

    public int Money;
    public int CurrentBagVolume = 0;
    public int KeyAmount = 0;

    private Dictionary<ItemData.TypeEnum, List<Item>> _bagTypeDic = new Dictionary<ItemData.TypeEnum, List<Item>>();
    private Dictionary<ItemData.TypeEnum, List<Item>> _warehouseTypeDic = new Dictionary<ItemData.TypeEnum, List<Item>>();

    private Dictionary<EquipData.TypeEnum, List<Equip>> _bagEquipDic = new Dictionary<EquipData.TypeEnum, List<Equip>>();
    private Dictionary<EquipData.TypeEnum, List<Equip>> _warehouseEquipDic = new Dictionary<EquipData.TypeEnum, List<Equip>>();

    private List<Item> _bagCanCookList = new List<Item>();
    private List<Item> _warehouseCanCookList = new List<Item>();

    private Equip _defaultWeapon = new Equip(EquipData.TypeEnum.Weapon);
    private Equip _defaultArmor = new Equip(EquipData.TypeEnum.Armor);

    public void Init()
    {
        foreach (ItemData.TypeEnum type in (ItemData.TypeEnum[])Enum.GetValues(typeof(ItemData.TypeEnum)))
        {
            _bagTypeDic.Add(type, new List<Item>());
            _warehouseTypeDic.Add(type, new List<Item>());
        }


        foreach (EquipData.TypeEnum type in Enum.GetValues(typeof(EquipData.TypeEnum)))
        {
            _bagEquipDic.Add(type, new List<Equip>());
            _warehouseEquipDic.Add(type, new List<Equip>());
        }

        ItemMemo memo = Caretaker.Instance.Load<ItemMemo>();
        if (memo != null)
        {
            Money = memo.Money;
            KeyAmount = memo.KeyAmount;

            foreach (Item item in memo.BagItemList)
            {
                AddBagItem(item.ID, item.Amount);
            }

            foreach (Item item in memo.WarehouseItemList)
            {
                AddWarehouseItem(item.ID, item.Amount);
            }
        }

        AddItem(1011, 5, Type.Warehouse);
        AddItem(1012, 5, Type.Warehouse);
        AddItem(1013, 5, Type.Warehouse);
    }

    public void Save()
    {
        ItemMemo memo = new ItemMemo();
        memo.Money = Money;
        memo.CurrentBagVolume = CurrentBagVolume;
        memo.KeyAmount = KeyAmount;

        foreach (Item item in _bagTypeDic[ItemData.TypeEnum.All])
        {
            memo.BagItemList.Add(item);
        }

        foreach (Item item in _warehouseTypeDic[ItemData.TypeEnum.All])
        {
            memo.WarehouseItemList.Add(item);
        }

        Caretaker.Instance.Save<ItemMemo>(memo);
    }

    public void AddItem(List<int> idList, Type type)
    {
        if (type == Type.Bag)
        {
            AddBagItem(idList);
        }
        else
        {
            AddWarehouseItem(idList);
        }
    }

    public void AddItem(int id, int amount, Type type)
    {
        if (type == Type.Bag)
        {
            AddBagItem(id, amount);
        }
        else
        {
            AddWarehouseItem(id, amount);
        }
    }

    private void AddBagItem(List<int> idList)
    {
        for (int i = 0; i < idList.Count; i++)
        {
            if (idList[i] >= 0)
            {
                AddBagItem(idList[i], 1);
            }
            else //負數代表錢
            {
                AddMoney(idList[i] * -1);
            }
        }
    }

    private void AddWarehouseItem(List<int> idList)
    {
        for (int i = 0; i < idList.Count; i++)
        {
            if (idList[i] >= 0)
            {
                AddWarehouseItem(idList[i], 1);
            }
            else //負數代表錢
            {
                AddMoney(idList[i] * -1);
            }
        }
    }

    private void AddBagItem(int id, int amount)
    {
        Item item;
        ItemData.RootObject data = ItemData.GetData(id);

        if (data.Type == ItemData.TypeEnum.Equip)
        {
            Equip equip;
            for (int i = 0; i < amount; i++)
            {
                equip = new Equip(data.ID);
                AddBagEquip(equip);
            }
        }
        else
        {
            if (BagItemListContains(id))
            {
                item = GetBagItem(id);
                item.Amount += amount;
            }
            else
            {
                if (data.Type == ItemData.TypeEnum.Food || data.Type == ItemData.TypeEnum.Medicine)
                {
                    item = new Food(id, amount);
                }
                else
                {
                    item = new Item(id, amount);
                }
                _bagTypeDic[ItemData.TypeEnum.All].Add(item);
                _bagTypeDic[data.Type].Add(item);
                if (item.CanCook)
                {
                    _bagCanCookList.Add(item);
                }
            }
        }
        CurrentBagVolume += data.Volume * amount;
    }

    private void AddWarehouseItem(int id, int amount)
    {
        Item item;
        ItemData.RootObject data = ItemData.GetData(id);

        if (data.Type == ItemData.TypeEnum.Equip)
        {
            Equip equip;
            for (int i = 0; i < amount; i++)
            {
                equip = new Equip(data.ID);
                AddWarehouseEquip(equip);
            }
        }
        else
        {
            if (WarehouseItemListContains(id))
            {
                item = GetWarehouseItem(id);
                item.Amount += amount;
            }
            else
            {
                if (data.Type == ItemData.TypeEnum.Food || data.Type == ItemData.TypeEnum.Medicine)
                {
                    item = new Food(id, amount);
                }
                else
                {
                    item = new Item(id, amount);
                }
                _warehouseTypeDic[ItemData.TypeEnum.All].Add(item);
                _warehouseTypeDic[data.Type].Add(item);
                if (item.CanCook)
                {
                    _warehouseCanCookList.Add(item);
                }
            }
        }
    }

    public void AddEquip(Type type, Equip equip)
    {
        if (type == Type.Bag)
        {
            AddBagEquip(equip);
        }
        else
        {
            AddWarehouseEquip(equip);
        }
    }

    private void AddBagEquip(Equip equip)
    {
        _bagEquipDic[equip.EquipType].Add(equip);
        _bagTypeDic[ItemData.TypeEnum.All].Add(equip);
        _bagTypeDic[ItemData.TypeEnum.Equip].Add(equip);
        CurrentBagVolume += equip.Volume;
    }

    private void AddWarehouseEquip(Equip equip)
    {
        _warehouseEquipDic[equip.EquipType].Add(equip);
        _warehouseTypeDic[ItemData.TypeEnum.All].Add(equip);
        _warehouseTypeDic[ItemData.TypeEnum.Equip].Add(equip);
    }

    public void AddFood(Food food, Type type)
    {
        if (type == Type.Bag)
        {
            _bagTypeDic[ItemData.TypeEnum.All].Add(food);
            _bagTypeDic[ItemData.TypeEnum.Food].Add(food);
        }
        else
        {
            _warehouseTypeDic[ItemData.TypeEnum.All].Add(food);
            _warehouseTypeDic[ItemData.TypeEnum.Food].Add(food);
        }
    }

    public void MinusItem(int id, int amount, Type type)
    {
        if (type == Type.Bag)
        {
            MinusBagItem(id, amount);
        }
        else
        {
            MinusWarehouseItem(id, amount);
        }
    }

    private void MinusBagItem(int id, int amount)
    {
        Item item;
        ItemData.RootObject data = ItemData.GetData(id);

        if (BagItemListContains(id))
        {
            item = GetBagItem(id);
            item.Amount -= amount;

            if (item.Amount < 0)
            {
                Debug.Log("道具不足,差 " + item.Amount * -1 + " 個");
                item.Amount += amount;
            }
            else if (item.Amount == 0)
            {
                _bagTypeDic[ItemData.TypeEnum.All].Remove(item);
                _bagTypeDic[data.Type].Remove(item);
                _bagCanCookList.Remove(item);
            }
            CurrentBagVolume -= data.Volume * amount;
            //SortBag();
        }
        else
        {
            Debug.Log("沒有這個道具: " + data.GetName());
        }
    }

    private void MinusWarehouseItem(int id, int amount) //obj 有可能為 id 或 equip
    {
        Item item;
        ItemData.RootObject data = ItemData.GetData(id);

        if (WarehouseItemListContains(id))
        {
            item = GetWarehouseItem(id);
            item.Amount -= amount;

            if (item.Amount < 0)
            {
                Debug.Log("道具不足,差 " + item.Amount * -1 + " 個");
                item.Amount += amount;
            }
            else if (item.Amount == 0)
            {
                _warehouseTypeDic[ItemData.TypeEnum.All].Remove(item);
                _warehouseTypeDic[data.Type].Remove(item);
                _warehouseCanCookList.Remove(item);
            }
        }
        else
        {
            Debug.Log("沒有這個道具: " + data.GetName());
        }
    }

    public void MinusEquip(Type type, Equip equip)
    {
        if (type == Type.Bag)
        {
            MinusBagEquip(equip);
        }
        else
        {
            MinusWarehouseEquip(equip);
        }
    }

    private void MinusBagEquip(Equip equip)
    {
        _bagEquipDic[equip.EquipType].Remove(equip);
        _bagTypeDic[ItemData.TypeEnum.All].Remove(equip);
        _bagTypeDic[ItemData.TypeEnum.Equip].Remove(equip);
        CurrentBagVolume -= equip.Volume;
    }

    private void MinusWarehouseEquip(Equip equip)
    {
        _warehouseEquipDic[equip.EquipType].Remove(equip);
        _warehouseTypeDic[ItemData.TypeEnum.All].Remove(equip);
        _warehouseTypeDic[ItemData.TypeEnum.Equip].Remove(equip);
    }

    public void AddKey()
    {
        KeyAmount++;
    }

    public bool UseKey()
    {
        if (KeyAmount > 0)
        {
            KeyAmount--;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddMoney(int addMoney)
    {
        Money += addMoney;
        if (Money > _maxMoney)
        {
            Money = _maxMoney;
        }
    }

    public void MinusMoney(int minusMoney)
    {
        if (minusMoney <= Money)
        {
            Money -= minusMoney;
        }
        else
        {
            Debug.Log("錢不夠!");
        }
    }

    public bool IsMoneyEnough(int price)
    {
        return Money >= price;
    }

    public int GetItemAmount(int id, Type type)
    {
        Item item;
        if (type == Type.Warehouse)
        {
            if (WarehouseItemListContains(id))
            {
                item = GetWarehouseItem(id);
                return item.Amount;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (BagItemListContains(id))
            {
                item = GetBagItem(id);
                return item.Amount;
            }
            else
            {
                return 0;
            }
        }
    }

    public bool BagIsFull()
    {
        return CurrentBagVolume > MaxBagVolume;
    }

    public List<Item> GetItemListByType(Type type, ItemData.TypeEnum itemType)
    {
        List<Item> itemList;
        if (type == Type.Bag)
        {
            _bagTypeDic.TryGetValue(itemType, out itemList);
        }
        else
        {
            _warehouseTypeDic.TryGetValue(itemType, out itemList);
        }
        return itemList;
    }

    public List<Equip> GetEquipListByType(Type type, EquipData.TypeEnum equipType)
    {
        List<Equip> equipList;
        if (type == Type.Bag)
        {
            equipList = new List<Equip>(_bagEquipDic[equipType]);
        }
        else
        {
            equipList = new List<Equip>(_warehouseEquipDic[equipType]);
        }

        if (equipType == EquipData.TypeEnum.Weapon)
        {
            equipList.Insert(0, _defaultWeapon);
        }
        else if (equipType == EquipData.TypeEnum.Armor)
        {
            equipList.Insert(0, _defaultArmor);
        }

        return equipList;
    }

    public List<Item> GetCanCookList(Type type)
    {
        if (type == Type.Bag)
        {
            return _bagCanCookList;
        }
        else
        {
            return _warehouseCanCookList;
        }
    }

    public void PutBagItemIntoWarehouse() //回村莊的時候,把背包的東西都放進倉庫裡
    {
        foreach (Item item in _bagTypeDic[ItemData.TypeEnum.All])
        {
            AddWarehouseItem(item.ID, item.Amount);
        }

        CurrentBagVolume = 0;
        KeyAmount = 0;
        _bagTypeDic.Clear();
        _bagEquipDic.Clear();

        foreach (ItemData.TypeEnum type in (ItemData.TypeEnum[])Enum.GetValues(typeof(ItemData.TypeEnum)))
        {
            _bagTypeDic.Add(type, new List<Item>());
        }

        foreach (EquipData.TypeEnum type in Enum.GetValues(typeof(EquipData.TypeEnum)))
        {
            _bagEquipDic.Add(type, new List<Equip>());
        }
    }

    private bool BagItemListContains(int id)
    {
        for (int i=0; i<_bagTypeDic[ItemData.TypeEnum.All].Count; i++)
        {
            if (_bagTypeDic[ItemData.TypeEnum.All][i].ID == id)
            {
                return true;
            }
        }
        return false;
    }

    private bool WarehouseItemListContains(int id)
    {
        for (int i = 0; i < _warehouseTypeDic[ItemData.TypeEnum.All].Count; i++)
        {
            if (_warehouseTypeDic[ItemData.TypeEnum.All][i].ID == id)
            {
                return true;
            }
        }
        return false;
    }

    private Item GetBagItem(int id)
    {
        Item item = null;
        for (int i = 0; i < _bagTypeDic[ItemData.TypeEnum.All].Count; i++)
        {
            if (_bagTypeDic[ItemData.TypeEnum.All][i].ID == id)
            {
                item = _bagTypeDic[ItemData.TypeEnum.All][i];
                break;
            }
        }
        return item;
    }

    private Item GetWarehouseItem(int id)
    {
        Item item = null;
        for (int i = 0; i < _warehouseTypeDic[ItemData.TypeEnum.All].Count; i++)
        {
            if (_warehouseTypeDic[ItemData.TypeEnum.All][i].ID == id)
            {
                item = _warehouseTypeDic[ItemData.TypeEnum.All][i];
                break;
            }
        }
        return item;
    }

    /*private void SortBag()
    {
        List<int> itemIdList = new List<int>(BagDic.Keys);
        Dictionary<int, int> tempDic = new Dictionary<int, int>(BagDic);


        itemIdList.Sort((x, y) =>
        {
            return x.CompareTo(y);
        });

        BagDic.Clear();
        for (int i = 0; i < itemIdList.Count; i++)
        {
            BagDic.Add(itemIdList[i], tempDic[itemIdList[i]]);
        }
    }

    private void SortWarehouse()
    {
        List<int> itemIdList = new List<int>(WarehouseDic.Keys);
        Dictionary<int, int> tempDic = new Dictionary<int, int>(WarehouseDic);


        itemIdList.Sort((x, y) =>
        {
            return x.CompareTo(y);
        });

        WarehouseDic.Clear();
        for (int i = 0; i < itemIdList.Count; i++)
        {
            WarehouseDic.Add(itemIdList[i], tempDic[itemIdList[i]]);
        }
    }*/
}
