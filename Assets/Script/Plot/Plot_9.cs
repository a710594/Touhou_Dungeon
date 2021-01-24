using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_9 : Plot //第二場BOSS戰前的對話
{
    public override void Start()
    {
        if (ProgressManager.Instance.Memo.BOSS_2_Flag)
        {
            return;
        }

        ExploreUI.Instance.SetVisible(false);
        ExploreController.Instance.PlayerPause();
        ExploreController.Instance.Write();
        Camera.main.transform.DOMoveY(32, 2).OnComplete(() =>
        {
            ConversationUI.Open(12001, false, () =>
            {
                ChangeSceneUI.Instance.StartClock(() =>
                {
                    AudioSystem.Instance.Stop(true);
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle_BOSS_2, () =>
                    {
                        Plot_10 plot_10 = new Plot_10();
                        BattleController.Instance.TurnStartHandler += plot_10.Start;

                        BattleController.Instance.SpecialInit(() =>
                        {
                            ProgressManager.Instance.Memo.BOSS_2_Flag = true;
                            AudioSystem.Instance.Stop(false);
                            MySceneManager.Instance.ChangeScene(MySceneManager.Instance.LastScene, () =>
                            {
                                ExploreController.Instance.SetFloorFromMemo();
                                ExploreUI.Instance.SetVisible(false);
                                ConversationUI.Open(18001, false, () =>
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
            ConversationUI.Instance.Handler = OnConversationHandler;
        });
    }

    private void OnConversationHandler(int id)
    {
        if (id == 12010)
        {
            Camera.main.transform.DOMoveY(34, 1);
        }
        else if (id == 12015)
        {
            SpriteRenderer sprite = GameObject.Find("uuz").GetComponent<SpriteRenderer>();
            sprite.DOColor(Color.white, 1);
        }
    }
}
