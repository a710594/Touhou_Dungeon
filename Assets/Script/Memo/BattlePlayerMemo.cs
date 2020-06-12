using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerMemo
{
    public int Lv;
    public int ID;
    public int MaxHP;
    public int CurrentHP;
    public int MaxMP;
    public int CurrentMP;
    public int ATK;
    public int DEF;
    public int MTK;
    public int MEF;
    public int AGI;
    public int SEN;
    public int MOV;
    public int EquipATK;
    public int EquipDEF;
    public int EquipMTK;
    public int EquipMEF;
    public int ActionCount;
    public bool HasUseSkill;
    public FoodBuff FoodBuff;
    public List<int> SkillList = new List<int>();
    public List<int> SkillCdList = new List<int>();
    public List<int> SpellCardList = new List<int>();

    public Dictionary<int, BattleStatus> StatusDic;
    public int SleepingId = -1;
    public float ParalysisProbability = 0;
    public Dictionary<int, int> PoisonDic;

    public bool IsSelected;
    public int QueueIndex = -1;
    public Vector3 Position;

    public BattlePlayerMemo() { }

    public BattlePlayerMemo(BattleCharacterPlayer player) 
    {
        Lv = player.Info.Lv;
        ID = player.Info.ID;
        MaxHP = player.Info.MaxHP;
        CurrentHP = player.Info.CurrentHP;
        MaxMP = player.Info.MaxMP;
        CurrentMP = player.Info.CurrentMP;
        ATK = player.Info._atk;
        DEF = player.Info._def;
        MTK = player.Info._mtk;
        MEF = player.Info._mef;
        AGI = player.Info._agi;
        SEN = player.Info._sen;
        MOV = player.Info._mov;
        EquipATK = player.Info.EquipATK;
        EquipDEF = player.Info.EquipDEF;
        EquipMTK = player.Info.EquipMTK;
        EquipMEF = player.Info.EquipMEF;
        ActionCount = player.Info.ActionCount;
        HasUseSkill = player.Info.HasUseSkill;
        FoodBuff = player.Info.FoodBuff;
        for (int i=0; i<player.SkillList.Count; i++) 
        {
            SkillList.Add(player.SkillList[i].Data.ID);
            SkillCdList.Add(player.SkillList[i].CurrentCD);
        }
        for (int i = 0; i < player.SpellCardList.Count; i++)
        {
            SpellCardList.Add(player.SpellCardList[i].Data.ID);
        }
        StatusDic = player.Info.StatusDic;
        SleepingId = player.Info.SleepingId;
        ParalysisProbability = player.Info.ParalysisProbability;
        PoisonDic = player.Info.PoisonDic;
        Position = player.transform.position;
}
}
