using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculaterGroup : MonoBehaviour
{
    private enum TypeEnum
    {
        Player = 0,
        Enemy,
    }

    public Dropdown TypeDropDown;
    public InputField IDInputField;
    public InputField LvInputField;
    public InputField EquipAtkField;
    public InputField EquipMtkField;
    public InputField EquipDefField;
    public InputField EquipMefField;
    public InputField BuffAtkField;
    public InputField BuffMtkField;
    public InputField BuffDefField;
    public InputField BuffMefField;
    public BattleCharacterInfo Info = new BattleCharacterInfo();

    public Text NameLabel;
    public Text HPLabel;
    public Text MPLabel;
    public Text ATKLabel;
    public Text DEFLabel;
    public Text MTKLabel;
    public Text MEFLabel;
    public Text AGILabel;
    public Text SENLabel;

    public void SetData()
    {
        TypeEnum type = (TypeEnum)TypeDropDown.value;
        if (type == TypeEnum.Player)
        {
            Info.Init(int.Parse(IDInputField.text), int.Parse(LvInputField.text), int.Parse(EquipAtkField.text), int.Parse(EquipMtkField.text), int.Parse(EquipDefField.text), int.Parse(EquipMefField.text));
        }
        else if (type == TypeEnum.Enemy)
        {
            Info.Init(int.Parse(IDInputField.text), int.Parse(LvInputField.text));
        }

        Info.SetBuff(int.Parse(BuffAtkField.text), int.Parse(BuffMtkField.text), int.Parse(BuffDefField.text), int.Parse(BuffMefField.text));

        NameLabel.text = Info.Name;
        HPLabel.text = "HP: " + Info.CurrentHP.ToString();
        MPLabel.text = "MP: " + Info.CurrentMP.ToString();
        ATKLabel.text = "ATK:" + Info.ATK.ToString();
        DEFLabel.text = "DEF" + Info.DEF.ToString();
        MTKLabel.text = "MTK" + Info.MTK.ToString();
        MEFLabel.text = "MEF" + Info.MEF.ToString();
        AGILabel.text = "AGI" + Info.AGI.ToString();
        SENLabel.text = "SEN" + Info.SEN.ToString();
    }

    private void TypeDropDownOnValueChange(int index) 
    {
        if (index == 0) //player
        {
            EquipAtkField.gameObject.SetActive(true);
            EquipMtkField.gameObject.SetActive(true);
            EquipDefField.gameObject.SetActive(true);
            EquipMefField.gameObject.SetActive(true);
        }
        else if(index == 1) //enemy
        {
            EquipAtkField.gameObject.SetActive(false);
            EquipMtkField.gameObject.SetActive(false);
            EquipDefField.gameObject.SetActive(false);
            EquipMefField.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        TypeDropDown.onValueChanged.AddListener(TypeDropDownOnValueChange);
    }
}
