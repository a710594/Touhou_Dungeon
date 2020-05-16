using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Single : AI
{
    protected override void SelectSkill()
    {
        _character.SelectedSkill = _skillList[0];
    }
}
