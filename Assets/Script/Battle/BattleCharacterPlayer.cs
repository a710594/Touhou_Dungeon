using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacterPlayer : BattleCharacter
{
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
        Name = member.JobName;
        SmallImage = member.SmallImage;
        MediumImage = member.MediumImage;
        LargeImage = member.LargeImage;
        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + SmallImage);

        int id;
        SkillData.RootObject skillData;
        Skill skill;
        for (int i = 0; i < member.SkillList.Count; i++)
        {
            id = member.SkillList[i];
            skillData = SkillData.GetData(id);
            skill = SkillFactory.GetNewSkill(skillData);
            BattleController.Instance.TurnEndHandler += skill.SetCD;

            SkillList.Add(skill);
        }

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }

    public void ReturnToOriginalPosition()
    {
        transform.position = _originalPosition;
    }

    public void Move()
    {
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
        });
    }

    public void SelectSkill(Skill skill)
    {
        SelectedSkill = skill;
    }
}
