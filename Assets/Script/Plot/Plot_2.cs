using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_2 //新手教學戰鬥後的對話,對話結束後到探索場景
{
    public void Start()
    {
        KeyValuePair<bool, int> flag = ProgressManager.Instance.Memo.FlagList[1];

        BattleUI.Close();
        Camera.main.transform.position = new Vector3(100, 0, -10);
        ConversationUI.Open(flag.Value, () =>
        {
            TeamManager.Instance.RecoverAllMember();
            ItemManager.Instance.AddItem(0, 1, ItemManager.Type.Bag);
            flag = new KeyValuePair<bool, int>(true, flag.Value);
            ProgressManager.Instance.Memo.FlagList[1] = new KeyValuePair<bool, int>(true, flag.Value);
            AudioSystem.Instance.Stop(true);

            AudioSystem.Instance.Play("Forest", true);
            ExploreController.Instance.GenerateFloor(1);
        });
    }
}
