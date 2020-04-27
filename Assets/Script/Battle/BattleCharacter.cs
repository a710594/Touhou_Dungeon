using System;
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

    public enum LiveStateEnum
    {
        Alive,
        Dying,
        Dead,
    }

    public enum NotActReason
    {
        Sleeping,
        Paralysis,
        None,
    }

    public Action OnDeathHandler;

    public string MediumImage;
    public string LargeImage;
    public Vector2Int TargetPosition = new Vector2Int();
    public CampEnum Camp;
    public LiveStateEnum LiveState;
    public SpriteRenderer Sprite;
    public Animator Animator;
    public Transform ValueBarAnchor;
    public Skill SelectedSkill;
    public SpriteOutline SpriteOutline;
    public GrayScale GrayScale;
    public BattleCharacterInfo Info = new BattleCharacterInfo();
    public List<Skill> SkillList = new List<Skill>();
    public List<Skill> SpellCardList = new List<Skill>();

    protected Vector2 _originalPosition = new Vector2();
    protected Vector2Int _lookAt = Vector2Int.right;
    protected Queue<Vector2Int> _path;
    protected List<Vector2Int> _moveRangeList = new List<Vector2Int>();

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

    public void InitOrignalPosition()
    {
        _originalPosition = transform.position;
    }

    public void GetMoveRange()
    {
        _moveRangeList.Clear();

        Vector2Int orign = Vector2Int.FloorToInt(_originalPosition);
        List<Vector2Int> positionList = Utility.GetRhombusPositionList(Info.MoveDistance, orign, false);
        for (int i = 0; i < positionList.Count; i++)
        {
            //BattleFieldManager.Instance.Refresh(orign, positionList[i], false);
            //int pathLength = BattleFieldManager.Instance.GetPathLength(orign, positionList[i], false);
            int pathLength = BattleFieldManager.Instance.GetPathLength(orign, positionList[i], Camp);
            if (pathLength != -1 && pathLength - 1 <= Info.MoveDistance) //pathLength - 1 是因為要扣掉自己那一格
            {
                _moveRangeList.Add(positionList[i]);
            }
        }
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
        List<Vector2Int> list = BattleFieldManager.Instance.GetPath(transform.position, position, Camp);
        if (list != null)
        {
            _path = new Queue<Vector2Int>(list);
        }
        else
        {
            _path = null;
        }

        return _path;
    }

    public void InitActionCount()
    {
        Info.InitActionCount();
    }

    public void ActionDone()
    {
        Info.ActionDone();
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

    public void SetTarget(Vector2Int position)
    {
        TargetPosition = position;
    }

    public bool IsTarget(Vector2Int position)
    {
        return position == TargetPosition;
    }

    public virtual void UseSkill(Action callback)
    {
        SelectedSkill.Use(this, callback);
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
                if (Camp == CampEnum.Enemy || LiveState == LiveStateEnum.Dying)
                {
                    SetDeath(() =>
                    {
                        if (callback != null)
                        {
                            callback(this);
                        }
                    });
                }
                else
                {
                    SetDying(() =>
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
                    if (Camp == CampEnum.Enemy || LiveState == LiveStateEnum.Dying)
                    {
                        SetDeath(callback);
                    }
                    else
                    {
                        SetDying(callback);
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
        Info.CurrentHP += recover;
        BattleUI.Instance.SetFloatingNumber(this, recover.ToString(), FloatingNumber.Type.Recover, () =>
        {
            if (callback != null)
            {
                callback(this);
            }
        });

        if (LiveState == LiveStateEnum.Dying)
        {
            LiveState = LiveStateEnum.Alive;
            GrayScale.SetScale(1);
        }
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

    protected virtual void SetDeath(Action callback)
    {
        LiveState = LiveStateEnum.Dead;
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
        LiveState = LiveStateEnum.Dying;
        GrayScale.SetScale(0);
        if (callback != null)
        {
            callback();
        }
    }
}
