using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFactory
{
    public static Skill GetNewSkill(int id, BattleCharacterInfo user, int lv)
    {
        return GetNewSkill(SkillData.GetData(id), user, lv);
    }

    public static Skill GetNewSkill(SkillData.RootObject skillData, BattleCharacterInfo user, int lv)
    {
        Skill skill = new Skill();

        if (skillData.Type == SkillData.TypeEnum.Attack)
        {
            skill = new AttackSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.Cure)
        {
            skill = new CureSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.Buff)
        {
            skill = new BuffSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.Poison)
        {
            skill = new PoisonSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.Paralysis)
        {
            skill = new ParalysisSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.ClearAbnormal)
        {
            skill = new ClearAbnormalSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.Field)
        {
            skill = new FieldSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.CureItem)
        {
            skill = new CureItemSkill(skillData, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.Striking)
        {
            skill = new StrikingSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.CureLeastHP)
        {
            skill = new CureLeastHPSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.Summon)
        {
            skill = new SummonSkill(skillData, user, lv);
        }

        return skill;
    }
}
