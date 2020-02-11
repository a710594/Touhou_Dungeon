using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralysis : BattleStatus
{
    public int Probability;

    public Paralysis(int id)
    {
        Data = BattleStatusData.GetData(id);

        Probability = Data.Probability;
        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Icon = Data.Icon;
    }
}
