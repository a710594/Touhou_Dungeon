using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoUI : MonoBehaviour
{
    public Button Button;

    private void OnClick() 
    {
        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore)
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
                BattleController.Instance.InitFromMemo();
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
