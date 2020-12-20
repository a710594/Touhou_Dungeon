using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_10 : Plot
{
    public override void Start()
    {
        if (BattleController.Instance.Turn == 1)
        {
            BattleUI.Instance.SetVisible(false);
            ConversationUI.Open(13001, false, () =>
            {
                BattleUI.Instance.SetVisible(true);
                BattleController.Instance.TurnStartHandler -= Start;
            });
        }

        Plot_11 plot_11 = new Plot_11();
        BattleController.Instance.ShowEndHandler += plot_11.Start;
    }
}
