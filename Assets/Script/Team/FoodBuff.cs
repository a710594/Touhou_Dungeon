using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBuff
{
    public bool IsEmpty = true;
    public float ATK = 1;
    public float DEF = 1;
    public float MTK = 1;
    public float MEF = 1;
    public float AGI = 1;
    public float SEN = 1;
    public string FoodName;
    public string Comment = string.Empty;

    public FoodBuff() { }

    public FoodBuff(ItemEffectData.RootObject data)
    {
        ATK = (float)data.ATK / 100f;
        DEF = (float)data.DEF / 100f;
        MTK = (float)data.MTK / 100f;
        MEF = (float)data.MEF / 100f;
        AGI = (float)data.AGI / 100f;
        SEN = (float)data.SEN / 100f;
        FoodName = ItemData.GetData(data.ID).GetName();
        Comment = data.BuffComment;
        IsEmpty = false;
    }

    public void Clear()
    {
        ATK = 1;
        DEF = 1;
        MTK = 1;
        MEF = 1;
        AGI = 1;
        SEN = 1;
        Comment = string.Empty;
        IsEmpty = true;
    }
}
