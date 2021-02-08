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
    public bool CanBeStacked;
    public ItemData.TypeEnum Type;
    public int CookTag;

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
            CanBeStacked = itemData.CanBeStacked;
            Type = itemData.Type;
            CookTag = itemData.CookTag;
        }
        else
        {
            Debug.Log("裝備資料不存在!");
        }
    }

    public Item(Item item, int amount)
    {
        ID = item.ID;
        Name = item.Name;
        Comment = item.Comment;
        Icon = item.Icon;
        Volume = item.Volume;
        Price = item.Price;
        Amount = amount;
        CanCook = item.CanCook;
        CanBeStacked = item.CanBeStacked;
        Type = item.Type;
        CookTag = item.CookTag;
    }
}
