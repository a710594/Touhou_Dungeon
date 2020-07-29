using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : BattleStatus
{
    public enum ValueType
    {
        ATK,
        DEF,
        MTK,
        MEF,
        AGI,
        SEN,
        MoveDistance,
    }

    public float Value;

    public Buff() { }

    public Buff(int id, int lv)
    {
        Data = BattleStatusData.GetData(id);
        if (Data.ValueList.Count >= lv)
        {
            Value = Data.ValueList[lv - 1];
        }
        else
        {
            Value = Data.ValueList[0];
        }

        if (Data.ValueType != BattleStatusData.TypeEnum.MOV)
        {
            Value /= 100f;
        }

        RemainTurn = Data.Turn;
        Message = Data.Message;
        Icon = Data.Icon;

        Comment = Data.GetComment(lv);
    }
}
