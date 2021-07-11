using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_13 : Plot
{
    public override void Start()
    {
        ExploreUI.Instance.SetVisible(false);
        ExploreController.Instance.StopEnemy();
        ConversationUI.Open(20001, false, () =>
        {
            ProgressManager.Instance.Memo.Floor13_Flag = true;
            GameSystem.Instance.AutoSave();
            ExploreUI.Instance.SetVisible(true);
            ExploreController.Instance.ContinueEnemy();
        });
    }
}
