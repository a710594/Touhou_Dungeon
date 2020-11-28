using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip :Item
{
    public static readonly int MaxLv = 5;

    public EquipData.TypeEnum EquipType;
    public int ATK;
    public int DEF;
    public int MTK;
    public int MEF;


    private int _lv;

    public Equip() { }

    public Equip(EquipData.TypeEnum type)
    {
        EquipType = type;
        Name = "無";
    }

    public Equip(int id, int lv = 1)
    {
        ItemData.RootObject itemData = ItemData.GetData(id);
        EquipData.RootObject equipData = EquipData.GetData(id);

        if (itemData != null && equipData != null)
        {
            ID = id;
            Name = itemData.GetName();
            if (lv > 1) 
            {
                Name += "+" + (_lv - 1);
            }
            Comment = itemData.GetComment();
            Icon = itemData.Icon;
            Volume = itemData.Volume;
            Price = itemData.Price;
            Amount = 1;
            Type = itemData.Type;

            EquipType = equipData.Type;
            ATK = Mathf.RoundToInt(equipData.ATK * (1+(_lv-1)*0.1f));
            DEF = Mathf.RoundToInt(equipData.DEF * (1 + (_lv - 1) * 0.1f));
            MTK = Mathf.RoundToInt(equipData.MTK * (1 + (_lv - 1) * 0.1f));
            MEF = Mathf.RoundToInt(equipData.MEF * (1 + (_lv - 1) * 0.1f));
        }
        else
        {
            Debug.Log("裝備資料不存在!");
        }
    }

    public void SetData(int lv) 
    {
        ItemData.RootObject itemData = ItemData.GetData(ID);
        EquipData.RootObject equipData = EquipData.GetData(ID);

        _lv = lv;
        if (lv > 1)
        {
            Name = itemData.GetName();
            Name += "+" + (_lv - 1);
        }
        ATK = Mathf.RoundToInt(equipData.ATK * (1 + (_lv - 1) * 0.1f));
        DEF = Mathf.RoundToInt(equipData.DEF * (1 + (_lv - 1) * 0.1f));
        MTK = Mathf.RoundToInt(equipData.MTK * (1 + (_lv - 1) * 0.1f));
        MEF = Mathf.RoundToInt(equipData.MEF * (1 + (_lv - 1) * 0.1f));
    }
}
