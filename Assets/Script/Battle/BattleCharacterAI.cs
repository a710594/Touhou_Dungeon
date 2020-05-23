using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacterAI : BattleCharacter
{
    public AI AI;
    //public BattleCharacter Target;
    public bool HasTarget = false;

    private Action _endAICallback;
    private List<Vector2Int> _detectRangeList = new List<Vector2Int>();

    public virtual void Init(int id, int lv)
    {
        EnemyData.RootObject data = EnemyData.GetData(id);

        Info.Init(id, lv);
        Info.SetPosition(transform.position);
        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + data.Image);
        gameObject.AddComponent(Type.GetType(data.AI));
        AI = GetComponent(Type.GetType(data.AI)) as AI;
        AI.Init(this, data.SkillList);

        //SelectedSkill = SkillFactory.GetNewSkill(1); //temp

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }

    public void StartAI(Action callback)
    {
        _endAICallback = callback;
        AI.StartAI();
    }

    public void EndAI()
    {
        if (_endAICallback != null)
        {
            _endAICallback();
        }
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
        TilePainter.Instance.Clear(3);

        for (int i = 0; i < _detectRangeList.Count; i++)
        {
            TilePainter.Instance.Painting("RedBlock", 3, _detectRangeList[i]);
        }
    }

    public void Move(Vector2Int destination)
    {
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

        transform.DOMove((Vector2)destination, 0.2f).SetEase(Ease.Linear);
    }

    public bool InSkillDistance()
    {
        return Utility.GetDistance(transform.position, TargetPosition) <= SelectedSkill.Data.Distance;
    }
}
