using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoUI : MonoBehaviour
{
    public Button Button;

    private void OnClick() 
    {
        /*if (!ProgressManager.Instance.Memo.FlagList[0].Key) //尚未結束第一個對話
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle, () =>
            {
                Plot_1 plot_1 = new Plot_1();
                plot_1.Start();
            });
        }
        else */if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore)
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
            {
                ExploreController.Instance.SetFloorFromMemo();
            });
        }
        else if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Battle)
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle, () =>
            {
                if (ProgressManager.Instance.Memo.FlagList[0].Key &&!ProgressManager.Instance.Memo.FlagList[1].Key) //尚未結束新手教學後的對話
                {
                    BattleController.Instance.InitFromMemo(() =>
                    {
                        MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
                        {
                            Plot_2 plot_2 = new Plot_2();
                            plot_2.Start();
                        });
                    }, () =>
                    {
                        MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Logo);
                    });
                }
                else
                {
                    BattleController.Instance.InitFromMemo();
                }
            });
        }
        else
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage);
        }
    }

    private void Awake()
    {
        Button.onClick.AddListener(OnClick);
    }
}
