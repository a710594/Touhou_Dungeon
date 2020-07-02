using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterMemo
{
    public bool IsAI = false;
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

    public BattleCharacterMemo() { }

    public BattleCharacterMemo(BattleCharacterInfo info) 
    {
        IsAI = info.IsAI;
        Lv = info.Lv;
        ID = info.ID;
        MaxHP = info.MaxHP;
        CurrentHP = info.CurrentHP;
        MaxMP = info.MaxMP;
        CurrentMP = info.CurrentMP;
        ATK = info._atk;
        DEF = info._def;
        MTK = info._mtk;
        MEF = info._mef;
        AGI = info._agi;
        SEN = info._sen;
        MOV = info._mov;
        EquipATK = info.EquipATK;
        EquipDEF = info.EquipDEF;
        EquipMTK = info.EquipMTK;
        EquipMEF = info.EquipMEF;
        ActionCount = info.ActionCount;
        HasUseSkill = info.HasUseSkill;
        FoodBuff = info.FoodBuff;
        for (int i=0; i<info.SkillList.Count; i++) 
        {
            SkillList.Add(info.SkillList[i].Data.ID);
            SkillCdList.Add(info.SkillList[i].CurrentCD);
        }
        for (int i = 0; i < info.SpellCardList.Count; i++)
        {
            SpellCardList.Add(info.SpellCardList[i].Data.ID);
        }
        StatusDic = info.StatusDic;
        SleepingId = info.SleepingId;
        ParalysisProbability = info.ParalysisProbability;
        PoisonDic = info.PoisonDic;
        Position = info.Position;
    }
}
