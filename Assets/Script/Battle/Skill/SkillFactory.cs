using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFactory
{
    public static Skill GetNewSkill(int id, BattleCharacterInfo user)
    {
        return GetNewSkill(SkillData.GetData(id), user);
    }

    public static Skill GetNewSkill(SkillData.RootObject skillData, BattleCharacterInfo user)
    {
        Skill skill = new Skill();

        if (skillData.Type == SkillData.TypeEnum.Attack)
        {
            skill = new AttackSkill(skillData, user);
        }
        else if (skillData.Type == SkillData.TypeEnum.Cure)
        {
            skill = new CureSkill(skillData, user);
        }
        else if (skillData.Type == SkillData.TypeEnum.Buff)
        {
            skill = new BuffSkill(skillData, user);
        }
        else if (skillData.Type == SkillData.TypeEnum.Poison)
        {
            skill = new PoisonSkill(skillData, user);
        }
        else if (skillData.Type == SkillData.TypeEnum.Paralysis)
        {
            skill = new ParalysisSkill(skillData, user);
        }
        else if (skillData.Type == SkillData.TypeEnum.ClearAbnormal)
        {
            skill = new ClearAbnormalSkill(skillData, user);
        }
        else if (skillData.Type == SkillData.TypeEnum.Field)
        {
            skill = new FieldSkill(skillData, user);
        }
        else if (skillData.Type == SkillData.TypeEnum.CureItem)
        {
            skill = new CureItemSkill(skillData);
        }
        else if (skillData.Type == SkillData.TypeEnum.Striking)
        {
            skill = new StrikingSkill(skillData, user);
        }
        else if (skillData.Type == SkillData.TypeEnum.CureLeastHP)
        {
            skill = new CureLeastHPSkill(skillData, user);
        }

        return skill;
    }
}
