using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillScrollItem : ScrollItem
{
    public Image Icon;
    public Image Mask;
    public Text NameLabel;
    public Text AmountLabel;

    public bool CanUse;
    public Skill Skill;
    public string NotUseReason;

    public override void SetData(object obj)
    {
        Skill = (Skill)obj;
        _data = this;
        Icon.overrideSprite = Resources.Load<Sprite>("Image/Skill/" + Skill.Data.Icon);
        //Label.text = Skill.Data.GetName();
        if (Skill.ItemID != 0)
        {
            AmountLabel.text = ItemManager.Instance.GetItemAmount(Skill.ItemID, ItemManager.Type.Bag).ToString();
        }
        else
        {
            AmountLabel.text = string.Empty;
        }

        CanUse = Skill.CanUse(out NotUseReason);
        Mask.gameObject.SetActive(!CanUse);
    }
}
