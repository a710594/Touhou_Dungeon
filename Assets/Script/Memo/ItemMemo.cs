using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMemo
{
    public int Money;
    public int CurrentBagVolume = 0;
    public int KeyAmount = 0;

    public Dictionary<int, int> BagItemDic = new Dictionary<int, int>(); //key 為負者為 equip, 其((值 * -1) -1) 為 EquipList 的 index
    public Dictionary<int, int> WarehouseItemDic = new Dictionary<int, int>(); //key 為負者為 equip, 其((值 * -1)  -1)為 EquipList 的 index

    public List<Equip> BagEquipList = new List<Equip>();
    public List<Equip> WarehouseEquipList = new List<Equip>();
}
