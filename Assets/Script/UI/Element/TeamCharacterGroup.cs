using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamCharacterGroup : MonoBehaviour
{
    public Text CharacterNameLabel;
    public Text LvLabel;
    public Text FoodBuffLabel;
    public Text TipLabel;
    public TextButton ATKLabel;
    public TextButton DEFLabel;
    public TextButton MTKLabel;
    public TextButton MEFLabel;
    public TextButton AGILabel;
    public TextButton SENLabel;
    public TextButton MOVLabel;
    public ValueBar HPBar;
    public ValueBar MPBar;
    public ValueBar EXPBar;
    public Image CharacterImage;
    public Button CloseTipButton;

    public void SetData(TeamMember member)
    {
        CharacterNameLabel.text = member.Data.GetName();
        LvLabel.text = "Lv." + TeamManager.Instance.Lv.ToString();
        HPBar.SetValue(member.CurrentHP, member.MaxHP);
        MPBar.SetValue(member.CurrentMP, member.MaxMP);
        EXPBar.SetValue(TeamManager.Instance.Exp, TeamManager.Instance.NeedExp(TeamManager.Instance.Lv));
        ATKLabel.Label.text = "力量：" + member.ATK.ToString();
        DEFLabel.Label.text = "體質：" + member.DEF.ToString();
        MTKLabel.Label.text = "智力：" + member.MTK.ToString();
        MEFLabel.Label.text = "意志：" + member.MEF.ToString();
        AGILabel.Label.text = "敏捷：" + member.AGI.ToString();
        SENLabel.Label.text = "感知：" + member.SEN.ToString();
        MOVLabel.Label.text = "移動：" + member.MOV.ToString();
        CharacterImage.overrideSprite = Resources.Load<Sprite>("Image/Character/Origin/" + member.Data.Image);
        //CharacterImage.SetNativeSize();

        FoodBuffLabel.text = "料理效果：\n" + member.FoodBuff.Comment;
    }

    private void ATKOnClick(object obj)
    {
        TipLabel.transform.parent.gameObject.SetActive(true);
        TipLabel.text = "影響攻擊力";
        CloseTipButton.gameObject.SetActive(true);
    }

    private void DEFOnClick(object obj)
    {
        TipLabel.transform.parent.gameObject.SetActive(true);
        TipLabel.text = "影響防禦力";
        CloseTipButton.gameObject.SetActive(true);
    }

    private void MTKOnClick(object obj)
    {
        TipLabel.transform.parent.gameObject.SetActive(true);
        TipLabel.text = "影響魔法攻擊力";
        CloseTipButton.gameObject.SetActive(true);
    }

    private void MEFOnClick(object obj)
    {
        TipLabel.transform.parent.gameObject.SetActive(true);
        TipLabel.text = "影響魔法防禦力";
        CloseTipButton.gameObject.SetActive(true);
    }

    private void AGIOnClick(object obj)
    {
        TipLabel.transform.parent.gameObject.SetActive(true);
        TipLabel.text = "影響行動順序、迴避率、被爆擊率";
        CloseTipButton.gameObject.SetActive(true);
    }

    private void SENOnClick(object obj)
    {
        TipLabel.transform.parent.gameObject.SetActive(true);
        TipLabel.text = "影響命中率與爆擊率";
        CloseTipButton.gameObject.SetActive(true);
    }

    private void MOVOnClick(object obj)
    {
        TipLabel.transform.parent.gameObject.SetActive(true);
        TipLabel.text = "影響移動距離";
        CloseTipButton.gameObject.SetActive(true);
    }

    private void CloseTipOnClick()
    {
        TipLabel.transform.parent.gameObject.SetActive(false);
        CloseTipButton.gameObject.SetActive(false);
    }

    private void Awake()
    {
        ATKLabel.OnClickHandler = ATKOnClick;
        DEFLabel.OnClickHandler = DEFOnClick;
        MTKLabel.OnClickHandler = MTKOnClick;
        MEFLabel.OnClickHandler = MEFOnClick;
        AGILabel.OnClickHandler = AGIOnClick;
        SENLabel.OnClickHandler = SENOnClick;
        MOVLabel.OnClickHandler = MOVOnClick;
        CloseTipButton.onClick.AddListener(CloseTipOnClick); 
    }
}
