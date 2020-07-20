using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : BattleStatus
{
    public int Damage;

    public Poison(int id, int lv)
    {
        int value = 0;
        Data = BattleStatusData.GetData(id);
        if (Data.ValueList.Count > lv)
        {
            value = Data.ValueList[lv - 1];
        }
        else
        {
            value = Data.ValueList[0];
        }

        Damage = value;
        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Message = Data.Message;
        Icon = Data.Icon;
    }
}
