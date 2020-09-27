using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLvCard : MonoBehaviour
{
    public Image Image;
    public Text LvLabel;
    public Text NameLabel;
    public ValueBar ExpBar;

    private int _currentLv;
    private int _currentExp;

    public void SetData(int originalLv, int originalExp, TeamMember member)
    {
        //_currentLv = member.Lv;
        //_currentExp = member.Exp;

        NameLabel.text = member.Data.GetName();
        Image.overrideSprite = Resources.Load<Sprite>("Image/Character/Small/" + member.Data.SmallImage);
        SetExpBar(originalLv, originalExp);
    }

    private void SetExpBar(int tempLv, int tempExp)
    {
        int needExp = UpgradeManager.Instance.NeedExp(tempLv);

        LvLabel.text = "Lv." + tempLv.ToString();
        if (tempLv < _currentLv)
        {
            ExpBar.SetValueTween(tempExp, needExp, needExp, () =>
            {
                LvLabel.text = "Lv." + (tempLv + 1).ToString();
                SetExpBar(tempLv + 1, 0);
            });
        }
        else
        {
            LvLabel.text = "Lv." + _currentLv.ToString();
            ExpBar.SetValueTween(tempExp, _currentExp, needExp, null);
        }
    }
}
