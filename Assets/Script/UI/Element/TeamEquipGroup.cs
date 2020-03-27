using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamEquipGroup : MonoBehaviour
{
    public Text NameLabel;
    public Text ATKLabel;
    public Text DEFLabel;
    public Text MTKLabel;
    public Text MEFLabel;
    public EquipIcon[] Icons;
    public Button ChangeButton;
    public Button TakeOffButton;

    private TeamMember _selectedMember;
    private Equip _selectedEquip;

    public void Init(TeamMember member)
    {
        _selectedMember = member;

        Icons[0].SetData(member.Weapon);
        Icons[1].SetData(member.Helmet);
        Icons[2].SetData(member.Armor);
        Icons[3].SetData(member.Jewelry);

        for (int i=0; i<Icons.Length; i++)
        {
            Icons[i].OnClickHandler = IconOnClick;
            Icons[i].SetSelect(false);
        }

        ClearInfo();
    }

    private void SetData(Equip equip)
    {
        if (equip.ID != 0)
        {
            NameLabel.text = equip.Name;
            ATKLabel.text = "攻擊：" + equip.ATK.ToString();
            DEFLabel.text = "防禦：" + equip.DEF.ToString();
            MTKLabel.text = "魔攻：" + equip.MTK.ToString();
            MEFLabel.text = "魔防：" + equip.MEF.ToString();
        }
        else
        {
            ClearInfo();
        }
    }

    private void ClearInfo()
    {
        NameLabel.text = string.Empty;
        ATKLabel.text = string.Empty;
        DEFLabel.text = string.Empty;
        MTKLabel.text = string.Empty;
        MEFLabel.text = string.Empty;
        ChangeButton.gameObject.SetActive(false);
        TakeOffButton.gameObject.SetActive(false);
    }

    private void IconOnClick(Equip equip)
    {
        _selectedEquip = equip;
        SetData(equip);
        ChangeButton.gameObject.SetActive(true);

        if (equip.ID != 0)
        {
            TakeOffButton.gameObject.SetActive(true);
        }
    }

    private void ChangeOnClick()
    {
        BagUI.Open(ItemManager.Type.Bag, _selectedMember, ItemManager.Instance.GetEquipListByType(ItemManager.Type.Bag, _selectedEquip.Type));
    }

    private void TakeOffOnClick()
    {
        _selectedMember.TakeOffEquip(_selectedEquip, ItemManager.Type.Bag);
        Init(_selectedMember);
        TakeOffButton.gameObject.SetActive(false);
    }

    private void Awake()
    {
        ClearInfo();
        ChangeButton.onClick.AddListener(ChangeOnClick);
        TakeOffButton.onClick.AddListener(TakeOffOnClick);
    }
}
