using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_12 : Plot //打完第二關BOSS,回到村莊,咲夜入隊
{
    public override void Start()
    {
        ConversationUI.Open(19001, true, () =>
        {
            ExploreController.Instance.ArriveFloor = 13;
            TeamManager.Instance.AddMember(5, true, new Vector2Int(0, -1), 41005, 42003);
            ProgressManager.Instance.Memo.Stage_3_Flag = true;
            AudioSystem.Instance.Play("Jinja", true);
            GameSystem.Instance.AutoSave();
            Tip_1();
        });
    }

    private void Tip_1()
    {
        ConfirmUI.Open("關於異常狀態：\n咲夜有一些能使敵方陷入異常狀態(毒/麻痺/睡眠)的技能。", "確定", Tip_2);
    }

    private void Tip_2()
    {
        ConfirmUI.Open("異常狀態可以用來牽制敵人，但要注意，一個敵人只會陷入一種異常狀態一次。", "確定", Tip_3);
    }

    private void Tip_3()
    {
        ConfirmUI.Open("比方說如果某個敵人陷入睡眠一次後就不會再次睡眠。", "確定", null);
    }
}
