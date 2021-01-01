using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_12 : Plot //打完第二關BOSS,回到村莊
{
    public override void Start()
    {
        //ConversationUI.Open(10001, true, () =>
        //{
            ExploreController.Instance.ArriveFloor = 13;
            //TeamManager.Instance.AddMember(4, new Vector2Int(0, 1), 41003, 42002);
            //ProgressManager.Instance.Memo.Stage_3_Flag = true; //等第三關寫好後再取消註解
            GameSystem.Instance.AutoSave();
            Debug.Log("敬請期待");
        //});
    }
}
