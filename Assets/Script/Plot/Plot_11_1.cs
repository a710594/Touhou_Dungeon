using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_11_1 : Plot
{
    public bool IsCompleted = false;

    private int _noDamageId = 12002;
    private CheckState _allDead = CheckState.NotSatisfy; //幽靈和魔法陣死光
    private CheckState _uuzGetNoDamage = CheckState.NotSatisfy; //uuz 在無敵狀態中被攻擊

    public Plot_11_1()
    {
        BattleCharacter character;
        character = GameObject.Find("uuz").GetComponent<BattleCharacter>();
        character.GetDamageHandler += CheckUuz;

        for (int i = 1; i <= 6; i++)
        {
            character = GameObject.Find("Ghost_" + i).GetComponent<BattleCharacter>();
            character.GetDamageHandler += CheckGhost;
        }

        for (int i = 1; i <= 2; i++)
        {
            character = GameObject.Find("MagicCircle_" + i).GetComponent<BattleCharacter>();
            character.GetDamageHandler += CheckGhost;
        }
    }

    public override void Start(Action callback)
    {
        if (_allDead == CheckState.Satisfy)
        {
            AllGhostDead(callback);
        }
        else if (_uuzGetNoDamage == CheckState.Satisfy)
        {
            UuzGetNoDamage(callback);
        }
        else
        {
            callback();
        }
    }

    private void CheckGhost(BattleCharacter character) //檢查幽靈死掉的數量
    {
        int count = 0;
        List<BattleCharacter> characterList = BattleController.Instance.CharacterList;
        for (int i = 0; i < characterList.Count; i++)
        {
            if ((characterList[i].EnemyId == 22 || characterList[i].EnemyId == 23 || characterList[i].EnemyId == 25) && characterList[i].LiveState == BattleCharacter.LiveStateEnum.Dead)
            {
                count++;
            }
        }

        if (_allDead == CheckState.NotSatisfy && count == 8)
        {
            _allDead = CheckState.Satisfy;
        }
    }

    private void CheckUuz(BattleCharacter character) //檢查uuz是否在無敵狀態中受到攻擊
    {
        if (_uuzGetNoDamage == CheckState.NotSatisfy && character.Info.IsNoDamage())
        {
            _uuzGetNoDamage = CheckState.Satisfy;
        }
    }

    private void AllGhostDead(Action callback) //所有的幽靈和魔法陣死掉
    {
        BattleCharacter uuz = GameObject.Find("uuz").GetComponent<BattleCharacter>();
        uuz.Info.RemoveStasus(_noDamageId);
        uuz.Animator.SetBool("NoDamage", false);

        BattleUI.Instance.SetVisible(false);
        ConversationUI.Open(15001, false, () =>
        {
            BattleUI.Instance.SetVisible(true);
            _allDead = CheckState.Completed;
            IsCompleted = true;
            callback();
        });
    }

    private void UuzGetNoDamage(Action callback) //uuz在無敵狀態中受到攻擊
    {
        BattleUI.Instance.SetVisible(false);
        ConversationUI.Open(14001, false, () =>
        {
            BattleUI.Instance.SetVisible(true);
            _uuzGetNoDamage = CheckState.Completed;
            callback();
        });
    }
}
