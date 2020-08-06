using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_3 : Plot
{
    public override void Start()
    {
        ExploreUI.Close();
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
                        BattleController.Instance.SpecialInit(null, null);
                    });
                });
            });
        });
    }
}
