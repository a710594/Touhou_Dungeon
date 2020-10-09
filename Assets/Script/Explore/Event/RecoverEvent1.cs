using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverEvent1 : Event
{
    public class Result1 : Result
    {
        public Result1() 
        {
            Comment = "你回復了全部的 HP 和 MP ！";
            isDoNothing = false;
        }

        public override void Execute()
        {
            for (int i = 0; i < TeamManager.Instance.MemberList.Count; i++)
            {
                TeamManager.Instance.MemberList[i].AddHP((int)(TeamManager.Instance.MemberList[i].MaxHP));
                TeamManager.Instance.MemberList[i].AddMP((int)(TeamManager.Instance.MemberList[i].MaxMP));
            }
            ItemManager.Instance.MinusMoney((int)(ItemManager.Instance.Money * 0.1f));
        }
    }

    public class Result2 : Result 
    {
        public Result2()
        {
            Comment = "你離開了。";
            isDoNothing = true;
        }
    }

    public RecoverEvent1() 
    {
        Tile = "Saisen";
        Comment = "你在路邊看到了一個賽錢箱，你要做什麼？";
        Options.Add("投入" + ((int)(ItemManager.Instance.Money * 0.1f)).ToString() + "元");
        Options.Add("不做任何事");
        Type = TypeEnum.Recover;
    }

    public override Result GetResult(int option)
    {
        if (option == 0)
        {
            return new Result1();
        }
        else
        {
            return new Result2();
        }
    }
}
