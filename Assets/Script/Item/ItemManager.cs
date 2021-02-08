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

    private Equip _defaultWeapon = new Equip(EquipData.TypeEnum.Weapon);
    private Equip _defaultArmor = new Equip(EquipData.TypeEnum.Armor);

    public void Init()
    {
        _bagTypeDic.Clear();
        _warehouseTypeDic.Clear();
        foreach (ItemData.TypeEnum type in (ItemData.TypeEnum[])Enum.GetValues(typeof(ItemData.TypeEnum)))
        {
            _bagTypeDic.Add(type, new List<Item>());
            _warehouseTypeDic.Add(type, new List<Item>());
        }

        ItemMemo memo = Caretaker.Instance.Load<ItemMemo>();
        if (memo != null)
        {
            Money = memo.Money;
            KeyAmount = memo.KeyAmount;

            List<Item> bagList = memo.GetBagList();
            for(int i=0; i<bagList.Count; i++)
            {
                _bagTypeDic[ItemData.TypeEnum.All].Add(bagList[i]);
                _bagTypeDic[bagList[i].Type].Add(bagList[i]);
            }

            List<Item> warehouseList = memo.GetWarehouseList();
            for (int i = 0; i < warehouseList.Count; i++)
            {
                _warehouseTypeDic[ItemData.TypeEnum.All].Add(warehouseList[i]);
                _warehouseTypeDic[warehouseList[i].Type].Add(warehouseList[i]);
            }
        }
        //AddItem(21002, 5, Type.Warehouse);
        //AddItem(21003, 5, Type.Warehouse);
        //AddItem(21004, 5, Type.Warehouse);
        //AddItem(21005, 5, Type.Warehouse);
        //AddItem(21006, 5, Type.Warehouse);
        //AddItem(21007, 5, Type.Warehouse);
    }

    public void Save()
    {
        ItemMemo memo = new ItemMemo();
        memo.Money = Money;
        memo.CurrentBagVolume = CurrentBagVolume;
        memo.KeyAmount = KeyAmount;

        memo.SetBagGroup(_bagTypeDic[ItemData.TypeEnum.All]);
        memo.SetWarehouseGroup(_warehouseTypeDic[ItemData.TypeEnum.All]);

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

        if (BagItemListContains(id) && data.CanBeStacked)
        {
            item = GetBagItem(id);
            item.Amount += amount;
        }
        else
        {
            item = GetNewItem(id, data.Type, amount);
            _bagTypeDic[ItemData.TypeEnum.All].Add(item);
            _bagTypeDic[data.Type].Add(item);
        }

        CurrentBagVolume += data.Volume * amount;
    }

    private void AddWarehouseItem(int id, int amount)
    {
        Item item;
        ItemData.RootObject data = ItemData.GetData(id);


        if (WarehouseItemListContains(id) && data.CanBeStacked)
        {
            item = GetWarehouseItem(id);
            item.Amount += amount;
        }
        else
        {
            item = GetNewItem(id, data.Type, amount);
            _warehouseTypeDic[ItemData.TypeEnum.All].Add(item);
            _warehouseTypeDic[data.Type].Add(item);
        }
        
    }

    public void AddItem(Item item, int amount, Type type)
    {
        if (type == Type.Bag)
        {
            AddBagItem(item, amount);
        }
        else
        {
            AddWarehouseItem(item, amount);
        }
    }

    public void AddBagItem(Item item, int amount)
    {
        Item bagItem;

        if (BagItemListContains(item.ID) && item.CanBeStacked)
        {
            bagItem = GetBagItem(item.ID);
            bagItem.Amount += amount;
        }
        else
        {
            bagItem = GetNewItem(item, amount);
            _bagTypeDic[ItemData.TypeEnum.All].Add(bagItem);
            _bagTypeDic[item.Type].Add(bagItem);
        }

        CurrentBagVolume += item.Volume;
    }

    public void AddWarehouseItem(Item item, int amount)
    {
        Item warehouseItem;

        if (WarehouseItemListContains(item.ID) && item.CanBeStacked)
        {
            warehouseItem = GetWarehouseItem(item.ID);
            warehouseItem.Amount += amount;
        }
        else
        {
            warehouseItem = GetNewItem(item, amount);
            _warehouseTypeDic[ItemData.TypeEnum.All].Add(warehouseItem);
            _warehouseTypeDic[item.Type].Add(warehouseItem);
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
            if (data.CanBeStacked)
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
                }
                CurrentBagVolume -= data.Volume * amount;
            }
            else
            {
                Debug.Log(data.GetName() + "不是可堆疊的道具");
            }
        }
        else
        {
            Debug.Log("沒有這個道具: " + data.GetName());
        }
    }

    private void MinusWarehouseItem(int id, int amount)
    {
        Item item;
        ItemData.RootObject data = ItemData.GetData(id);

        if (WarehouseItemListContains(id))
        {
            if (data.CanBeStacked)
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
                }
            }
            else
            {
                Debug.Log(data.GetName() + "不是可堆疊的道具");
            }
        }
        else
        {
            Debug.Log("沒有這個道具: " + data.GetName());
        }
    }

    public void MinusItem(Item item, int amount, Type type)
    {
        if (type == Type.Bag)
        {
            MinusBagItem(item, amount);
        }
        else
        {
            MinusWarehouseItem(item, amount);
        }
    }

    private void MinusBagItem(Item item, int amount)
    {
        if (BagItemListContains(item.ID))
        {
            if (item.CanBeStacked)
            {
                item.Amount -= amount;

                if (item.Amount < 0)
                {
                    Debug.Log("道具不足,差 " + item.Amount * -1 + " 個");
                    item.Amount += amount;
                }
                else if (item.Amount == 0)
                {
                    _bagTypeDic[ItemData.TypeEnum.All].Remove(item);
                    _bagTypeDic[item.Type].Remove(item);
                }
            }
            else
            {
                _bagTypeDic[ItemData.TypeEnum.All].Remove(item);
                _bagTypeDic[ItemData.TypeEnum.Equip].Remove(item);
            }
            CurrentBagVolume -= item.Volume * amount;
        }
        else
        {
            Debug.Log("沒有這個道具: " + item.Name);
        }
    }

    private void MinusWarehouseItem(Item item, int amount)
    {
        if (WarehouseItemListContains(item.ID))
        {
            if (item.CanBeStacked)
            {
                item.Amount -= amount;

                if (item.Amount < 0)
                {
                    Debug.Log("道具不足,差 " + item.Amount * -1 + " 個");
                    item.Amount += amount;
                }
                else if (item.Amount == 0)
                {
                    _warehouseTypeDic[ItemData.TypeEnum.All].Remove(item);
                    _warehouseTypeDic[item.Type].Remove(item);
                }
            }
            else
            {
                _warehouseTypeDic[ItemData.TypeEnum.All].Remove(item);
                _warehouseTypeDic[item.Type].Remove(item);
            }
        }
        else
        {
            Debug.Log("沒有這個道具: " + item.Name);
        }
    }

    public void MinusEquip(Type type, Equip equip)
    {
        if (type == Type.Bag)
        {
            _bagTypeDic[ItemData.TypeEnum.All].Remove(equip);
            _bagTypeDic[ItemData.TypeEnum.Equip].Remove(equip);
            CurrentBagVolume -= equip.Volume;
        }
        else
        {
            _warehouseTypeDic[ItemData.TypeEnum.All].Remove(equip);
            _warehouseTypeDic[ItemData.TypeEnum.Equip].Remove(equip);
        }
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
        Equip equip;
        List<Item> itemList;
        List<Equip> equipList = new List<Equip>();
        if (type == Type.Bag)
        {
            itemList = _bagTypeDic[ItemData.TypeEnum.Equip];
        }
        else
        {
            itemList = _warehouseTypeDic[ItemData.TypeEnum.Equip];
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            equip = (Equip)itemList[i];
            if (equip.EquipType == equipType)
            {
                equipList.Add(equip);
            }
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
        Item item;
        List<Item> itemList;
        List<Item> canCookList = new List<Item>();
        if (type == Type.Bag)
        {
            itemList = _bagTypeDic[ItemData.TypeEnum.All];
        }
        else
        {
            itemList = _warehouseTypeDic[ItemData.TypeEnum.All];
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            item = itemList[i];
            if (item.CanCook)
            {
                canCookList.Add(item);
            }
        }

        return canCookList;
    }

    public void BagToWarehouse() //回村莊的時候,把背包的東西都放進倉庫裡
    {
        foreach (Item item in _bagTypeDic[ItemData.TypeEnum.All])
        {
            AddWarehouseItem(item, item.Amount);
        }

        CurrentBagVolume = 0;
        KeyAmount = 0;
        _bagTypeDic.Clear();

        foreach (ItemData.TypeEnum type in (ItemData.TypeEnum[])Enum.GetValues(typeof(ItemData.TypeEnum)))
        {
            _bagTypeDic.Add(type, new List<Item>());
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

    private Item GetNewItem(int id, ItemData.TypeEnum type, int amount)
    {
        if (type == ItemData.TypeEnum.Equip)
        {
            return new Equip(id);
        }
        if (type == ItemData.TypeEnum.Food || type == ItemData.TypeEnum.Medicine)
        {
            return new Food(id, amount);
        }
        else
        {
            return new Item(id, amount);
        }
    }

    private Item GetNewItem(Item item, int amount)
    {
        if (item is Equip)
        {
            return new Equip((Equip)item);
        }
        if (item is Food)
        {
            return new Food((Food)item, amount);
        }
        else
        {
            return new Item(item, amount);
        }
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
