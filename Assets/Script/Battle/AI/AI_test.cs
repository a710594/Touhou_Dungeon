using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_test : AI
{
    protected override void SelectSkill()
    {
        _character.SelectedSkill = _skillList[0]; //temp
    }
}
