﻿using System.Collections;
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
        else if (skillData.Type == SkillData.TypeEnum.Sleeping)
        {
            skill = new SleepSkill(skillData, user, lv);
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
        else if (skillData.Type == SkillData.TypeEnum.Train)
        {
            skill = new TrainSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.Donothing)
        {
            skill = new DonothingSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.RecoverMP)
        {
            skill = new RecoverMPItemSkill(skillData, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.RageAttack)
        {
            skill = new RageAttackSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.AbnormalAttack)
        {
            skill = new AbnormalAttackSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.CancelAction)
        {
            skill = new CancelActionSkill(skillData, user, lv);
        }
        else if (skillData.Type == SkillData.TypeEnum.CureMySelf)
        {
            skill = new CureMyselfSkill(skillData, user, lv);
        }

        return skill;
    }
}
