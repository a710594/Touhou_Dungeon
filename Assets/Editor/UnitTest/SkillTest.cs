using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
public class SkillTest
{
    [Test]
    public void TestBuff()
    {
        JobData.Load();
        BattleStatusData.Load();

        BattleCharacterInfo Info = new BattleCharacterInfo();
        TeamMember teamMember = new TeamMember();
        teamMember.Init(1, 1);
        Info.Init(teamMember);
        Info.SetBuff(1);
        Assert.AreEqual(27, Info.DEF);
    }

    [Test]
    public void TestCureSkill()
    {
        SkillData.Load();

        CureSkill cureSkill = (CureSkill)SkillFactory.GetNewSkill(3);
        BattleCharacterInfo Info = new BattleCharacterInfo();
        TeamMember teamMember = new TeamMember();
        teamMember.Init(3, 1);
        Info.Init(teamMember);
        Assert.AreEqual(27, cureSkill.CalculateRecover(Info));
    }

    [Test]
    public void TestLvUp()
    {
        TeamMember teamMember = new TeamMember();
        teamMember.Init(1, 1);
        UpgradeManager.Instance.AddExp(teamMember, 15);
        Assert.AreEqual(3, teamMember.Lv);
    }
}
