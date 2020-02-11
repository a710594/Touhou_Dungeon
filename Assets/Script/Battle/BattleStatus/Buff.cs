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
    public float MoveDistance;

    public Buff() { }

    public Buff(int id)
    {
        Data = BattleStatusData.GetData(id);

        ATK = (float)Data.ATK / 100f;
        DEF = (float)Data.DEF / 100f;
        MTK = (float)Data.MTK / 100f;
        MEF = (float)Data.MEF / 100f;
        AGI = (float)Data.AGI / 100f;
        SEN = (float)Data.SEN / 100f;
        MoveDistance = (float)Data.MoveDistance / 100f;
        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Icon = Data.Icon;
    }

    public void Clear()
    {
        ATK = 1;
        DEF = 1;
        MTK = 1;
        MEF = 1;
        AGI = 1;
        SEN = 1;
        MoveDistance = 1;
        RemainTurn = 0;
        Data = null;
    }
}
