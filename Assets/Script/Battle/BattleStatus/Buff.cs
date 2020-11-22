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

    public BattleStatusData.TypeEnum Type;
    public float Value;

    public Buff() { }

    public Buff(int id, int lv)
    {
        Data = BattleStatusData.GetData(id);
        Type = Data.ValueType;
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

    public Buff(BattleStatusData.TypeEnum type, int value) //計算機用的
    {
        Type = type;
        Value = value;

        if (type != BattleStatusData.TypeEnum.MOV)
        {
            Value /= 100f;
        }
    }
}
