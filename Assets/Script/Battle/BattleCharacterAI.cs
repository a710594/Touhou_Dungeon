using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterAI : BattleCharacter
{
    public AI AI;

    private Action _endAICallback;

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
}
