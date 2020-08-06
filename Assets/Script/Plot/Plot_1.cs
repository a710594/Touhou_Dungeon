using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_1 : Plot //遊戲的第一個事件,對話後進行新手教學戰鬥
{
    public override void Start()
    {
        AudioSystem.Instance.Stop(false);
        ConversationUI.Open(1001, true, () =>
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.FirstBattle, () =>
            {
                BattleController.Instance.SpecialInit(() =>
                {
                    AudioSystem.Instance.Stop(false);
                    Plot_2 plot_2 = new Plot_2();
                    plot_2.Start();
                }, () =>
                {
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Logo);
                });
                ProgressManager.Instance.SetFlag(0, true);
            });
        });
    }
}