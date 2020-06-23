using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstConversation //遊戲的第一個事件
{
    // Start is called before the first frame update
    public void Start()
    {
        KeyValuePair<bool, int> flag = ProgressManager.Instance.Memo.FlagList[0];

        Debug.Log("!!!" + MySceneManager.Instance.CurrentScene);
        ConversationUI.Open(flag.Value, () =>
        {
            BattleController.Instance.Init(1, BattleGroupData.GetData(0), () =>
            {
                MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
                {
                    TeamManager.Instance.RecoverAllMember();
                    ExploreController.Instance.GenerateFloor(1);
                });
            });
            flag = new KeyValuePair<bool, int>(true, flag.Value);
            ProgressManager.Instance.Memo.FlagList[0] = new KeyValuePair<bool, int>(true, flag.Value); ;
        });
    }
}
