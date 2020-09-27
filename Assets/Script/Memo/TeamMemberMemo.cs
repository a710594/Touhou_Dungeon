using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMemberMemo
{
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
    public Vector2Int Formation;
    public int DataId;
    public Equip Weapon;
    public Equip Armor;
    public FoodBuff FoodBuff; //食物的效果一次只會有一個,戰鬥結束後就會消失
    public Dictionary<int, int> SkillList; //id, lv
    public Dictionary<int, int> SpellCardList; //id, lv

    public TeamMemberMemo() { }

    public TeamMemberMemo(TeamMember member) 
    {
        MaxHP = member.MaxHP;
        CurrentHP = member.CurrentHP;
        MaxMP = member.MaxMP;
        CurrentMP = member.CurrentMP;
        ATK = member._atk;
        DEF = member._def;
        MTK = member._mtk;
        MEF = member._mef;
        AGI = member._agi;
        SEN = member._sen;
        MOV = member.MOV;
        Formation = member.Formation;
        DataId = member.Data.ID;
        Weapon = member.Weapon;
        Armor = member.Armor;
        FoodBuff = member.FoodBuff;
        SkillList = member.SkillDic;
        SpellCardList = member.SpellCardDic;
    }

}
