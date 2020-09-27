using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
public class SkillTest
{
    [Test]
    public void TestBuff()
    {
        if (JobData.GetData(1) == null)
        {
            JobData.Load();
            BattleStatusData.Load();
        }

        BattleCharacterInfo Info = new BattleCharacterInfo();
        TeamMember teamMember = new TeamMember();
        teamMember.Init(1, 1);
        Info.Init(teamMember, 1);
        Info.SetBuff(1, 1);
        Assert.AreEqual(27, Info.DEF);
    }

    [Test]
    public void TestCureSkill()
    {
        if (SkillData.GetData(3) == null)
        {
            SkillData.Load();
        }

        CureSkill cureSkill = (CureSkill)SkillFactory.GetNewSkill(3, null, 1);
        BattleCharacterInfo Info = new BattleCharacterInfo();
        TeamMember teamMember = new TeamMember();
        teamMember.Init(3, 1);
        Info.Init(teamMember, 1);
        Debug.Log(Info.MEF + " " + cureSkill.Data.ValueList[0]);
        Assert.AreEqual(18, cureSkill.CalculateRecover(Info));
    }

    //[Test]
    //public void TestLvUp()
    //{
    //    TeamMember teamMember = new TeamMember();
    //    teamMember.Init(1, 1);
    //    UpgradeManager.Instance.AddExp(teamMember, 15);
    //    Assert.AreEqual(3, teamMember.Lv);
    //}

    [Test]
    public void TestAddMoney()
    {
        ItemManager.Instance.Money = 0;
        ItemManager.Instance.AddMoney(123);
        Assert.AreEqual(123, ItemManager.Instance.Money);
    }

    [Test]
    public void TestAddItem()
    {
        if (ItemData.GetData(1001) == null)
        {
            ItemData.Load();
            ItemManager.Instance.Init();
        }
        ItemManager.Instance.AddItem(1001, 3, ItemManager.Type.Bag);
        Assert.AreEqual(3, ItemManager.Instance.GetItemAmount(1001, ItemManager.Type.Bag));
    }
}
