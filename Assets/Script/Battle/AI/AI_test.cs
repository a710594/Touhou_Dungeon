using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_test : AI
{
    protected override void SelectSkill()
    {
        _character.SelectedSkill = SkillFactory.GetNewSkill(1); //temp
    }
}
