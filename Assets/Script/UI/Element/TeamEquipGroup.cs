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
    public TextButton[] EquipButton;
    public Button ChangeButton;
    public Button TakeOffButton;

    private TeamMember _selectedMember;
    private Equip _selectedEquip;

    public void Init(TeamMember member)
    {
        _selectedMember = member;

        EquipButton[0].SetData(member.Weapon.Name, member.Weapon);
        EquipButton[1].SetData(member.Armor.Name, member.Armor);

        for (int i=0; i< EquipButton.Length; i++)
        {
            EquipButton[i].OnClickHandler = IconOnClick;
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

    private void IconOnClick(object obj)
    {
        _selectedEquip = (Equip)obj;
        SetData(_selectedEquip);
        ChangeButton.gameObject.SetActive(true);

        if (_selectedEquip.ID != 0)
        {
            TakeOffButton.gameObject.SetActive(true);
        }
    }

    private void ChangeOnClick()
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
        BagUI.Open(type, _selectedMember, _selectedEquip.Type);
    }

    private void TakeOffOnClick()
    {
        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Villiage)
        {
            _selectedMember.TakeOffEquip(_selectedEquip, ItemManager.Type.Warehouse);
        }
        else
        {
            _selectedMember.TakeOffEquip(_selectedEquip, ItemManager.Type.Bag);
        }
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
