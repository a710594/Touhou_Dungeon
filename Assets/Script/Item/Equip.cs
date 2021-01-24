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
    public int Lv;
    public int UpgradePrice;
    public string Owner;

    public Equip() { }

    public Equip(EquipData.TypeEnum type) //default equip
    {
        Type = ItemData.TypeEnum.Equip;
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
            Lv = lv;
            if (lv > 1) 
            {
                Name += "+" + (Lv - 1);
            }
            Comment = itemData.GetComment();
            Icon = itemData.Icon;
            Volume = itemData.Volume;
            Price = itemData.Price;
            Amount = 1;
            Type = itemData.Type;

            EquipType = equipData.Type;
            ATK = Mathf.RoundToInt(equipData.ATK * (1+(Lv-1)*0.1f));
            DEF = Mathf.RoundToInt(equipData.DEF * (1 + (Lv - 1) * 0.1f));
            MTK = Mathf.RoundToInt(equipData.MTK * (1 + (Lv - 1) * 0.1f));
            MEF = Mathf.RoundToInt(equipData.MEF * (1 + (Lv - 1) * 0.1f));
            UpgradePrice = equipData.UpgradePrice;
        }
        else
        {
            Debug.Log("裝備資料不存在!");
        }
    }

    public Equip(Equip equip)
    {
        ID = equip.ID;
        Name = equip.Name;
        Lv = equip.Lv;
        Comment = equip.Comment;
        Icon = equip.Icon;
        Volume = equip.Volume;
        Price = equip.Price;
        Amount = 1;
        Type = equip.Type;

        EquipType = equip.EquipType;
        ATK = equip.ATK;
        DEF = equip.DEF;
        MTK = equip.MTK;
        MEF = equip.MEF;
        UpgradePrice = equip.UpgradePrice;
    }

    public void LvUp()
    {
        ItemData.RootObject itemData = ItemData.GetData(ID);
        EquipData.RootObject equipData = EquipData.GetData(ID);
        if (Lv < MaxLv)
        {
            Lv += 1;
        }
        Name = itemData.GetName();
        Name += "+" + (Lv - 1);
        ATK = Mathf.RoundToInt(equipData.ATK * (1 + (Lv - 1) * 0.1f));
        DEF = Mathf.RoundToInt(equipData.DEF * (1 + (Lv - 1) * 0.1f));
        MTK = Mathf.RoundToInt(equipData.MTK * (1 + (Lv - 1) * 0.1f));
        MEF = Mathf.RoundToInt(equipData.MEF * (1 + (Lv - 1) * 0.1f));
    }

    public void SetOwner(string owner)
    {
        Owner = owner;
    }
}
