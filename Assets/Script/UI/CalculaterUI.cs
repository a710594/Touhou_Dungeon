﻿using System.Collections;
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
    public Text SkillLabel;
    public Text ResultLabel;
    public InputField SkillInputField;
    public CalculaterGroup[] CalculaterGroup;

    public void CalculateOnClick()
    {
        CalculaterGroup[0].SetData();
        CalculaterGroup[1].SetData();
        Skill skill = SkillFactory.GetNewSkill(int.Parse(SkillInputField.text));
        SkillLabel.text = skill.Data.GetName();
        int damage = ((AttackSkill)skill).CalculateDamage(CalculaterGroup[0].Info, CalculaterGroup[1].Info, false);
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