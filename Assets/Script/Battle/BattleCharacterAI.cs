using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacterAI : BattleCharacter
{
    public AI AI;
    public BattleCharacter Target;

    private Action _endAICallback;
    private List<Vector2Int> _detectRangeList = new List<Vector2Int>();

    public virtual void Init(int id, int lv)
    {
        EnemyData.RootObject data = EnemyData.GetData(id);

        Lv = lv;
        MaxHP = Mathf.RoundToInt(data.HP * (1 + ((lv - 1) * 0.1f)));
        CurrentHP = MaxHP;
        _atk = Mathf.RoundToInt(data.ATK * (1 + ((lv - 1) * 0.1f)));
        _def = Mathf.RoundToInt(data.DEF * (1 + ((lv - 1) * 0.1f)));
        _mtk = Mathf.RoundToInt(data.MTK * (1 + ((lv - 1) * 0.1f)));
        _mef = Mathf.RoundToInt(data.MEF * (1 + ((lv - 1) * 0.1f)));
        _agi = Mathf.RoundToInt(data.AGI * (1 + ((lv - 1) * 0.1f)));
        _sen = Mathf.RoundToInt(data.SEN * (1 + ((lv - 1) * 0.1f)));
        _moveDistance = data.MoveDistance;
        Name = data.Name;
        SmallImage = data.Image;
        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + SmallImage);
        gameObject.AddComponent(Type.GetType(data.AI));
        AI = GetComponent(Type.GetType(data.AI)) as AI;
        AI.Init(data.SkillList);

        SelectedSkill = SkillFactory.GetNewSkill(1); //temp

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }

    public void StartAI(Action callback)
    {
        _endAICallback = callback;
        AI.StartAI(this);
    }

    public void EndAI()
    {
        if (_endAICallback != null)
        {
            _endAICallback();
        }
    }

    public override void SetDamage(BattleCharacter executor, SkillData.RootObject skillData, Action<BattleCharacter> callback)
    {
        HitType hitType;
        int damage = -1;
        int callbackCount = 0;
        string text;
        FloatingNumber.Type type = FloatingNumber.Type.Other;
        for (int i=0; i<skillData.Hits; i++)
        {
            hitType = (CheckHit(executor));
            if (hitType == HitType.Critical)
            {
                damage = CalculateDamage(executor, skillData, true);
                CurrentHP -= damage;
                type = FloatingNumber.Type.Critical;
            }
            else if (hitType == HitType.Hit)
            {
                damage = CalculateDamage(executor, skillData, false);
                CurrentHP -= damage;
                type = FloatingNumber.Type.Damage;
            }
            else if(hitType == HitType.Miss)
            {
                type = FloatingNumber.Type.Miss;
            }

            if (damage == -1)
            {
                text = "Miss";
            }
            else
            {
                text = damage.ToString();
            }

            BattleUI.Instance.SetFloatingNumber(this, text, type, () =>
            {
                callbackCount++;
                if (CurrentHP <= 0)
                {
                    SetDeath(()=> 
                    {
                        if (callback != null)
                        {
                            callback(this);
                        }
                    });
                }
                else
                {
                    if (callbackCount == skillData.Hits && callback != null)
                    {
                        callback(this);
                    }
                }
            });
        }

        if (IsSleeping)
        {
            //解除睡眠狀態
            StatusDic.Remove(_sleepingId);
            _sleepingId = -1;
        }
    }

    public override void SetPoisonDamage(Action callback) //回合結束時計算毒傷害
    {
        int damage;
        int callbackCount = 0;
        for (int i = 0; i < _poisonIdList.Count; i++)
        {
            damage = ((Poison)(StatusDic[_poisonIdList[i]])).Damage;

            CurrentHP -= damage;

            BattleUI.Instance.SetFloatingNumber(this, damage.ToString(), FloatingNumber.Type.Poison, () =>
            {
                callbackCount++;
                if (CurrentHP <= 0)
                {
                    SetDeath(callback);
                }
                else
                {
                    if (callbackCount == _poisonIdList.Count && callback != null)
                    {
                        callback();
                    }
                }
            });
        }
    }

    public override void SetRecover(int recover, Action<BattleCharacter> callback)
    {
        CurrentHP += recover;
        BattleUI.Instance.SetFloatingNumber(this, recover.ToString(), FloatingNumber.Type.Recover, () =>
        {
            if (callback != null)
            {
                callback(this);
            }
        });
    }

    public List<Vector2Int> GetDetectRange() //偵查範圍:移動後可用技能擊中目標的範圍
    {
        InitOrignalPosition();
        GetMoveRange();
        _detectRangeList = new List<Vector2Int>(_moveRangeList);

        Vector2Int position = new Vector2Int();
        List<Vector2Int> newPositionList = new List<Vector2Int>();
        for (int i = 0; i < SelectedSkill.Data.Distance; i++)
        {
            for (int j = 0; j < _detectRangeList.Count; j++)
            {
                position = _detectRangeList[j];
                if (!_detectRangeList.Contains(position + Vector2Int.right))
                {
                    newPositionList.Add(position + Vector2Int.right);
                }
                if (!_detectRangeList.Contains(position + Vector2Int.left))
                {
                    newPositionList.Add(position + Vector2Int.left);
                }
                if (!_detectRangeList.Contains(position + Vector2Int.up))
                {
                    newPositionList.Add(position + Vector2Int.up);
                }
                if (!_detectRangeList.Contains(position + Vector2Int.down))
                {
                    newPositionList.Add(position + Vector2Int.down);
                }
            }
            for (int j = 0; j < newPositionList.Count; j++)
            {
                _detectRangeList.Add(newPositionList[j]);
            }
        }
        _detectRangeList = BattleFieldManager.Instance.RemoveBound(_detectRangeList);
        return _detectRangeList;
    }

    public void ShowDetectRange()
    {
        TilePainter.Instance.Clear(2);

        for (int i = 0; i < _detectRangeList.Count; i++)
        {
            TilePainter.Instance.Painting("Red", 2, _detectRangeList[i]);
        }
    }

    public void Move(Vector2Int destination)
    {
        if (transform.position.x - destination.x > 0 && _lookAt == Vector2Int.right)
        {
            Sprite.flipX = true;
            _lookAt = Vector2Int.left;
        }
        else if (transform.position.x - destination.x < 0 && _lookAt == Vector2Int.left)
        {
            Sprite.flipX = false;
            _lookAt = Vector2Int.right;
        }

        transform.DOMove((Vector2)destination, 0.2f).SetEase(Ease.Linear);
    }

    public bool InSkillDistance()
    {
        return Utility.GetDistance(transform.position, TargetPosition) <= SelectedSkill.Data.Distance;
    }
}
