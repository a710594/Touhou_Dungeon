using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager
{
    private static TeamManager _instance;
    public static TeamManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TeamManager();
            }
            return _instance;
        }
    }

    public int Power
    {
        get
        {
            return _power;
        }
    }
    private int _power;

    public List<TeamMember> MemberList = new List<TeamMember>();

    public void Init()
    {
        TeamMemo memo = Caretaker.Instance.Load<TeamMemo>();

        if (memo == null)
        {
            TeamScriptableObject data = Resources.Load<TeamScriptableObject>("ScriptableObject/TeamScriptableObject");

            TeamMember member;
            for (int i = 0; i < data.MemberList.Count; i++)
            {
                member = new TeamMember();
                member.Init(data.MemberList[i], 6);
                member.Formation = data.PositionList[i];
                MemberList.Add(member);
            }

            //temp
            //MemberList[0].SetEquip(42001);
            //MemberList[0].SetEquip(41001);
            //MemberList[1].SetEquip(42001);
            //MemberList[1].SetEquip(41002);
            //MemberList[2].SetEquip(42001);
            //MemberList[2].SetEquip(41002);

            MemberList[0].SetEquip(42002);
            MemberList[0].SetEquip(41002);
            MemberList[1].SetEquip(42002);
            MemberList[1].SetEquip(41004);
            MemberList[2].SetEquip(42002);
            MemberList[2].SetEquip(41004);
        }
        else
        {
            _power = memo.Power;
            TeamMember member;
            for (int i = 0; i < memo.MemberList.Count; i++)
            {
                member = new TeamMember();
                member.Init(memo.MemberList[i]);
                MemberList.Add(member);
            }
        }
    }

    public void Save() 
    {
        TeamMemo memo = new TeamMemo();
        memo.Power = _power;
        memo.MemberList = new List<TeamMemberMemo>();
        for (int i=0; i<MemberList.Count; i++) 
        {
            memo.MemberList.Add(new TeamMemberMemo(MemberList[i]));
        }
        Caretaker.Instance.Save<TeamMemo>(memo);
    }

    public void Refresh(int power, List<BattleCharacter> list)
    {
        _power = power;

        for (int i = 0; i < list.Count; i++)
        {
            MemberList[i].Refresh(list[i]);
            MemberList[i].ClearFoodBuff();
        }
    }

    public void AddExp(int exp)
    {
        for (int i = 0; i < MemberList.Count; i++)
        {
            UpgradeManager.Instance.AddExp(MemberList[i], exp);
        }
    }

    public List<int> GetLvList()
    {
        List<int> lvList = new List<int>();
        for (int i=0; i<MemberList.Count; i++)
        {
            lvList.Add(MemberList[i].Lv);
        }
        return lvList;
    }

    public List<int> GetExpList()
    {
        List<int> expList = new List<int>();
        for (int i = 0; i < MemberList.Count; i++)
        {
            expList.Add(MemberList[i].Exp);
        }
        return expList;
    }

    public void RecoverAllMember() 
    {
        _power = 0;
        for (int i = 0; i < MemberList.Count; i++)
        {
            MemberList[i].RecoverCompletelyHP();
            MemberList[i].RecoverCompletelyMP();
        }
    }
}
