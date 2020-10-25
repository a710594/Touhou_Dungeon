using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int ID;
    public string Name;
    public string Comment;
    public string Icon;
    public int Volume;
    public int Price;
    public int Amount;
    public bool CanCook;
    public ItemData.TypeEnum Type;

    public Item() { }

    public Item(int id, int amount)
    {
        ItemData.RootObject itemData = ItemData.GetData(id);

        if (itemData != null)
        {
            ID = id;
            Name = itemData.GetName();
            Comment = itemData.GetComment();
            Icon = itemData.Icon;
            Volume = itemData.Volume;
            Price = itemData.Price;
            Amount = amount;
            CanCook = itemData.CanCook;
            Type = itemData.Type;
        }
        else
        {
            Debug.Log("裝備資料不存在!");
        }
    }
}
