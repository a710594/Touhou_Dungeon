using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEvent1 : Event
{
    public class Result1 : Result
    {
        public Result1()
        {
            Comment = "你踏進了境界的縫隙，被傳送到了另一個地方！";
            isDoNothing = false;
        }

        public override void Execute()
        {
            ExploreController.Instance.TeleportPlayer();
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

    public TeleportEvent1()
    {
        Tile = "Gap";
        Comment = "你看到了一個境界的縫隙，不知會通往何方。";
        Options.Add("進入縫隙");
        Options.Add("不做任何事");
        Type = TypeEnum.Telepoet;
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
