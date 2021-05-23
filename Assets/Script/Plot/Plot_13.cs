using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_13 : Plot
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
            ConversationUI.Open(3001, false, () =>
            {
                ChangeSceneUI.Instance.StartClock(() =>
                {
                    AudioSystem.Instance.Stop(true);
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle_BOSS_3, () =>
                    {
                        Plot_14 plot_14 = new Plot_14();
                        BattleController.Instance.TurnStartHandler += plot_14.Start;
                        BattleController.Instance.SpecialInit(() =>
                        {
                            ProgressManager.Instance.Memo.BOSS_3_Flag = true;
                            AudioSystem.Instance.Stop(false);
                            MySceneManager.Instance.ChangeScene(MySceneManager.Instance.LastScene, () =>
                            {
                                ExploreController.Instance.SetFloorFromMemo();
                                ExploreUI.Instance.SetVisible(false);
                                ConversationUI.Open(9001, false, () =>
                                {
                                    ExploreUI.Instance.SetVisible(true);
                                });
                            });
                        }, () =>
                        {
                            ConversationUI.Open(8001, false, () =>
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
        });
    }
}
