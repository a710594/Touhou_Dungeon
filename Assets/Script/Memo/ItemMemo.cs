using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMemo
{
    public class ItemGroup
    {
        public List<Item> ItemList = new List<Item>();
        public List<Food> FoodList = new List<Food>();
        public List<Equip> EquipList = new List<Equip>();

        public void Clear()
        {
            ItemList.Clear();
            FoodList.Clear();
            EquipList.Clear();
        }

        public void Add(Item item)
        {
            if (item is Equip)
            {
                EquipList.Add((Equip)item);
            }
            else if (item is Food)
            {
                FoodList.Add((Food)item);
            }
            else
            {
                ItemList.Add(item);
            }
        }

        public List<Item> GetTotalList() //把 Item, Food, Equip 等 List 整合成一個
        {
            List<Item> list = new List<Item>();
            for (int i=0; i<ItemList.Count; i++)
            {
                list.Add(ItemList[i]);
            }
            for (int i = 0; i < FoodList.Count; i++)
            {
                list.Add(FoodList[i]);
            }
            for (int i = 0; i < EquipList.Count; i++)
            {
                list.Add(EquipList[i]);
            }
            return list;
        }
    }

    public int Money;
    public int CurrentBagVolume = 0;
    public int KeyAmount = 0;

    public ItemGroup BagGroup = new ItemGroup();
    public ItemGroup WarehouseGroup = new ItemGroup();

    public void SetBagGroup(List<Item> bagList)
    {
        BagGroup.Clear();
        for (int i = 0; i < bagList.Count; i++) 
        {
            BagGroup.Add(bagList[i]);
        }
    }

    public void SetWarehouseGroup(List<Item> warehouseList)
    {
        WarehouseGroup.Clear();
        for (int i = 0; i < warehouseList.Count; i++)
        {
            WarehouseGroup.Add(warehouseList[i]);
        }
    }

    public List<Item> GetBagList()
    {
        return BagGroup.GetTotalList();
    }

    public List<Item> GetWarehouseList()
    {
        return WarehouseGroup.GetTotalList();
    }
}
