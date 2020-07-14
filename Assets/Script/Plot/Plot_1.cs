using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_1 //遊戲的第一個事件,對話後進行新手教學戰鬥
{
    public void Run()
    {
        KeyValuePair<bool, int> flag = ProgressManager.Instance.Memo.FlagList[0];

        ConversationUI.Open(flag.Value, () =>
        {
            BattleController.Instance.Init(1, BattleGroupData.GetData(0), () =>
            {
                MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
                {
                    Plot_2 plot_2 = new Plot_2();
                    plot_2.Start();
                });
            },()=> 
            {
                MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Logo);
            });
            flag = new KeyValuePair<bool, int>(true, flag.Value);
            ProgressManager.Instance.Memo.FlagList[0] = new KeyValuePair<bool, int>(true, flag.Value); ;
        });
    }
}
