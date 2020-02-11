using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager
{
    private static UpgradeManager _instance;
    public static UpgradeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UpgradeManager();
            }
            return _instance;
        }
    }

    public void AddExp(TeamMember member, int getExp)
    {
        int originalLv = member.Lv;
        int originalExp = member.Exp;
        int tempLv;
        int needExp;

        needExp = NeedExp(originalLv);
        if (getExp < needExp - originalExp)
        {
            member.Lv = originalLv;
            member.Exp = originalExp + getExp;
        }
        else
        {
            getExp -= (needExp - originalExp);
            tempLv = originalLv + 1;
            needExp = NeedExp(tempLv);
            while (getExp >= needExp)
            {
                getExp -= needExp;
                tempLv++;
                needExp = NeedExp(tempLv);
            }
            member.LvUp(tempLv, getExp);
        }
    }

    public int NeedExp(int lv)
    {
        return (int)Mathf.Pow(lv + 1, 2);
    }
}
