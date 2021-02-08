using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public int AddHP;
    public int AddMP;

    public Food() { }

    public Food(int id, int amount, int addHP = -1, int addMP = -1)
    {
        ItemData.RootObject itemData = ItemData.GetData(id);

        if (itemData != null)
        {
            ID = id;
            Name = itemData.GetName();
            Icon = itemData.Icon;
            Volume = itemData.Volume;
            Price = itemData.Price;
            Amount = amount;
            CanCook = itemData.CanCook;
            Type = itemData.Type;
            CookTag = itemData.CookTag;

            ItemEffectData.RootObject itemEffectData = ItemEffectData.GetData(id);
            if (addHP == -1)
            {
                AddHP = itemEffectData.AddHP;
            }
            else
            {
                AddHP = addHP;
            }

            if (addMP == -1)
            {
                AddMP = itemEffectData.AddMP;
            }
            else
            {
                AddMP = addMP;
            }

            Comment = String.Format(itemData.GetComment(), AddHP, AddMP);
        }
        else
        {
            Debug.Log("資料不存在!");
        }
    }

    public Food(Food food, int amount)
    {
        ID = food.ID;
        Name = food.Name;
        Icon = food.Icon;
        Volume = food.Volume;
        Price = food.Price;
        Amount = amount;
        CanCook = food.CanCook;
        Type = food.Type;
        AddHP = food.AddHP;
        AddMP = food.AddMP;
        Comment = food.Comment;
        CookTag = food.CookTag;
    }
}
