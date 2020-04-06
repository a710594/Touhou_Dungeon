using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamCharacterGroup : MonoBehaviour
{
    public Text CharacterNameLabel;
    public Text LvLabel;
    public Text ATKLabel;
    public Text DEFLabel;
    public Text MTKLabel;
    public Text MEFLabel;
    public Text AGILabel;
    public Text SENLabel;
    public Text MoveDistanceLabel;
    public Text FoodBuffLabel;
    public ValueBar HPBar;
    public ValueBar MPBar;
    public ValueBar EXPBar;
    public Image CharacterImage;

    public void SetData(TeamMember member)
    {
        CharacterNameLabel.text = member.Data.GetName();
        LvLabel.text = "Lv." + member.Lv.ToString();
        HPBar.SetValue(member.CurrentHP, member.MaxHP);
        MPBar.SetValue(member.CurrentMP, member.MaxMP);
        EXPBar.SetValue(member.Exp, UpgradeManager.Instance.NeedExp(member.Lv));
        ATKLabel.text = "力量：" + member.ATK.ToString();
        DEFLabel.text = "體質：" + member.DEF.ToString();
        MTKLabel.text = "智力：" + member.MTK.ToString();
        MEFLabel.text = "意志：" + member.MEF.ToString();
        AGILabel.text = "敏捷：" + member.AGI.ToString();
        SENLabel.text = "感知：" + member.SEN.ToString();
        MoveDistanceLabel.text = "移動：" + member.MoveDistance.ToString();
        CharacterImage.overrideSprite = Resources.Load<Sprite>("Image/Character/Origin/" + member.Data.Image);
        //CharacterImage.SetNativeSize();

        if (member.HasFoodBuff)
        {
            FoodBuffLabel.text = "料理效果：\n" + member.FoodBuff.Comment;
        }
        else
        {
            FoodBuffLabel.text = "料理效果：";
        }
    }
}
