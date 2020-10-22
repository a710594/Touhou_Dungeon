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
            Comment = itemData.GetComment();
            Icon = itemData.Icon;
            Volume = itemData.Volume;
            Price = itemData.Price;
            Amount = amount;
            Type = itemData.Type;

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
        }
        else
        {
            Debug.Log("資料不存在!");
        }
    }
}
