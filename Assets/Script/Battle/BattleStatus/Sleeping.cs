using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleeping : BattleStatus
{
    public Sleeping(int id)
    {
        Data = BattleStatusData.GetData(id);

        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Icon = Data.Icon;
    }
}
