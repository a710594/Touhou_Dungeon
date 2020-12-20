using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_11_2 :Plot
{
    private CheckState _summonGhost = CheckState.NotSatisfy; //uuz 剩一條血的時候召喚幽靈
    private CheckState _uuzLastHP = CheckState.NotSatisfy; //uuz 剩最後一條血的時候
    private int _startTurn; //開始召喚幽靈的回合
    private List<Vector2Int> _positionList = new List<Vector2Int>(); //幽靈可能出現的位置


    public Plot_11_2()
    {
        BattleCharacter character;
        character = GameObject.Find("uuz").GetComponent<BattleCharacter>();
        character.GetDamageHandler += Check;

        _positionList.Add(new Vector2Int(-2, 3));
        _positionList.Add(new Vector2Int(2, 3));
        _positionList.Add(new Vector2Int(-2, 2));
        _positionList.Add(new Vector2Int(-1, 2));
        _positionList.Add(new Vector2Int(1, 2));
        _positionList.Add(new Vector2Int(2, 2));
        _positionList.Add(new Vector2Int(-2, 1));
        _positionList.Add(new Vector2Int(-1, 1));
        _positionList.Add(new Vector2Int(0, 1));
        _positionList.Add(new Vector2Int(1, 1));
        _positionList.Add(new Vector2Int(2, 1));
    }

    public override void Start(Action callback)
    {
        if (_summonGhost == CheckState.Satisfy)
        {
            StartSummonGhost(callback);
        }
        else if (_uuzLastHP == CheckState.Satisfy)
        {
            UuzLastHP(callback);
        }
        else
        {
            callback();
        }
    }

    private void Check(BattleCharacter character)
    {
        if (_summonGhost == CheckState .NotSatisfy && character.Info.HPQueue.Count == 2 && character.Info.CurrentHP == 0) //剩一條血時觸發
        {
            _summonGhost = CheckState.Satisfy;
        }
        else if (_uuzLastHP == CheckState .NotSatisfy && character.Info.HPQueue.Count == 1 && character.Info.CurrentHP == 0) //剩零條血時觸發
        {
            _uuzLastHP = CheckState.Satisfy;
        }
    }

    //public void Check(BattleCharacter character, Action callback)
    //{
    //    if (!_uuzGetNoDamage && character.Info.IsNoDamage())
    //    {
    //        UuzGetNoDamage(callback);
    //    }
    //    else if (!_summonGhost && character.Info.HPQueue.Count == 1) //剩一條血時觸發
    //    {
    //        _uuzGetNoDamage = true;
    //        _summonGhost = true;
    //        StartSummonGhost(callback);
    //    }
    //    else if (!_uuzLastHP && character.Info.HPQueue.Count == 0) //剩零條血時觸發
    //    {
    //        _uuzLastHP = true;
    //        UuzLastHP(callback);
    //    }
    //    else
    //    {
    //        callback();
    //    }
    //}

    private void StartSummonGhost(Action callback) //uuz 剩一條血的時候第一次召喚幽靈
    {
        if (GetGhostPosition(out Vector2Int position))
        {
            BattleCharacter character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            int id = 22 + UnityEngine.Random.Range(0, 1); //隨機選擇戰士幽靈或法師幽靈
            character.Init(id, 1); //temp
            character.SetPosition(position);
            character.GetDamageHandler += SummonGhostGetDamage;
            BattleController.Instance.AddCharacer(character, true);
            BattleCharacter uuz = GameObject.Find("uuz").GetComponent<BattleCharacter>();
            uuz.SetNoDamage(11002);
            uuz.Animator.SetBool("NoDamage", true);

            Camera.main.transform.DOMove(new Vector3(position.x, position.y, Camera.main.transform.position.z), 0.5f).OnComplete(() =>
            {
                BattleUI.Instance.SetVisible(false);
                ConversationUI.Open(16001, false, () =>
                {
                    BattleUI.Instance.SetVisible(true);
                    _summonGhost = CheckState.Completed;
                    callback();
                });
            });

            BattleController.Instance.TurnStartHandler += SummonGhost;
            _startTurn = BattleController.Instance.Turn;
        }
        else
        {
            callback();
        }
    }

    private bool GetGhostPosition(out Vector2Int position)
    {
        position = new Vector2Int();
        List<Vector2Int> tempList = new List<Vector2Int>(_positionList);
        for (int i = 0; i < tempList.Count; i++)
        {
            position = tempList[UnityEngine.Random.Range(0, tempList.Count)];
            if (BattleController.Instance.GetCharacterByPosition(position) == null)
            {
                return true;
            }
            else
            {
                tempList.Remove(position);
                i--;
            }
        }
        return false;
    }

    private void SummonGhost() //uuz 剩一條血的時候每n回合召喚幽靈
    {
        if ((BattleController.Instance.Turn - _startTurn)%2==0 && GetGhostPosition(out Vector2Int position))
        {
            BattleCharacter character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            int id = 22 + UnityEngine.Random.Range(0, 1); //隨機選擇戰士幽靈或法師幽靈
            character.Init(id, 1); //temp
            character.SetPosition(position);
            character.GetDamageHandler += SummonGhostGetDamage;
            BattleController.Instance.AddCharacer(character, true);
            BattleCharacter uuz = GameObject.Find("uuz").GetComponent<BattleCharacter>();
            uuz.SetNoDamage(11002);
            uuz.Animator.SetBool("NoDamage", true);
        }
    }

    private void SummonGhostGetDamage(BattleCharacter character) //uuz剩一條血的時候,幽靈全被打倒時解除無敵
    {
        int count = 0;
        List<BattleCharacter> characterList = BattleController.Instance.CharacterList;
        for (int i = 0; i < characterList.Count; i++)
        {
            if ((characterList[i].EnemyId == 22 || characterList[i].EnemyId == 23) && characterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
            {
                count++;
            }
        }

        if (count == 0)
        {
            BattleCharacter uuz = GameObject.Find("uuz").GetComponent<BattleCharacter>();
            uuz.Info.RemoveStasus(11002);
            uuz.Animator.SetBool("NoDamage", false);
        }
    }

    private void UuzLastHP(Action callback) //uuz 最後一條血的對話
    {
        BattleUI.Instance.SetVisible(false);
        GameObject yukari = GameObject.Find("uuz");
        Vector3 position = new Vector3(yukari.transform.position.x, yukari.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.DOMove(position, 0.5f).OnComplete(() =>
        {
            ConversationUI.Open(17001, false, () =>
            {
                BattleUI.Instance.SetVisible(true);
                _uuzLastHP = CheckState.Completed;
                callback();
            });
        });
        BattleController.Instance.TurnStartHandler -= SummonGhost;
    }
}
