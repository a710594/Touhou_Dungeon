using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_14 : Plot
{
    public override void Start()
    {
        if (BattleController.Instance.Turn == 1)
        {
            BattleUI.Instance.SetVisible(false);
            ConversationUI.Open(4001, false, () =>
            {
                BattleUI.Instance.SetVisible(true);
                BattleController.Instance.TurnStartHandler -= Start;
            });
        }

        Plot_15 plot_15 = new Plot_15();
        plot_15.Start();
    }
}
