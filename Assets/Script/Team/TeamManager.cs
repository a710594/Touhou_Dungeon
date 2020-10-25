﻿using System.Collections;
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

        if (memo == null)
        {
            Lv = 12;
            Exp = 0;

            AddMember(1, new Vector2Int(0, 0), 41003, 42002);
            AddMember(2, new Vector2Int(1, 0), 41004, 42002);
            AddMember(3, new Vector2Int(-1, 0), 41004, 42002);
            AddMember(4, new Vector2Int(0, 1), 41003, 42002); //temp
        }
        else
        {
            Lv = memo.Lv;
            Exp = memo.Exp;
            _power = memo.Power;
            TeamMember member;
            for (int i = 0; i < memo.MemberList.Count; i++)
            {
                member = new TeamMember();
                member.Init(memo.MemberList[i]);
                MemberList.Add(member);
            }
        }
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

            SetLv(tempLv, addExp);
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
            return ExpData.NeedExp(lv - 1);
        }
        else
        {
            return 0;
        }
    }

    public void AddMember(int job, Vector2Int position, int weapon, int armor) 
    {
        TeamMember member = new TeamMember();
        member.Init(job, Lv);
        member.Formation = position;
        member.SetEquip(weapon);
        member.SetEquip(armor);
        MemberList.Add(member);
    }

    private void SetLv(int lv, int exp) 
    {
        Lv = lv;
        Exp = exp;
        for (int i = 0; i < MemberList.Count; i++)
        {
            MemberList[i].LvUp(lv, exp);
        }
    }
}
