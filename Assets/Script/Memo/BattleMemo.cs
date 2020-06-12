using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMemo
{
    //BattleFieldManager
    public BoundsInt MapBound;
    public Dictionary<string, BattleField> MapDic = new Dictionary<string, BattleField>();

    //BattleController
    public int Turn;
    public int Power;
    public int Exp;
    public int QueueLength;
    public List<BattlePlayerMemo> PlayerList = new List<BattlePlayerMemo>();
    public List<BattleEnemyMemo> EnemyList = new List<BattleEnemyMemo>();
}
