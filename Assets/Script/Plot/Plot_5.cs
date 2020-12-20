using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_5 : Plot
{
    private BattleCharacter _getDamageCharacter;
    private Plot_5_1 _plot_5_1 = new Plot_5_1();
    private Plot_5_2 _plot_5_2 = new Plot_5_2();

    public override void Start(Action callback)
    {
        if (!_plot_5_1.IsCompleted)
        {
            _plot_5_1.Start(callback);
        }
        else
        {
            _plot_5_2.Start(callback);
        }
    }
}
