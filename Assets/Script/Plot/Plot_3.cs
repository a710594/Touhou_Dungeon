using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_3 : Plot
{
    public override void Start()
    {
        if (ProgressManager.Instance.Memo.BOSS_1_Flag)
        {
            return;
        }

        ExploreUI.Close();
        ExploreController.Instance.PlayerPause();
        ExploreController.Instance.Write();
        Camera.main.transform.DOMoveY(12, 3).OnComplete(()=> 
        {
            ConversationUI.Open(3001, false, ()=> 
            {
                ChangeSceneUI.Instance.StartClock(() =>
                {
                    AudioSystem.Instance.Stop(true);
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle_BOSS_1, () =>
                    {
                        Plot_4 plot_4 = new Plot_4();
                        BattleController.Instance.TurnStartHandler += plot_4.Start;
                        BattleController.Instance.SpecialInit(()=> 
                        {
                            ProgressManager.Instance.Memo.BOSS_1_Flag = true;
                            AudioSystem.Instance.Stop(false);
                            MySceneManager.Instance.ChangeScene(MySceneManager.Instance.LastScene, () =>
                            {
                                ExploreController.Instance.SetFloorFromMemo();
                            });
                        }, ()=>
                        {
                            AudioSystem.Instance.Stop(false);
                            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage, () =>
                            {
                                ItemManager.Instance.PutBagItemIntoWarehouse();
                                TeamManager.Instance.RecoverAllMember();
                            });
                        });
                    });
                });
            });
        });
    }
}
