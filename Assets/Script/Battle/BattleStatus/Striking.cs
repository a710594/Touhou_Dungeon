using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striking : BattleStatus
{
    public Striking(int id)
    {
        Data = BattleStatusData.GetData(id);

        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Message = Data.Message;
        Icon = Data.Icon;
    }
}
