using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip :Item
{
    public EquipData.TypeEnum Type;
    public int ATK;
    public int DEF;
    public int MTK;
    public int MEF;

    public Equip() { }

    public Equip(EquipData.TypeEnum type)
    {
        Type = type;
        Name = "無";
    }

    public Equip(int id)
    {
        ItemData.RootObject itemData = ItemData.GetData(id);
        EquipData.RootObject equipData = EquipData.GetData(id);

        if (itemData != null && equipData != null)
        {
            ID = id;
            Name = itemData.GetName();
            Comment = itemData.GetComment();
            Icon = itemData.Icon;
            Volume = itemData.Volume;
            Price = itemData.Price;
            Amount = 1;
            Type = equipData.Type;
            ATK = equipData.ATK;
            DEF = equipData.DEF;
            MTK = equipData.MTK;
            MEF = equipData.MEF;
        }
        else
        {
            Debug.Log("裝備資料不存在!");
        }
    }
}
