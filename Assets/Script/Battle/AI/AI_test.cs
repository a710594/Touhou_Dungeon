using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_test : AI
{
    protected override IEnumerator Run()
    {
        //_character.SelectedSkill = _skillList[0];
        //_character.SelectNearestTarget();
        //_character.GetPath();
        //while (_character.CanMove() && !_character.IsArriveGoal() && !_character.InSkillDistance())
        //{
        //    _character.Move();
        //    yield return new WaitForSeconds(0.2f);
        //}

        //if (_character.InSkillDistance())
        //{
        //    _character.GetSkillRange();
        //    yield return new WaitForSeconds(0.5f);
        //}

        _character.EndAI();

        yield return null;
    }
}
