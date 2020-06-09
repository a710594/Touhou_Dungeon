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
    public readonly int MaxBagVolume = 50;

    public int Money;
    public int CurrentBagVolume = 0;
    public int KeyAmount = 0;

    private Dictionary<ItemData.TypeEnum, Dictionary<object, int>> _bagTypeDic = new Dictionary<ItemData.TypeEnum, Dictionary<object, int>>(); //type, object, amount 其中 object 可以是 id 也可以是 equip
    private Dictionary<ItemData.TypeEnum, Dictionary<object, int>> _warehouseTypeDic = new Dictionary<ItemData.TypeEnum, Dictionary<object, int>>(); //type, object, amount 其中 object 可以是 id 也可以是 equip

    private Dictionary<EquipData.TypeEnum, List<Equip>> _bagEquipDic = new Dictionary<EquipData.TypeEnum, List<Equip>>();
    private Dictionary<EquipData.TypeEnum, List<Equip>> _warehouseEquipDic = new Dictionary<EquipData.TypeEnum, List<Equip>>();

    public void Init()
    {
        foreach (ItemData.TypeEnum type in (ItemData.TypeEnum[])Enum.GetValues(typeof(ItemData.TypeEnum)))
        {
            _bagTypeDic.Add(type, new Dictionary<object, int>());
            _warehouseTypeDic.Add(type, new Dictionary<object, int>());
        }

        foreach (EquipData.TypeEnum type in Enum.GetValues(typeof(EquipData.TypeEnum)))
        {
            _bagEquipDic.Add(type, new List<Equip>());
            _warehouseEquipDic.Add(type, new List<Equip>());
        }

        //temp
        AddWarehouseItem(2004, 5);
        Money = 50;

        ItemMemo memo = Caretaker.Instance.Load<ItemMemo>();
        if (memo != null)
        {
            Money = memo.Money;
            CurrentBagVolume = memo.CurrentBagVolume;
            KeyAmount = memo.KeyAmount;

            foreach (KeyValuePair<int, int> item in memo.BagItemDic)
            {
                if (item.Key < 0)
                {
                    AddBagItem(memo.BagEquipList[item.Key * -1 -1], 1);
                }
                else
                {
                    AddBagItem(item.Key, item.Value);
                }
            }

            foreach (KeyValuePair<int, int> item in memo.WarehouseItemDic)
            {
                if (item.Key == -1)
                {
                    AddWarehouseItem(memo.WarehouseEquipList[item.Key * -1 - 1], 1);
                }
                else
                {
                    AddWarehouseItem(item.Key, item.Value);
                }
            }
        }
    }

    public void Save()
    {
        int equipIndex;

        ItemMemo memo = new ItemMemo();
        memo.Money = Money;
        memo.CurrentBagVolume = CurrentBagVolume;
        memo.KeyAmount = KeyAmount;

        equipIndex = 1;
        foreach (KeyValuePair<object, int> item in _bagTypeDic[ItemData.TypeEnum.All])
        {
            if (item.Key is int)
            {
                memo.BagItemDic.Add((int)item.Key, item.Value);
            }
            else if (item.Key is Equip)
            {
                memo.BagItemDic.Add(equipIndex * -1, 0);
                memo.BagEquipList.Add((Equip)item.Key);
                equipIndex++;
            }
        }

        equipIndex = 1;
        foreach (KeyValuePair<object, int> item in _warehouseTypeDic[ItemData.TypeEnum.All])
        {
            if (item.Key is int)
            {
                memo.WarehouseItemDic.Add((int)item.Key, item.Value);
            }
            else if (item.Key is Equip)
            {
                memo.WarehouseItemDic.Add(-1, equipIndex);
                memo.WarehouseEquipList.Add((Equip)item.Key);
                equipIndex++;
            }
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

    public void AddItem(object obj, int amount, Type type)
    {
        if (type == Type.Bag)
        {
            AddBagItem(obj, amount);
        }
        else
        {
            AddWarehouseItem(obj, amount);
        }
    }

    private void AddBagItem(List<int> idList)
    {
        for (int i = 0; i < idList.Count; i++)
        {
            AddBagItem(idList[i], 1);
        }
    }

    private void AddWarehouseItem(List<int> idList)
    {
        for (int i = 0; i < idList.Count; i++)
        {
            AddWarehouseItem(idList[i], 1);
        }
    }

    private void AddBagItem(object obj, int amount)
    {
        if (obj is int)
        {
            int id = (int)obj;
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
                if (_bagTypeDic[ItemData.TypeEnum.All].ContainsKey(id))
                {
                    _bagTypeDic[ItemData.TypeEnum.All][id] += amount;
                    _bagTypeDic[data.Type][id] += amount;
                }
                else
                {
                    _bagTypeDic[ItemData.TypeEnum.All].Add(id, amount);
                    _bagTypeDic[data.Type].Add(id, amount);
                }
                //MissionManager.Instance.CheckMission(MissionData.TypeEnum.GetItem, id, BagDic[id]);
            }
            CurrentBagVolume += data.Volume * amount;
            //SortBag();
        }
        else
        {
            for (int i=0; i<amount; i++)
            {
                AddBagEquip((Equip)obj);
            }
        }
    }

    private void AddBagEquip(Equip equip)
    {
        _bagEquipDic[equip.Type].Add(equip);
        _bagTypeDic[ItemData.TypeEnum.All].Add(equip, 1);
        _bagTypeDic[ItemData.TypeEnum.Equip].Add(equip, 1);
    }

    private void AddWarehouseItem(object obj, int amount)
    {
        if (obj is int)
        {
            int id = (int)obj;
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
                if (_warehouseTypeDic[ItemData.TypeEnum.All].ContainsKey(id))
                {
                    _warehouseTypeDic[ItemData.TypeEnum.All][id] += amount;
                    _warehouseTypeDic[data.Type][id] += amount;
                }
                else
                {
                    _warehouseTypeDic[ItemData.TypeEnum.All].Add(id, amount);
                    _warehouseTypeDic[data.Type].Add(id, amount);
                }
                //MissionManager.Instance.CheckMission(MissionData.TypeEnum.GetItem, id, WarehouseDic[id]);
            }
            //SortWarehouse();
        }
        else
        {
            AddWarehouseEquip((Equip)obj);
        }
    }

    private void AddWarehouseEquip(Equip equip)
    {
        _warehouseEquipDic[equip.Type].Add(equip);
        _warehouseTypeDic[ItemData.TypeEnum.All].Add(equip, 1);
        _warehouseTypeDic[ItemData.TypeEnum.Equip].Add(equip, 1);
    }

    public void MinusItem(object obj, int amount, Type type) //obj 有可能為 id 或 equip
    {
        if (type == Type.Bag)
        {
            MinusBagItem(obj, amount);
        }
        else
        {
            MinusWarehouseItem(obj, amount);
        }
    }

    private void MinusBagItem(object obj, int amount) //obj 有可能為 id 或 equip
    {
        if (obj is int)
        {
            int id = (int)obj;
            ItemData.RootObject data = ItemData.GetData(id);

            if (_bagTypeDic[ItemData.TypeEnum.All].ContainsKey(id))
            {
                _bagTypeDic[ItemData.TypeEnum.All][id] -= amount;
                _bagTypeDic[data.Type][id] -= amount;
                if (_bagTypeDic[ItemData.TypeEnum.All][id] < 0)
                {
                    Debug.Log("道具不足,差 " + _bagTypeDic[ItemData.TypeEnum.All][id] * -1 + " 個");
                    _bagTypeDic[ItemData.TypeEnum.All][id] += amount;
                    _bagTypeDic[data.Type][id] += amount;
                }
                else if (_bagTypeDic[ItemData.TypeEnum.All][id] == 0)
                {
                    _bagTypeDic[ItemData.TypeEnum.All].Remove(id);
                    _bagTypeDic[data.Type].Remove(id);
                }
                CurrentBagVolume -= data.Volume * amount;
                //SortBag();
            }
            else
            {
                Debug.Log("沒有這個道具: " + data.GetName());
            }
        }
        else
        {
            MinusBagEquip((Equip)obj);
        }
    }

    private void MinusWarehouseItem(object obj, int amount)
    {
        if (obj is int)
        {
            int id = (int)obj;
            ItemData.RootObject data = ItemData.GetData(id);

            if (_warehouseTypeDic[ItemData.TypeEnum.All].ContainsKey(id))
            {
                _warehouseTypeDic[ItemData.TypeEnum.All][id] -= amount;
                _warehouseTypeDic[data.Type][id] -= amount;
                if (_warehouseTypeDic[ItemData.TypeEnum.All][id] < 0)
                {
                    Debug.Log("道具不足,差 " + _warehouseTypeDic[ItemData.TypeEnum.All][id] * -1 + " 個");
                    _warehouseTypeDic[ItemData.TypeEnum.All][id] += amount;
                    _warehouseTypeDic[data.Type][id] += amount;
                }
                else if (_warehouseTypeDic[ItemData.TypeEnum.All][id] == 0)
                {
                    _warehouseTypeDic[ItemData.TypeEnum.All].Remove(id);
                    _warehouseTypeDic[data.Type].Remove(id);
                }
                //SortWarehouse();
            }
            else
            {
                Debug.Log("沒有這個道具: " + data.GetName());
            }
        }
        else
        {
            MinusWarehouseEquip((Equip)obj);
        }
    }

    private void MinusBagEquip(Equip equip)
    {
        _bagEquipDic[equip.Type].Remove(equip);
        _bagTypeDic[ItemData.TypeEnum.All].Remove(equip);
        _bagTypeDic[ItemData.TypeEnum.Equip].Remove(equip);
        CurrentBagVolume -= equip.Volume;
    }

    private void MinusWarehouseEquip(Equip equip)
    {
        _warehouseEquipDic[equip.Type].Remove(equip);
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
        if (type == Type.Warehouse)
        {
            if (_warehouseTypeDic[ItemData.TypeEnum.All].ContainsKey(id))
            {
                return _warehouseTypeDic[ItemData.TypeEnum.All][id];
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (_bagTypeDic[ItemData.TypeEnum.All].ContainsKey(id))
            {
                return _bagTypeDic[ItemData.TypeEnum.All][id];
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

    public Dictionary<object, int> GetItemDicByType(Type type, ItemData.TypeEnum itemType)
    {
        Dictionary<object, int> itemDic;
        if (type == Type.Bag)
        {
            _bagTypeDic.TryGetValue(itemType, out itemDic);
        }
        else
        {
            _warehouseTypeDic.TryGetValue(itemType, out itemDic);
        }
        return itemDic;
    }

    public List<Equip> GetEquipListByType(Type type, EquipData.TypeEnum equipType)
    {
        List<Equip> equipList;
        if (type == Type.Bag)
        {
            _bagEquipDic.TryGetValue(equipType, out equipList);
        }
        else
        {
            _warehouseEquipDic.TryGetValue(equipType, out equipList);
        }
        return equipList;
    }

    public void PutBagItemIntoWarehouse() //回村莊的時候,把背包的東西都放進倉庫裡
    {
        foreach (KeyValuePair<object, int> item in _bagTypeDic[ItemData.TypeEnum.All])
        {
            AddWarehouseItem(item.Key, item.Value);
        }

        CurrentBagVolume = 0;
        _bagTypeDic.Clear();
        _bagEquipDic.Clear();

        foreach (ItemData.TypeEnum type in (ItemData.TypeEnum[])Enum.GetValues(typeof(ItemData.TypeEnum)))
        {
            _bagTypeDic.Add(type, new Dictionary<object, int>());
        }

        foreach (EquipData.TypeEnum type in Enum.GetValues(typeof(EquipData.TypeEnum)))
        {
            _bagEquipDic.Add(type, new List<Equip>());
        }
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
