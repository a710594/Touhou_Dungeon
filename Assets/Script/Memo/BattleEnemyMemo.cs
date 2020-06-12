using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemyMemo
{
    public int ID;
    public int Lv;
    public int CurrentHP;
    public int QueueIndex;
    public Vector3 Position;

    public Dictionary<int, BattleStatus> StatusDic;
    public int SleepingId;
    public float ParalysisProbability;
    public Dictionary<int, int> PoisonDic;

    public BattleEnemyMemo() { }

    public BattleEnemyMemo(BattleCharacterAI ai) 
    {
        ID = ai.Info.ID;
        Lv = ai.Info.Lv;
        CurrentHP = ai.Info.CurrentHP;
        Position = ai.transform.position;
        StatusDic = ai.Info.StatusDic;
        SleepingId = ai.Info.SleepingId;
        ParalysisProbability = ai.Info.ParalysisProbability;
        PoisonDic = ai.Info.PoisonDic;
    }
}
