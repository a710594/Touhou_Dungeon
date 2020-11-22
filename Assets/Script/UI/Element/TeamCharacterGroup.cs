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
    public Text EquipATKLabel;
    public Text EquipDEFLabel;
    public Text EquipMTKLabel;
    public Text EquipMEFLabel;
    public TextButton ATKLabel;
    public TextButton DEFLabel;
    public TextButton MTKLabel;
    public TextButton MEFLabel;
    public TextButton AGILabel;
    public TextButton SENLabel;
    public TextButton MOVLabel;
    public TextButton WeaponButton;
    public TextButton ArmorButton;
    public ValueBar HPBar;
    public ValueBar MPBar;
    public ValueBar EXPBar;
    public Image CharacterImage;
    public Button CloseTipButton;

    private TeamMember _selectedMember;

    public void SetData(TeamMember member)
    {
        _selectedMember = member;
        CharacterNameLabel.text = member.Data.GetName();
        LvLabel.text = TeamManager.Instance.Lv.ToString();
        HPBar.SetValue(member.CurrentHP, member.MaxHP);
        MPBar.SetValue(member.CurrentMP, member.MaxMP);
        EXPBar.SetValue(TeamManager.Instance.Exp, TeamManager.Instance.NeedExp(TeamManager.Instance.Lv));
        ATKLabel.Label.text = member.ATK.ToString();
        DEFLabel.Label.text = member.DEF.ToString();
        MTKLabel.Label.text = member.MTK.ToString();
        MEFLabel.Label.text = member.MEF.ToString();
        AGILabel.Label.text = member.AGI.ToString();
        SENLabel.Label.text = member.SEN.ToString();
        MOVLabel.Label.text = member.MOV.ToString();
        WeaponButton.SetData(member.Weapon.Name, member.Weapon);
        EquipATKLabel.text = member.Weapon.ATK.ToString();
        EquipMTKLabel.text = member.Weapon.MTK.ToString();
        ArmorButton.SetData(member.Armor.Name, member.Armor);
        EquipDEFLabel.text = member.Armor.DEF.ToString();
        EquipMEFLabel.text = member.Armor.MEF.ToString();
        CharacterImage.overrideSprite = Resources.Load<Sprite>("Image/Character/Origin/" + member.Data.Image);
        CharacterImage.SetNativeSize();

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
        TipLabel.text = "影響行動順序與迴避率";
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

    private void EquipOnClick(object data)
    {
        ItemManager.Type type;
        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Villiage)
        {
            type = ItemManager.Type.Warehouse;
        }
        else
        {
            type = ItemManager.Type.Bag;
        }
        BagUI.Open(type, _selectedMember, (Equip)data);
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
        WeaponButton.OnClickHandler = EquipOnClick;
        ArmorButton.OnClickHandler = EquipOnClick;
        CloseTipButton.onClick.AddListener(CloseTipOnClick); 
    }
}
