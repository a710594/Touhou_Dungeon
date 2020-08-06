using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterMemo
{
    public bool IsActive;
    public bool IsAI;
    public bool IsTeamMember;
    public int Lv;
    public int JobID;
    public int EnemyID;
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
    public int Camp;
    public int CurrentPriority;
    public bool HasUseSkill;
    public FoodBuff FoodBuff;
    public List<int> SkillList = new List<int>();
    public List<int> SkillCdList = new List<int>();
    public List<int> SkillLvList = new List<int>();
    public List<int> SpellCardList = new List<int>();
    public List<int> SpellCardCdList = new List<int>();
    public List<int> SpellCardLvList = new List<int>();

    public Dictionary<int, BattleStatus> StatusDic;
    public int SleepingId = -1;
    public int StrikingId = -1;
    public Dictionary<int, int> ParalysisDic;
    public Dictionary<int, int> PoisonDic;

    public bool IsSelected;
    //public int QueueIndex = -1;
    public List<int> QueueIndexList = new List<int>(); //actionQueue index
    public List<int> PriorityList = new List<int>(); //技能的優先值
    public Vector3 Position;

    public BattleCharacterMemo() { }

    public BattleCharacterMemo(BattleCharacterInfo info) 
    {
        IsActive = info.IsActive;
        IsAI = info.IsAI;
        IsTeamMember = info.IsTeamMember;
        Lv = info.Lv;
        if (info.JobData != null)
        {
            JobID = info.JobData.ID;
        }
        if (info.EnemyData != null)
        {
            EnemyID = info.EnemyData.ID;
        }
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
        Camp = (int)info.Camp;
        CurrentPriority = info.CurrentPriority;
        HasUseSkill = info.HasUseSkill;
        FoodBuff = info.FoodBuff;
        for (int i=0; i<info.SkillList.Count; i++) 
        {
            SkillList.Add(info.SkillList[i].Data.ID);
            SkillCdList.Add(info.SkillList[i].CurrentCD);
            SkillLvList.Add(info.SkillList[i].Lv);
        }
        for (int i = 0; i < info.SpellCardList.Count; i++)
        {
            SpellCardList.Add(info.SpellCardList[i].Data.ID);
            SpellCardCdList.Add(info.SpellCardList[i].CurrentCD);
            SpellCardLvList.Add(info.SpellCardList[i].Lv);
        }
        StatusDic = info.StatusDic;
        SleepingId = info.SleepingId;
        StrikingId = info.StrikingId;
        ParalysisDic = info.ParalysisDic;
        PoisonDic = info.PoisonDic;
        Position = info.Position;
    }
}
