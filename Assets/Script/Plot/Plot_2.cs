using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_2 //新手教學戰鬥後的對話,對話結束後到探索場景
{
    public void Start()
    {
        KeyValuePair<bool, int> flag = ProgressManager.Instance.Memo.FlagList[1];

        ConversationUI.Open(flag.Value, () =>
        {
            TeamManager.Instance.RecoverAllMember();
            ItemManager.Instance.AddItem(0, 1, ItemManager.Type.Bag);
            LoadingUI.Instance.Open(() =>
            {
                ExploreController.Instance.GenerateFloor(1);
            });
            flag = new KeyValuePair<bool, int>(true, flag.Value);
            ProgressManager.Instance.Memo.FlagList[1] = new KeyValuePair<bool, int>(true, flag.Value); ;
        });
    }
}
