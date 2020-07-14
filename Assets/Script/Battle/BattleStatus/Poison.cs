using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : BattleStatus
{
    public int Damage;

    public Poison(int id)
    {
        Data = BattleStatusData.GetData(id);

        Damage = Data.Value;
        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Message = Data.Message;
        Icon = Data.Icon;
    }
}
