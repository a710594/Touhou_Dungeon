using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculaterUI : MonoBehaviour
{
    private enum TypeEnum
    {
        Player = 0,
        Enemy,
    }

    public Button CalculateButton;
    public Text ResultLabel;
    public InputField SkillInputField;
    public Dropdown SkillTypeDropDown;
    public CalculaterGroup[] CalculaterGroup;

    public void CalculateOnClick()
    {
        CalculaterGroup[0].SetData();
        CalculaterGroup[1].SetData();
        //Skill skill = SkillFactory.GetNewSkill(int.Parse(SkillInputField.text), CalculaterGroup[0].Info, 1); //等級暫時填1
        AttackSkill skill = new AttackSkill(Convert.ToBoolean(SkillTypeDropDown.value), int.Parse(SkillInputField.text));
        int damage =  skill.CalculateDamage(CalculaterGroup[0].Info, CalculaterGroup[1].Info, false);
        ResultLabel.text = CalculaterGroup[0].Info.Name + " 對 " + CalculaterGroup[1].Info.Name + " 造成了 " + damage + " 傷害";
    }

    private void Awake()
    {
        JobData.Load();
        EnemyData.Load();
        SkillData.Load();
        ItemData.Load();
        EquipData.Load();

        CalculateButton.onClick.AddListener(CalculateOnClick);
    }
}
