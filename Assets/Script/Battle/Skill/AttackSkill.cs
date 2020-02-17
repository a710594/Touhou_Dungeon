using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkill : Skill
{
    public AttackSkill(SkillData.RootObject data)
    {
        Data = data;
    }

    public override void Use(BattleCharacter executor, Action callback)
    {
        base.Use(executor, callback);

        GameObject particle;
        for (int i = 0; i < _skillRangeList.Count; i++)
        {
            particle = ResourceManager.Instance.Spawn("Particle/" + Data.ParticleName, ResourceManager.Type.Other);
            particle.transform.position = _skillRangeList[i] + Vector2.up; // + Vector2.up 是為了調整特效生成的位置
        }

        for (int i = 0; i < _targetList.Count; i++)
        {
            SetEffect(_targetList[i]);
        }

        if (_targetList.Count == 0)
        {
            OnSkillEnd();
        }
    }

    public override void SetEffect(BattleCharacter target)
    {
        base.SetEffect(target);

        target.SetDamage(_executor, Data, CheckSkillCallback);
    }
}
