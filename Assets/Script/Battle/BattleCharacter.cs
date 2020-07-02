﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacter : MonoBehaviour
{
    public enum CampEnum
    {
        Partner = 0,
        Enemy,
        All,
        None,
    }

    public enum NotActReason
    {
        Sleeping,
        Paralysis,
        None,
    }

    public enum LiveStateEnum
    {
        Alive,
        Dying,
        Dead,
    }

    public Action OnDeathHandler;

    public string MediumImage;
    public string LargeImage;
    public Vector2Int TargetPosition = new Vector2Int();
    public CampEnum Camp;
    public SpriteRenderer Sprite;
    public Animator Animator;
    public Transform ValueBarAnchor;
    public Skill SelectedSkill;
    public SpriteOutline SpriteOutline;
    public GrayScale GrayScale;
    public AI AI;
    public BattleCharacterInfo Info = new BattleCharacterInfo();

    public LiveStateEnum LiveState
    {
        get
        {
            if (Info.CurrentHP > 0)
            {
                return LiveStateEnum.Alive;
            }
            else if (Info.CurrentHP == 0 && Camp == CampEnum.Partner)
            {
                return LiveStateEnum.Dying;
            }
            else
            {
                return LiveStateEnum.Dead;
            }
        }
    }

    public bool CanHitTarget
    {
        get 
        {
            return AI.CanHitTarget;
        }
    }

    protected Vector2 _originalPosition = new Vector2();
    protected Vector2 _lastPosition = new Vector2(); //上一步的位置
    protected Vector2Int _lookAt = Vector2Int.left;
    protected List<Vector2Int> _moveRangeList = new List<Vector2Int>();
    private List<Vector2Int> _detectRangeList = new List<Vector2Int>();


    public void Init(TeamMember member)
    {
        Info.Init(member);
        Init(member.Data);
    }
    public void Init(BattleCharacterMemo memo)
    {
        Info.Init(memo);
        Init(JobData.GetData(memo.ID));
    }

    private void Init(JobData.RootObject data)
    {
        _originalPosition = transform.position;

        Camp = CampEnum.Partner;
        MediumImage = data.MediumImage;
        LargeImage = data.LargeImage;

        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + data.SmallImage);
        if (data.Animator != string.Empty)
        {
            Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/" + data.Animator);
        }

        if (LiveState == LiveStateEnum.Dying)
        {
            GrayScale.SetScale(0);
            Animator.SetBool("IsDying", true);
        }

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }

    public void Init(int id, int lv)
    {
        EnemyData.RootObject data = EnemyData.GetData(id);

        Info.Init(id, lv);
        _originalPosition = transform.position;
        Camp = CampEnum.Enemy;
        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + data.Image);
        if (data.Animator != string.Empty)
        {
            Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/" + data.Animator);
        }
        gameObject.AddComponent(Type.GetType(data.AI));
        AI = GetComponent(Type.GetType(data.AI)) as AI;
        AI.Init(this, data.SkillList);

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }

    public void CheckBattleStatus()
    {
        Info.CheckBattleStatus();
    }

    public bool CanAct(out NotActReason reason)
    {
        if (Info.IsSleeping)
        {
            reason = NotActReason.Sleeping;
            return false;
        }
        else if (Info.IsParalysis)
        {
            reason = NotActReason.Paralysis;
            return false;
        }
        else
        {
            reason = NotActReason.None;
            return true;
        }
    }

    public void ReturnToOriginalPosition()
    {
        transform.position = _originalPosition;
    }

    public List<Vector2Int> GetMoveRange()
    {
        _moveRangeList.Clear();

        Vector2Int orign = Vector2Int.FloorToInt(_originalPosition);
        List<Vector2Int> positionList = Utility.GetRhombusPositionList(Info.MOV, orign, false);
        for (int i = 0; i < positionList.Count; i++)
        {
            int pathLength = BattleFieldManager.Instance.GetPathLength(orign, positionList[i], Camp);
            if (pathLength != -1 && pathLength - 1 <= Info.MOV) //pathLength - 1 是因為要扣掉自己那一格
            {
                _moveRangeList.Add(positionList[i]);
            }
        }
        return _moveRangeList;
    }

    public void ShowMoveRange()
    {
        TilePainter.Instance.Clear(2);

        for (int i = 0; i < _moveRangeList.Count; i++)
        {
            TilePainter.Instance.Painting("BlueGrid", 2, _moveRangeList[i]);
        }
    }

    public bool InMoveRange(Vector2Int pos)
    {
        return _moveRangeList.Contains(pos);
    }

    public Queue<Vector2Int> GetPath(Vector2Int position)
    {
        Queue<Vector2Int> path;
        List<Vector2Int> list = BattleFieldManager.Instance.GetPath(transform.position, position, Camp);
        if (list != null)
        {
            path = new Queue<Vector2Int>(list);
        }
        else
        {
            path = null;
        }

        return path;
    }

    public void StartMove(Queue<Vector2Int> path, Action callback)
    {
        StartCoroutine(Move(path, callback));
    }

    public void InitActionCount()
    {
        Info.InitActionCount();
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
        _originalPosition = position;
        Info.SetPosition(position);
    }

    public void MoveDone()
    {
        _lastPosition = _originalPosition;
        _originalPosition = transform.position;
        Info.MoveDone();
        Info.SetPosition(transform.position);
    }

    public void SkillDone()
    {
        Info.SkillDone();
    }

    public bool CanUndoMove()
    {
        if (Info.ActionCount < 2 && !Info.HasUseSkill)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MoveUndo()
    {
        transform.position = _lastPosition;
        _originalPosition = _lastPosition;
        Info.MoveUndo();
    }

    public void ActionDoneCompletely()
    {
        Info.ActionDoneCompletely();
    }

    public void GetSkillDistance()
    {
        SelectedSkill.GetDistance(this, BattleController.Instance.CharacterList);
    }

    public bool IsInSkillDistance(Vector2Int position)
    {
        if (SelectedSkill != null)
        {
            return SelectedSkill.IsInDistance(position);
        }
        else
        {
            return false;
        }
    }

    public void GetSkillRange()
    {
        Vector2Int orign = Vector2Int.FloorToInt(transform.position);
        SelectedSkill.GetRange(TargetPosition, orign, this, BattleController.Instance.CharacterList);
    }

    public List<Vector2Int> GetDetectRange() //偵查範圍:移動後可用技能擊中目標的範圍
    {
        if (SelectedSkill == null)
        {
            return null;
        }

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
        TilePainter.Instance.Clear(3);

        for (int i = 0; i < _detectRangeList.Count; i++)
        {
            TilePainter.Instance.Painting("RedBlock", 3, _detectRangeList[i]);
        }
    }

    public void SetTarget(Vector2Int position)
    {
        TargetPosition = position;
    }

    public bool IsTarget(Vector2Int position)
    {
        return position == TargetPosition;
    }

    public void SelectSkill(Skill skill)
    {
        SelectedSkill = skill;
    }

    public virtual void UseSkill(Action callback)
    {
        SelectedSkill.Use(this, callback);

        if (Camp == CampEnum.Partner)
        {
            Info.CurrentMP -= SelectedSkill.Data.MP;
            BattleController.Instance.MinusPower(SelectedSkill.Data.NeedPower);
        }
    }

    public virtual void SetDamage(int damage, AttackSkill.HitType hitType, Action<BattleCharacter> callback)
    {
        Info.CurrentHP -= damage;

        string text = "";
        FloatingNumber.Type floatingNumberType = FloatingNumber.Type.Other;
        if (hitType == AttackSkill.HitType.Critical)
        {
            floatingNumberType = FloatingNumber.Type.Critical;
            text = damage.ToString();
        }
        else if (hitType == AttackSkill.HitType.Hit)
        {
            floatingNumberType = FloatingNumber.Type.Damage;
            text = damage.ToString();
        }
        else if (hitType == AttackSkill.HitType.Miss)
        {
            floatingNumberType = FloatingNumber.Type.Miss;
            text = "Miss";
        }

        BattleUI.Instance.SetFloatingNumber(this, text, floatingNumberType, () =>
        {
            if (Info.CurrentHP <= 0)
            {
                if (LiveState == BattleCharacter.LiveStateEnum.Dying)
                {
                    SetDying(() =>
                    {
                        if (callback != null)
                        {
                            callback(this);
                        }
                    });
                }
                else
                {
                    SetDeath(() =>
                    {
                        if (callback != null)
                        {
                            callback(this);
                        }
                    });
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(this);
                }
            }
        });

        if (Info.IsSleeping)
        {
            //解除睡眠狀態
            Info.RemoveSleep();
        }
    }

    public void SetPoison(int id, int damage, Action<BattleCharacter> callback)
    {
        Info.SetPoison(id, damage);

        BattleUI.Instance.SetStatus(this, BattleStatusData.GetData(id).Comment, FloatingNumber.Type.Other, () =>
        {
            if (callback != null)
            {
                callback(this);
            }
        });
    }

    public virtual void SetPoisonDamage(Action callback) //回合結束時計算毒傷害
    {
        int callbackCount = 0;
        List<int> damageList = Info.GetPoisonDamageList();
        for(int i=0; i< damageList.Count; i++)
        {
            Info.CurrentHP -= damageList[i];

            BattleUI.Instance.SetFloatingNumber(this, damageList[i].ToString(), FloatingNumber.Type.Poison, () =>
            {
                callbackCount++;
                if (Info.CurrentHP <= 0)
                {
                    if (LiveState == BattleCharacter.LiveStateEnum.Dying)
                    {
                        SetDying(callback);
                    }
                    else
                    {
                        SetDeath(callback);
                    }
                }
                else
                {
                    if (callbackCount == damageList.Count && callback != null)
                    {
                        callback();
                    }
                }
            });
        }
    }

    public virtual void SetRecover(int recover, Action<BattleCharacter> callback)
    {
        if (LiveState == BattleCharacter.LiveStateEnum.Dying)
        {
            GrayScale.SetScale(1);
            Animator.SetBool("IsDying", false);
        }

        Info.CurrentHP += recover;
        BattleUI.Instance.SetFloatingNumber(this, recover.ToString(), FloatingNumber.Type.Recover, () =>
        {
            if (callback != null)
            {
                callback(this);
            }
        });
    }

    public void ClearAbnormal(Action<BattleCharacter> callback)
    {
        Info.ClearAbnormal();

        BattleUI.Instance.SetStatus(this, "解除異常狀態", FloatingNumber.Type.Other, () =>
        {
            if (callback != null)
            {
                callback(this);
            }
        });
    }

    public void SetBuff(int id, Action<BattleCharacter> callback)
    {
        Info.SetBuff(id);

        BattleUI.Instance.SetStatus(this, BattleStatusData.GetData(id).Comment, FloatingNumber.Type.Other, () =>
        {
            callback(this);
        });
    }

    public void SetOutline(bool show)
    {
        SpriteOutline.SetOutline(show);
    }

    public void StartAI(Action callback)
    {
        AI.StartAI(callback);
    }

    protected virtual void SetDeath(Action callback)
    {
        Sprite.DOFade(0, 0.5f).OnComplete(() =>
        {
            BattleUI.Instance.SetLittleHPBar(this, false);

            if (callback != null)
            {
                callback();
            }
        });
    }

    private void SetDying(Action callback)
    {
        GrayScale.SetScale(0);
        Animator.SetBool("IsDying", true);
        if (callback != null)
        {
            callback();
        }
    }

    private IEnumerator Move(Queue<Vector2Int> path, Action callback)
    {
        Vector2Int position = new Vector2Int();

        if (Animator != null)
        {
            Animator.SetBool("IsMoving", true);
        }

        while (path.Count > 0)
        {
            position = path.Dequeue();

            if (transform.position.x - position.x > 0 && _lookAt == Vector2Int.right)
            {
                Sprite.flipX = false;
                _lookAt = Vector2Int.left;
            }
            else if (transform.position.x - position.x < 0 && _lookAt == Vector2Int.left)
            {
                Sprite.flipX = true;
                _lookAt = Vector2Int.right;
            }
            transform.DOMove((Vector2)position, 0.2f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.2f);
        }

        if (Animator != null)
        {
            Animator.SetBool("IsMoving", false);
        }

        if (callback != null)
        {
            callback();
        }
    }
}
