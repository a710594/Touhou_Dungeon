using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_5 : Plot
{
    private BattleCharacter _getDamageCharacter;
    private Plot_5_1 _plot_5_1 = new Plot_5_1();
    private Plot_5_2 _plot_5_2 = new Plot_5_2();

    public Plot_5()
    {
        BattleCharacter character;
        character = GameObject.Find("Yukari").GetComponent<BattleCharacter>();
        character.GetDamageHandler += OnGetDamage;

        for (int i = 1; i <= 2; i++)
        {
            character = GameObject.Find("Gap_" + i).GetComponent<BattleCharacter>();
            character.GetDamageHandler += OnGetDamage;
        }
    }

    public override void Start(Action callback)
    {
        if (_getDamageCharacter != null)
        {
            if (_getDamageCharacter.EnemyId == 8)
            {
                _plot_5_1.Check(callback);
            }
            else if (_getDamageCharacter.EnemyId == 9)
            {
                _plot_5_2.Check(callback);
            }
            else
            {
                callback();
            }
            _getDamageCharacter = null;
        }
        else
        {
            callback();
        }
    }

    private void OnGetDamage(BattleCharacter character)
    {
        _getDamageCharacter = character;
    }
}
