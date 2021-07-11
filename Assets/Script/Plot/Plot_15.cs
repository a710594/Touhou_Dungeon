using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_15 : Plot
{
    private int _startTurn;
    private BattleCharacter _remilia;

    public override void Start()
    {
        _startTurn = BattleController.Instance.Turn;
        _remilia = GameObject.Find("Remilia").GetComponent<BattleCharacter>();
        BattleController.Instance.TurnStartHandler += SummonBat;
        BattleController.Instance.TurnStartHandler -= Start;
    }

    private void SummonBat() //雷米利亞活著且血大於一條的時候每2回合召喚一次蝙蝠
    {
        if (_remilia.LiveState == BattleCharacter.LiveStateEnum.Alive && _remilia.Info.HPQueue.Count > 0 && (BattleController.Instance.Turn - _startTurn) % 2 == 1)
        {
            BattleCharacter character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            character.Init(28, _remilia.Lv);
            character.SetPosition(BattleFieldManager.Instance.GetValidPosition());
            BattleController.Instance.AddCharacer(character, true);
        }
    }
}
