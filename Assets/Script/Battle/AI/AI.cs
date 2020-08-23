using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [HideInInspector]
    public bool CanHitTarget = false;

    protected Skill _selectedSkill;
    protected BattleCharacter _myself;
    protected List<Skill> _skillList = new List<Skill>();

    public void Init(BattleCharacter character, List<int> list)
    {
        _myself = character;

        for (int i = 0; i < list.Count; i++)
        {
            _skillList.Add(SkillFactory.GetNewSkill(list[i], _myself.Info, 1)); //等級填1是暫時的
        }
        _myself.SelectedSkill = _skillList[0];
    }

    public virtual void StartAI(Action callback)
    {
    }
}
