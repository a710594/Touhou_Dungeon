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

    public float ATK = 1;
    public float DEF = 1;
    public float MTK = 1;
    public float MEF = 1;
    public float AGI = 1;
    public float SEN = 1;
    public float MOV;

    public Buff() { }

    public Buff(int id)
    {
        Data = BattleStatusData.GetData(id);

        if (Data.ValueType == BattleStatusData.TypeEnum.ATK)
        {
            ATK = (float)Data.Value / 100f;
        }
        else if (Data.ValueType == BattleStatusData.TypeEnum.DEF)
        {
            DEF = (float)Data.Value / 100f;
        }
        else if (Data.ValueType == BattleStatusData.TypeEnum.MTK)
        {
            MTK = (float)Data.Value / 100f;
        }
        else if (Data.ValueType == BattleStatusData.TypeEnum.MEF)
        {
            MEF = (float)Data.Value / 100f;
        }
        else if (Data.ValueType == BattleStatusData.TypeEnum.AGI)
        {
            AGI = (float)Data.Value / 100f;
        }
        else if (Data.ValueType == BattleStatusData.TypeEnum.SEN)
        {
            ATK = (float)Data.Value / 100f;
        }
        else if (Data.ValueType == BattleStatusData.TypeEnum.MOV)
        {
            MOV = (float)Data.Value / 100f;
        }

        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Message = Data.Message;
        Icon = Data.Icon;
    }
}
