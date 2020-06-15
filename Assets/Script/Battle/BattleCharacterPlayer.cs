using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacterPlayer : BattleCharacter
{
    public void Init(TeamMember member)
    {
        Info.Init(member);
        _originalPosition = transform.position;
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
            if (skillData.CD > 0)
            {
                BattleController.Instance.TurnEndHandler += skill.SetCD;
            }

            SkillList.Add(skill);
        }

        for (int i = 0; i < member.SpellCardList.Count; i++)
        {
            id = member.SpellCardList[i];
            skillData = SkillData.GetData(id);
            skill = SkillFactory.GetNewSkill(skillData);
            if (skillData.CD > 0)
            {
                BattleController.Instance.TurnEndHandler += skill.SetCD;
            }

            SpellCardList.Add(skill);
        }

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }
    public void Init(BattlePlayerMemo memo)
    {
        JobData.RootObject data = JobData.GetData(memo.ID);

        Info.Init(memo);
        _originalPosition = transform.position;
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

        SkillData.RootObject skillData;
        Skill skill;
        for (int i = 0; i < memo.SkillList.Count; i++)
        {
            skillData = SkillData.GetData(memo.SkillList[i]);
            skill = SkillFactory.GetNewSkill(skillData);
            skill.CurrentCD = memo.SkillCdList[i];
            if (skillData.CD > 0)
            {
                BattleController.Instance.TurnEndHandler += skill.SetCD;
            }

            SkillList.Add(skill);
        }

        for (int i = 0; i < memo.SpellCardList.Count; i++)
        {
            skillData = SkillData.GetData(memo.SpellCardList[i]);
            skill = SkillFactory.GetNewSkill(skillData);
            if (skillData.CD > 0)
            {
                BattleController.Instance.TurnEndHandler += skill.SetCD;
            }

            SpellCardList.Add(skill);
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
        Info.CurrentMP -= SelectedSkill.Data.MP;
        BattleController.Instance.MinusPower(SelectedSkill.Data.NeedPower);
    }
}
