using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public int AddHP;
    public int AddMP;

    public Food() { }

    public Food(int id, int amount)
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
            Type = itemData.Type;

            ItemEffectData.RootObject itemEffectData = ItemEffectData.GetData(id);
            AddHP = itemEffectData.AddHP;
            AddMP = itemEffectData.AddMP;
        }
        else
        {
            Debug.Log("裝備資料不存在!");
        }
    }
}
