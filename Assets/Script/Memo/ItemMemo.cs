using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMemo
{
    public int Money;
    public int CurrentBagVolume = 0;
    public int KeyAmount = 0;

    public List<Item> BagItemList = new List<Item>(); 
    public List<Item> WarehouseItemList = new List<Item>(); 
}
