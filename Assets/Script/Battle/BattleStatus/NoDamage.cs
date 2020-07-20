using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使攻擊的傷害無效化
public class NoDamage : BattleStatus
{
    public NoDamage(int id)
    {
        Data = BattleStatusData.GetData(id);

        RemainTurn = Data.Turn;
        Comment = Data.Comment;
        Message = Data.Message;
        Icon = Data.Icon;
    }
}
