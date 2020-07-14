using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralysis : BattleStatus
{
    public int Probability;

    public Paralysis(int id)
    {
        Data = BattleStatusData.GetData(id);

        Probability = Data.Value;
        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Message = Data.Message;
        Icon = Data.Icon;
    }
}
