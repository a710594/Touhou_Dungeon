using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFactory
{
    public static Skill GetNewSkill(int id)
    {
        return GetNewSkill(SkillData.GetData(id));
    }

    public static Skill GetNewSkill(SkillData.RootObject skillData)
    {
        Skill skill = new Skill();

        if (skillData.Type == SkillData.TypeEnum.Attack)
        {
            skill = new AttackSkill(skillData);
        }
        else if (skillData.Type == SkillData.TypeEnum.Cure)
        {
            skill = new CureSkill(skillData);
        }
        else if (skillData.Type == SkillData.TypeEnum.Buff)
        {
            skill = new BuffSkill(skillData);
        }
        else if (skillData.Type == SkillData.TypeEnum.Poison)
        {
            skill = new PoisonSkill(skillData);
        }
        else if (skillData.Type == SkillData.TypeEnum.Field)
        {
            skill = new FieldSkill(skillData);
        }
        else if (skillData.Type == SkillData.TypeEnum.CureItem)
        {
            skill = new CureItemSkill(skillData);
        }

        return skill;
    }
}
