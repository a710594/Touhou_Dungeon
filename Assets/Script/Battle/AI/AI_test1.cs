using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_test1 : AI
{
    protected override void SelectSkill()
    {
        _character.SelectedSkill = SkillFactory.GetNewSkill(16); //temp
    }
}
