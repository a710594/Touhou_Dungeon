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

    private Dictionary<ItemData.TypeEnum, Dictionary<object, int>> _typeBagDic = new Dictionary<ItemData.TypeEnum, Dictionary<object, int>>(); //type, object, amount 其中 object 可以是 id 也可以是 equip
    private Dictionary<ItemData.TypeEnum, Dictionary<object, int>> _typeWarehouseDic = new Dictionary<ItemData.TypeEnum, Dictionary<object, int>>(); //type, object, amount 其中 object 可以是 id 也可以是 equip

    private Dictionary<EquipData.TypeEnum, List<Equip>> _bagEquipDic = new Dictionary<EquipData.TypeEnum, List<Equip>>();
    private Dictionary<EquipData.TypeEnum, List<Equip>> _warehouseEquipDic = new Dictionary<EquipData.TypeEnum, List<Equip>>();

    public void Init(/*ItemInfo info = null*/)
    {
        foreach (ItemData.TypeEnum type in (ItemData.TypeEnum[])Enum.GetValues(typeof(ItemData.TypeEnum)))
        {
            _typeBagDic.Add(type, new Dictionary<object, int>());
            _typeWarehouseDic.Add(type, new Dictionary<object, int>());
        }

        foreach (EquipData.TypeEnum type in Enum.GetValues(typeof(EquipData.TypeEnum)))
        {
            _bagEquipDic.Add(type, new List<Equip>());
            _warehouseEquipDic.Add(type, new List<Equip>());
        }

        //temp
        AddBagItem(21, 5);
        Money = 10000;

        //Money = 0;
        //CurrentBagVolume = 0;
        //_bagMoney = 0;
        //_warehouseDic.Clear();
        //_bagDic.Clear();

        //if (info != null)
        //{
        //    CurrentBagVolume = info.CurrentBagVolume;
        //    Money = info.Money;
        //    _bagMoney = info.BagMoney;

        //    for (int i = 0; i < info.WarehouseIdList.Count; i++)
        //    {
        //        _warehouseDic.Add(info.WarehouseIdList[i], info.WarehouseAmountList[i]);
        //    }

        //    for (int i = 0; i < info.BagIdList.Count; i++)
        //    {
        //        _bagDic.Add(info.BagIdList[i], info.BagAmountList[i]);
        //    }
        //}
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

    private void AddBagItem(object obj, int amount)
    {
        if (obj is int)
        {
            int id = (int)obj;
            ItemData.RootObject data = ItemData.GetData(id);

            if (data.Type == ItemData.TypeEnum.Equip)
            {
                Equip equip = new Equip(data.ID);
                AddBagEquip(equip);
            }
            else
            {
                if (_typeBagDic[ItemData.TypeEnum.All].ContainsKey(id))
                {
                    _typeBagDic[ItemData.TypeEnum.All][id] += amount;
                    _typeBagDic[data.Type][id] += amount;
                }
                else
                {
                    _typeBagDic[ItemData.TypeEnum.All].Add(id, amount);
                    _typeBagDic[data.Type].Add(id, amount);
                }
                //MissionManager.Instance.CheckMission(MissionData.TypeEnum.GetItem, id, BagDic[id]);
            }
            CurrentBagVolume += data.Volume;
            //SortBag();
        }
        else
        {
            AddBagEquip((Equip)obj);
        }
    }

    private void AddBagEquip(Equip equip)
    {
        _bagEquipDic[equip.Type].Add(equip);
        _typeBagDic[ItemData.TypeEnum.All].Add(equip, 1);
        _typeBagDic[ItemData.TypeEnum.Equip].Add(equip, 1);
    }

    private void AddWarehouseItem(object obj, int amount)
    {
        if (obj is int)
        {
            int id = (int)obj;
            ItemData.RootObject data = ItemData.GetData(id);

            if (data.Type == ItemData.TypeEnum.Equip)
            {
                Equip equip = new Equip(data.ID);
                AddWarehouseEquip(equip);
            }
            else
            {
                if (_typeWarehouseDic[ItemData.TypeEnum.All].ContainsKey(id))
                {
                    _typeWarehouseDic[ItemData.TypeEnum.All][id] += amount;
                    _typeWarehouseDic[data.Type][id] += amount;
                }
                else
                {
                    _typeWarehouseDic[ItemData.TypeEnum.All].Add(id, amount);
                    _typeWarehouseDic[data.Type].Add(id, amount);
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
        _typeWarehouseDic[ItemData.TypeEnum.All].Add(equip, 1);
        _typeWarehouseDic[ItemData.TypeEnum.Equip].Add(equip, 1);
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

            if (_typeBagDic[ItemData.TypeEnum.All].ContainsKey(id))
            {
                _typeBagDic[ItemData.TypeEnum.All][id] -= amount;
                _typeBagDic[data.Type][id] -= amount;
                if (_typeBagDic[ItemData.TypeEnum.All][id] < 0)
                {
                    Debug.Log("道具不足,差 " + _typeBagDic[ItemData.TypeEnum.All][id] * -1 + " 個");
                    _typeBagDic[ItemData.TypeEnum.All][id] += amount;
                    _typeBagDic[data.Type][id] += amount;
                }
                else if (_typeBagDic[ItemData.TypeEnum.All][id] == 0)
                {
                    _typeBagDic[ItemData.TypeEnum.All].Remove(id);
                    _typeBagDic[data.Type].Remove(id);
                }
                CurrentBagVolume -= data.Volume * amount;
                //SortBag();
            }
            else
            {
                Debug.Log("沒有這個道具: " + data.Name);
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

            if (_typeWarehouseDic[ItemData.TypeEnum.All].ContainsKey(id))
            {
                _typeWarehouseDic[ItemData.TypeEnum.All][id] -= amount;
                _typeWarehouseDic[data.Type][id] -= amount;
                if (_typeWarehouseDic[ItemData.TypeEnum.All][id] < 0)
                {
                    Debug.Log("道具不足,差 " + _typeWarehouseDic[ItemData.TypeEnum.All][id] * -1 + " 個");
                    _typeWarehouseDic[ItemData.TypeEnum.All][id] += amount;
                    _typeWarehouseDic[data.Type][id] += amount;
                }
                else if (_typeWarehouseDic[ItemData.TypeEnum.All][id] == 0)
                {
                    _typeWarehouseDic[ItemData.TypeEnum.All].Remove(id);
                    _typeWarehouseDic[data.Type].Remove(id);
                }
                //SortWarehouse();
            }
            else
            {
                Debug.Log("沒有這個道具: " + data.Name);
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
        _typeBagDic[ItemData.TypeEnum.All].Remove(equip);
        _typeBagDic[ItemData.TypeEnum.Equip].Remove(equip);
        CurrentBagVolume -= equip.Volume;
    }

    private void MinusWarehouseEquip(Equip equip)
    {
        _warehouseEquipDic[equip.Type].Remove(equip);
        _typeWarehouseDic[ItemData.TypeEnum.All].Remove(equip);
        _typeWarehouseDic[ItemData.TypeEnum.Equip].Remove(equip);
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
        if (minusMoney < Money)
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
            if (_typeWarehouseDic[ItemData.TypeEnum.All].ContainsKey(id))
            {
                return _typeWarehouseDic[ItemData.TypeEnum.All][id];
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (_typeBagDic[ItemData.TypeEnum.All].ContainsKey(id))
            {
                return _typeBagDic[ItemData.TypeEnum.All][id];
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
            _typeBagDic.TryGetValue(itemType, out itemDic);
        }
        else
        {
            _typeWarehouseDic.TryGetValue(itemType, out itemDic);
        }
        return itemDic;
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
