using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacterPlayer : BattleCharacter
{
    public GrayScale GrayScale;

    public int MaxMP;
    protected int _currentMP;
    public int CurrentMP
    {
        get
        {
            return _currentMP;
        }

        set
        {
            _currentMP = value;
            if (_currentMP <= 0)
            {
                _currentMP = 0;
            }
            else if (_currentMP > MaxMP)
            {
                _currentMP = MaxMP;
            }
        }
    }

    public void Init(TeamMember member)
    {
        Lv = member.Lv;
        MaxHP = member.MaxHP;
        CurrentHP = member.CurrentHP;
        MaxMP = member.MaxMP;
        CurrentMP = member.CurrentMP;
        _atk = member.ATK;
        _def = member.DEF;
        _mtk = member.MTK;
        _mef = member.MEF;
        _agi = member.AGI;
        _sen = member.SEN;
        _moveDistance = member.MoveDistance;
        Name = member.Data.GetName();
        SmallImage = member.Data.SmallImage;
        MediumImage = member.Data.MediumImage;
        LargeImage = member.Data.LargeImage;


        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + SmallImage);
        if (member.Data.Animator != string.Empty)
        {
            Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/" + member.Data.Animator);
        }

        int id;
        SkillData.RootObject skillData;
        Skill skill;
        for (int i = 0; i < member.SkillList.Count; i++)
        {
            id = member.SkillList[i];
            skillData = SkillData.GetData(id);
            skill = SkillFactory.GetNewSkill(skillData);
            BattleController.Instance.TurnEndHandler += skill.SetCD;

            if (skill.IsSpellCard)
            {
                SpellCardList.Add(skill);
            }
            else
            {
                SkillList.Add(skill);
            }
        }

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }

    public void ReturnToOriginalPosition()
    {
        transform.position = _originalPosition;
    }

    public void Move()
    {
        if (Animator != null)
        {
            Animator.SetBool("IsMoving", true);
        }

        Vector2Int destination = _path.Dequeue();
        if (transform.position.x - destination.x > 0 && _lookAt == Vector2Int.right)
        {
            Sprite.flipX = false;
            _lookAt = Vector2Int.left;
        }
        else if (transform.position.x - destination.x < 0 && _lookAt == Vector2Int.left)
        {
            Sprite.flipX = true;
            _lookAt = Vector2Int.right;
        }

        transform.DOMove((Vector2)destination, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (_path.Count > 0)
            {
                Move();
            }
            else
            {
                Animator.SetBool("IsMoving", false);
            }
        });
    }

    public void SelectSkill(Skill skill)
    {
        SelectedSkill = skill;
    }

    public override void UseSkill(Action callback)
    {
        base.UseSkill(callback);
        CurrentMP -= SelectedSkill.Data.MP;
        BattleController.Instance.MinusPower(SelectedSkill.Data.NeedPower);
    }

    public override void SetDamage(BattleCharacter executor, SkillData.RootObject skillData, Action<BattleCharacter> callback)
    {
        HitType hitType;
        int damage = -1;
        int callbackCount = 0;
        string text;
        FloatingNumber.Type type = FloatingNumber.Type.Other;
        for (int i = 0; i < skillData.Hits; i++)
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
            else if (hitType == HitType.Miss)
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
                    if (LiveState == LiveStateEnum.Dying)
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
        foreach (KeyValuePair<int, BattleStatus> item in _poisonDic)
        {
            damage = ((Poison)item.Value).Damage;

            CurrentHP -= damage;

            BattleUI.Instance.SetFloatingNumber(this, damage.ToString(), FloatingNumber.Type.Poison, () =>
            {
                callbackCount++;
                if (CurrentHP <= 0)
                {
                    if (LiveState == LiveStateEnum.Dying)
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
                    if (callbackCount == _poisonDic.Count && callback != null)
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

        if (LiveState == LiveStateEnum.Dying)
        {
            LiveState = LiveStateEnum.Alive;
            GrayScale.SetScale(1);
        }
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
