using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Single : AI
{
    protected override void SelectSkill()
    {
        _selectedSkill = _skillList[0];
        _character.SelectedSkill = _selectedSkill;
    }
}
