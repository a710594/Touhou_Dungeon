using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_2 : Plot //新手教學戰鬥後的對話,對話結束後到探索場景
{
    public override void Start()
    {
        BattleUI.Close();
        Camera.main.transform.position = new Vector3(100, 0, -10);
        ConversationUI.Open(2001, true, () =>
        {
            TeamManager.Instance.RecoverAllMember();
            ItemManager.Instance.AddItem(0, 1, ItemManager.Type.Bag);
            AudioSystem.Instance.Stop(true);
            ProgressManager.Instance.SetFlag(1, true);

            AudioSystem.Instance.Play("Forest", true);
            ExploreController.Instance.GenerateFloor(6);
        });
    }
}
