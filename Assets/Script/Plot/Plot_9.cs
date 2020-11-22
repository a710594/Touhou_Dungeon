using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_9 : Plot
{
    private BattleCharacter _getDamageCharacter;
    private Plot_9_1 _plot_9_1 = new Plot_9_1();
    private Plot_9_2 _plot_9_2 = new Plot_9_2();

    public Plot_9()
    {
        BattleCharacter character;
        character = GameObject.Find("uuz").GetComponent<BattleCharacter>();
        character.GetDamageHandler += OnGetDamage;

        for (int i = 1; i <= 6; i++)
        {
            character = GameObject.Find("Ghost_" + i).GetComponent<BattleCharacter>();
            character.GetDamageHandler += OnGetDamage;
        }
    }

    public override void Start(Action callback)
    {
        if (_getDamageCharacter != null)
        {
            if (_getDamageCharacter.EnemyId == 24)
            {
                _plot_9_1.Check(_getDamageCharacter, callback);
            }
            else if ((_getDamageCharacter.EnemyId == 22 || _getDamageCharacter.EnemyId == 23))
            {
                _plot_9_2.Check(_getDamageCharacter, callback);
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
