using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_4 : Plot
{
    public override void Start()
    {
        if (BattleController.Instance.Turn == 1)
        {
            BattleUI.Instance.SetVisible(false);
            ConversationUI.Open(4001, false, ()=> 
            {
                BattleUI.Instance.SetVisible(true);
                BattleController.Instance.TurnStartHandler -= Start;
            });
        }

        Plot_5 plot_5 = new Plot_5();
        BattleCharacter character;
        character = GameObject.Find("Gap_1").GetComponent<BattleCharacter>();
        character.OnDeathHandler += plot_5.Check;
        character = GameObject.Find("Gap_2").GetComponent<BattleCharacter>();
        character.OnDeathHandler += plot_5.Check;
    }
}
