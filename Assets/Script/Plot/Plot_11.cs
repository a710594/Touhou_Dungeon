using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_11 : Plot
{
    private Plot_11_1 _plot_11_1 = new Plot_11_1();
    private Plot_11_2 _plot_11_2 = new Plot_11_2();

    //public Plot_11()
    //{
    //    BattleCharacter character;
    //    character = GameObject.Find("uuz").GetComponent<BattleCharacter>();
    //    character.GetDamageHandler += OnGetDamage;

    //    for (int i = 1; i <= 6; i++)
    //    {
    //        character = GameObject.Find("Ghost_" + i).GetComponent<BattleCharacter>();
    //        character.GetDamageHandler += OnGetDamage;
    //    }

    //    for (int i = 1; i <= 2; i++)
    //    {
    //        character = GameObject.Find("MagicCircle_" + i).GetComponent<BattleCharacter>();
    //        character.GetDamageHandler += OnGetDamage;
    //    }

    //    _plot_11_1.OnCompleteHandler = Set_11_1_Complete;
    //}

    public override void Start(Action callback)
    {
        if (!_plot_11_1.IsCompleted)
        {
            _plot_11_1.Start(callback);
        }
        else
        {
            _plot_11_2.Start(callback);
        }
    }
}
