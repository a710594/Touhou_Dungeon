using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_8 : Plot //第二場BOSS戰前的對話
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
            ConversationUI.Open(10001, false, () =>
            {
                ChangeSceneUI.Instance.StartClock(() =>
                {
                    AudioSystem.Instance.Stop(true);
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle_BOSS_2, () =>
                    {
                        Plot_9 plot_9 = new Plot_9();
                        BattleController.Instance.ShowEndHandler += plot_9.Start;

                        BattleController.Instance.SpecialInit(() =>
                        {
                            ProgressManager.Instance.Memo.BOSS_2_Flag = true;
                            AudioSystem.Instance.Stop(false);
                            MySceneManager.Instance.ChangeScene(MySceneManager.Instance.LastScene, () =>
                            {
                                ExploreController.Instance.SetFloorFromMemo();
                                //ExploreUI.Instance.SetVisible(false);
                                //ConversationUI.Open(9001, false, () =>
                                //{
                                //    ExploreUI.Instance.SetVisible(true);
                                //});
                            });
                        }, () =>
                        {
                            //ConversationUI.Open(8001, false, () =>
                            //{
                                AudioSystem.Instance.Stop(false);
                                MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage, () =>
                                {
                                    ItemManager.Instance.PutBagItemIntoWarehouse();
                                    TeamManager.Instance.RecoverAllMember();
                                });
                            //});
                        });
                    });
                });
            });
            //ConversationUI.Instance.Handler = OnConversationHandler;
        });
    }

    //private void OnConversationHandler(int id)
    //{
    //    if (id == 3004)
    //    {
    //        SpriteRenderer sprite = GameObject.Find("Yukari").GetComponent<SpriteRenderer>();
    //        sprite.DOColor(Color.white, 1);
    //    }
    //}
}
