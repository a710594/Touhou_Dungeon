using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBuff
{
    public float ATK = 1;
    public float DEF = 1;
    public float MTK = 1;
    public float MEF = 1;
    public float AGI = 1;
    public float SEN = 1;
    public string Comment;

    public FoodBuff() { }

    public FoodBuff(int id)
    {
        ItemEffectData.RootObject data = ItemEffectData.GetData(id);

        ATK = (float)data.ATK / 100f;
        DEF = (float)data.DEF / 100f;
        MTK = (float)data.MTK / 100f;
        MEF = (float)data.MEF / 100f;
        AGI = (float)data.AGI / 100f;
        SEN = (float)data.SEN / 100f;
        Comment = data.BuffComment;
    }

    public void Clear()
    {
        ATK = 1;
        DEF = 1;
        MTK = 1;
        MEF = 1;
        AGI = 1;
        SEN = 1;
    }
}
