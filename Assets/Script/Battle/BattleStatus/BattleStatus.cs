using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatus
{
    public BattleStatusData.RootObject Data;
    public int RemainTurn;
    public string Comment;
    public string Message;
    public string Icon;

    public void ResetTurn()
    {
        RemainTurn = Data.Turn;
    }
}
