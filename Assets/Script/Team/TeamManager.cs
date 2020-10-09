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

    public int Lv;
    public int Exp;
    public List<TeamMember> MemberList = new List<TeamMember>();

    public void Init()
    {
        TeamMemo memo = Caretaker.Instance.Load<TeamMemo>();

        //if (memo == null)
        //{

            Lv = 7;
            Exp = 0;

            TeamScriptableObject data = Resources.Load<TeamScriptableObject>("ScriptableObject/TeamScriptableObject");

            TeamMember member;
            for (int i = 0; i < data.MemberList.Count; i++)
            {
                member = new TeamMember();
                member.Init(data.MemberList[i], Lv);
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
            MemberList[0].SetEquip(41003);
            MemberList[1].SetEquip(42002);
            MemberList[1].SetEquip(41004);
            MemberList[2].SetEquip(42002);
            MemberList[2].SetEquip(41004);
            MemberList[3].SetEquip(42002);
            MemberList[3].SetEquip(41003);
        //}
        //else
        //{
        //    Lv = memo.Lv;
        //    Exp = memo.Exp;
        //    _power = memo.Power;
        //    TeamMember member;
        //    for (int i = 0; i < memo.MemberList.Count; i++)
        //    {
        //        member = new TeamMember();
        //        member.Init(memo.MemberList[i]);
        //        MemberList.Add(member);
        //    }
        //}
        _power = 50;
    }

    public void Save() 
    {
        TeamMemo memo = new TeamMemo();
        memo.Lv = Lv;
        memo.Exp = Exp;
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

    public void AddExp(int addExp)
    {
        int originalLv = Lv;
        int originalExp = Exp;
        int tempLv;
        int needExp;

        needExp = NeedExp(originalLv);
        if (addExp < needExp - originalExp)
        {
            Lv = originalLv;
            Exp = originalExp + addExp;
        }
        else
        {
            addExp -= (needExp - originalExp);
            tempLv = originalLv + 1;
            needExp = NeedExp(tempLv);

            if (needExp > 0)
            {
                while (addExp >= needExp)
                {
                    addExp -= needExp;
                    tempLv++;
                    needExp = NeedExp(tempLv);
                }
            }
            else //已達最大等級
            {
                addExp = 0;
                tempLv = _maxLv;
            }

            Lv = tempLv;
            Exp = addExp;
            for (int i = 0; i < MemberList.Count; i++)
            {
                MemberList[i].LvUp(tempLv, addExp);
            }
        }
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

    private static readonly int _maxLv = 99;
    public int NeedExp(int lv)
    {
        if (lv < _maxLv)
        {
            return (int)Mathf.Pow(lv + 1, 2);
        }
        else
        {
            return 0;
        }
    }
}
