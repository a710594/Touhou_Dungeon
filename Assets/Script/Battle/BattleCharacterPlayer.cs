﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacterPlayer : BattleCharacter
{
    public void Init(TeamMember member)
    {
        Info.Init(member);
        Info.SetPosition(transform.position);
        MediumImage = member.Data.MediumImage;
        LargeImage = member.Data.LargeImage;

        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + member.Data.SmallImage);
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
                Info.SetPosition(transform.position);
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
        Info.CurrentMP -= SelectedSkill.Data.MP;
        BattleController.Instance.MinusPower(SelectedSkill.Data.NeedPower);
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
