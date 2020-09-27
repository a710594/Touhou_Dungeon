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
                BattleController.Instance.InitHandler = BattleInit;
                BattleController.Instance.SpecialInit(() =>
                {
                    AudioSystem.Instance.Stop(false);
                    Plot_2 plot_2 = new Plot_2();
                    plot_2.Start();
                }, () =>
                {
                    AudioSystem.Instance.Stop(false);
                    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Logo);
                });
            });
        });
    }

    private void BattleInit()
    {
        GameObject obj;
        obj = GameObject.Find("Reimu");
        obj.transform.position = Vector3.one * 100;
        obj.GetComponent<BattleCharacter>().SetActive(false);

        obj = GameObject.Find("Sanae");
        obj.transform.position = Vector3.one * 100;
        obj.GetComponent<BattleCharacter>().SetActive(false);

        BattleController.Instance.TurnStartHandler += OpenBattleTutorialUI;
    }

    private void OpenBattleTutorialUI()
    {
        BattleTutorialUI.Open();
        BattleController.Instance.TurnStartHandler -= OpenBattleTutorialUI;
    }
}