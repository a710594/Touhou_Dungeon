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

        return skill;
    }
}
