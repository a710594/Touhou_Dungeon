using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_8 : Plot
{
    public override void Start()
    {
        ExploreUI.Instance.SetVisible(false);
        ExploreController.Instance.StopEnemy();
        ConversationUI.Open(11001, false, () =>
        {
            ProgressManager.Instance.Memo.Floor7_Flag = true;
            GameSystem.Instance.AutoSave();
            ExploreUI.Instance.SetVisible(true);
            ExploreController.Instance.ContinueEnemy();
        });
    }
}
