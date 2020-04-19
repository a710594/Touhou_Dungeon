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

    public List<TeamMember> MemberList = new List<TeamMember>();
    public Dictionary<TeamMember, Vector2Int> MemberPositionDic = new Dictionary<TeamMember, Vector2Int>(); //隊形

    public void Init()
    {
        TeamScriptableObject data = Resources.Load<TeamScriptableObject>("ScriptableObject/TeamScriptableObject");

        TeamMember member;
        for (int i = 0; i < data.MemberList.Count; i++)
        {
            member = new TeamMember();
            member.Init(data.MemberList[i]);
            MemberList.Add(member);
            MemberPositionDic.Add(member, data.PositionList[i]);
        }
    }

    public void Refresh(List<BattleCharacterPlayer> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            MemberList[i].SetData(list[i]);
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
        for (int i = 0; i < MemberList.Count; i++)
        {
            MemberList[i].RecoverCompletelyHP();
            MemberList[i].RecoverCompletelyMP();
        }
    }
}
