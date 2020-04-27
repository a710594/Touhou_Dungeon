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
            TeamMember teamMember = new TeamMember();
            teamMember.Init(int.Parse(IDInputField.text), int.Parse(LvInputField.text));
            Info.Init(teamMember);
        }
        else if (type == TypeEnum.Enemy)
        {
            Info.Init(int.Parse(IDInputField.text), int.Parse(LvInputField.text));
        }

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
}
