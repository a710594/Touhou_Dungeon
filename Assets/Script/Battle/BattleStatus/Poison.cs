using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : BattleStatus
{
    public int Damage;

    public Poison(int id)
    {
        Data = BattleStatusData.GetData(id);

        Damage = Data.Damage;
        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Icon = Data.Icon;
    }
}
