using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//第三關BOSS關
public class Plot_14 : Plot
{
    public override void Start()
    {
        if (ProgressManager.Instance.Memo.BOSS_3_Flag)
        {
            return;
        }

        ExploreUI.Instance.SetVisible(false);
        ExploreController.Instance.PlayerPause();
        ExploreController.Instance.Write();
        Camera.main.transform.DOMoveY(12, 3).OnComplete(() =>
        {
            ConversationUI.Open(21001, false, () =>
            {
                ChangeSceneUI.Instance.StartClock(() =>
                {
                    AudioSystem.Instance.Stop(true);
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle_BOSS_3, () =>
                    {
                        Plot_15 plot_15 = new Plot_15();
                        BattleController.Instance.TurnStartHandler += plot_15.Start;
                        BattleController.Instance.SpecialInit(() =>
                        {
                            ProgressManager.Instance.Memo.BOSS_3_Flag = true;
                            AudioSystem.Instance.Stop(false);
                            MySceneManager.Instance.ChangeScene(MySceneManager.Instance.LastScene, () =>
                            {
                                ExploreController.Instance.SetFloorFromMemo();
                                ExploreUI.Instance.SetVisible(false);
                                ConversationUI.Open(22001, false, () =>
                                {
                                    ExploreUI.Instance.SetVisible(true);
                                });
                            });
                        }, () =>
                        {
                            AudioSystem.Instance.Stop(false);
                            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage, () =>
                            {
                                ItemManager.Instance.BagToWarehouse();
                                TeamManager.Instance.RecoverAllMember();
                            });
                        });
                    });
                });
            });
        });
    }
}
