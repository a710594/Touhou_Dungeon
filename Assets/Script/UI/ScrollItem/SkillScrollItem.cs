using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillScrollItem : ScrollItem
{
    public Image Icon;
    public Text Label;

    public bool CanUse;
    public Skill Skill;
    public string NotUseReason;

    public override void SetData(object obj)
    {
        Skill = (Skill)obj;
        _data = this;
        //Icon.overrideSprite = Resources.Load<Sprite>("Image/" + skill.Data.Icon);
        Label.text = Skill.Data.Name;

        CanUse = Skill.CanUse(((BattleCharacterPlayer)BattleController.Instance.SelectedCharacter).CurrentMP, out NotUseReason);
        if (CanUse)
        {
            Background.color = Color.white;
        }
        else
        {
            Background.color = Color.gray;
        }
    }
}
