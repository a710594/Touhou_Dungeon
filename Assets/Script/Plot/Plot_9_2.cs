using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_9_2
{
    private bool _ghostDie6 = false; //幽靈死掉六個

    public void Check(BattleCharacter character, Action callback)
    {
        if (!_ghostDie6)
        {
            GhostGetDamage(callback);
        }
        else
        {
            callback();
        }
    }

    private void GhostGetDamage(Action callback) //檢查幽靈死掉的數量
    {
        int count = 0;
        List<BattleCharacter> characterList = BattleController.Instance.CharacterList;
        for (int i = 0; i < characterList.Count; i++)
        {
            if ((characterList[i].EnemyId == 22 || characterList[i].EnemyId == 23) && characterList[i].LiveState == BattleCharacter.LiveStateEnum.Dead)
            {
                count++;
            }
        }

        if (!_ghostDie6 && count == 6)
        {
            BattleCharacter uuz = GameObject.Find("uuz").GetComponent<BattleCharacter>();
            uuz.Info.RemoveStasus(11002);
            uuz.Animator.SetBool("NoDamage", false);

            BattleUI.Instance.SetVisible(false);
            ConversationUI.Open(12001, false, () =>
            {
                BattleUI.Instance.SetVisible(true);
                callback();
            });
            _ghostDie6 = true;
        }
        else
        {
            callback();
        }
    }
}
