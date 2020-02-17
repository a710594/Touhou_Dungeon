using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacter : MonoBehaviour
{
    protected static readonly int _maxActionCount = 2;

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

    public enum HitType
    {
        Miss,
        Hit,
        Critical
    }

    public Action OnDeathHandler;

    public int Lv;
    public string Name;
    public string SmallImage;
    public string MediumImage;
    public string LargeImage;
    public Vector2Int TargetPosition = new Vector2Int();
    public CampEnum Camp;
    public LiveStateEnum LiveState;
    public SpriteRenderer Sprite;
    public Transform ValueBarAnchor;
    public Skill SelectedSkill;
    public SpriteOutline SpriteOutline;
    public List<Skill> SkillList = new List<Skill>();
    public Dictionary<int, BattleStatus> StatusDic = new Dictionary<int, BattleStatus>();

    protected int _sleepingId = -1;
    protected float _paralysisProbability = 0;
    protected Vector2 _originalPosition = new Vector2();
    protected Vector2Int _lookAt = Vector2Int.right;
    protected Queue<Vector2Int> _path;
    protected List<int> _poisonIdList = new List<int>();
    protected List<Vector2Int> _moveRangeList = new List<Vector2Int>();

    public int MaxHP;
    protected int _currentHP;
    public int CurrentHP
    {
        get
        {
            return _currentHP;
        }

        set
        {
            _currentHP = value;
            if (_currentHP <= 0)
            {
                _currentHP = 0;

                if (OnDeathHandler != null)
                {
                    OnDeathHandler();
                }
            }
            else if (_currentHP > MaxHP)
            {
                _currentHP = MaxHP;
            }
        }
    }

    protected int _atk; //不含buff的總和攻擊力
    public int ATK
    {
        get
        {
            return Mathf.RoundToInt((float)_atk * GetBuffATK() * BattleFieldManager.Instance.GetFieldBuff(transform.position).ATK);
        }
    }

    protected int _def; //不含buff的總和防禦力
    public int DEF
    {
        get
        {
            return Mathf.RoundToInt((float)_def * GetBuffDEF() * BattleFieldManager.Instance.GetFieldBuff(transform.position).DEF);
        }
    }

    protected int _mtk; //不含buff的總和魔法攻擊力
    public int MTK
    {
        get
        {
            return Mathf.RoundToInt((float)_mtk * GetBuffMTK() * BattleFieldManager.Instance.GetFieldBuff(transform.position).MTK);
        }
    }

    protected int _mef; //不含buff的總和魔法防禦力
    public int MEF
    {
        get
        {
            return Mathf.RoundToInt((float)_mef * GetBuffMEF() * BattleFieldManager.Instance.GetFieldBuff(transform.position).MEF);
        }
    }

    protected int _agi; //不含buff的敏捷(影響迴避)
    public int AGI
    {
        get
        {
            return Mathf.RoundToInt((float)_agi * GetBuffAGI() * BattleFieldManager.Instance.GetFieldBuff(transform.position).AGI);
        }
    }

    protected int _sen; //不含buff的感知(影響命中)
    public int SEN
    {
        get
        {
            return Mathf.RoundToInt((float)_sen * GetBuffSEN() * BattleFieldManager.Instance.GetFieldBuff(transform.position).SEN);
        }
    }

    protected int _moveDistance; //不含buff的移動距離
    public int MoveDistance
    {
        get
        {
            return _moveDistance;
        }
    }

    public bool IsPoisoning
    {
        get
        {
            return _poisonIdList.Count > 0;
        }
    }

    public bool IsSleeping
    {
        get
        {
            return _sleepingId != -1;
        }
    }

    protected int _actionCount; //不含buff的總和攻擊力
    public int ActionCount
    {
        get
        {
            return _actionCount;
        }
    }

    public void CheckBattleStatus()
    {
        List<int> keyList = new List<int>(StatusDic.Keys);
        List<BattleStatus> statusList = new List<BattleStatus>(StatusDic.Values);

        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value.RemainTurn != -1) //-1代表永久
            {
                item.Value.RemainTurn--;
                if (item.Value.RemainTurn == 0)
                {
                    if (item.Value is Poison)
                    {
                        _poisonIdList.Remove(item.Value.Data.ID);
                    }
                    else if (item.Value is Paralysis)
                    {
                        _paralysisProbability = 0;
                    }
                    else if (item.Value is Sleeping)
                    {
                        _sleepingId = -1;
                    }

                    StatusDic.Remove(item.Key);
                }
            }
        }
    }

    public bool CanAct(out NotActReason reason)
    {
        if (IsSleeping)
        {
            reason = NotActReason.Sleeping;
            return false;
        }
        else if (UnityEngine.Random.Range(0, 100) < _paralysisProbability)
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
        List<Vector2Int> positionList = Utility.GetRhombusPositionList(MoveDistance, orign, false);
        for (int i = 0; i < positionList.Count; i++)
        {
            //BattleFieldManager.Instance.Refresh(orign, positionList[i], false);
            //int pathLength = BattleFieldManager.Instance.GetPathLength(orign, positionList[i], false);
            int pathLength = BattleFieldManager.Instance.GetPathLength(orign, positionList[i], Camp);
            if (pathLength != -1 && pathLength - 1 <= MoveDistance) //pathLength - 1 是因為要扣掉自己那一格
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
            TilePainter.Instance.Painting("Yellow", 2, _moveRangeList[i]);
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
        _actionCount = _maxActionCount;
    }

    public void ActionDone()
    {
        _actionCount--;
    }

    public void GetSkillDistance()
    {
        Vector2Int orign = Vector2Int.FloorToInt(transform.position);
        SelectedSkill.GetSkillDistance(orign, this, BattleController.Instance.CharacterList);
    }

    public bool IsInSkillDistance(Vector2Int position)
    {
        return SelectedSkill.IsInSkillDistance(position);
    }

    public void GetSkillRange()
    {
        Vector2Int orign = Vector2Int.FloorToInt(transform.position);
        SelectedSkill.GetSkillRange(TargetPosition, orign, this, BattleController.Instance.CharacterList);
    }

    public void SetTarget(Vector2Int position)
    {
        TargetPosition = position;
    }

    public bool IsTarget(Vector2Int position)
    {
        return position == TargetPosition;
    }

    public void UseSkill(Action callback)
    {
        if (SelectedSkill != null)
        {
            SelectedSkill.Use(this, callback);
        }
        else
        {
            if (callback != null)
            {
                callback();
            }
        }
    }

    public virtual void SetDamage(BattleCharacter executor, SkillData.RootObject skillData, Action<BattleCharacter> callback)
    {
    }

    public virtual void SetPoisonDamage(Action callback) //回合結束時計算毒傷害
    {
    }

    public virtual void SetRecover(int recover, Action<BattleCharacter> callback)
    {
    }

    public void SetBuff(int id, Action<BattleCharacter> callback)
    {
        Buff buff;

        if (!StatusDic.ContainsKey(id))
        {
            buff = new Buff(id);
            StatusDic.Add(id, buff);
        }
        else
        {
            buff = (Buff)StatusDic[id];
            buff.ResetTurn();
        }

        BattleUI.Instance.SetStatus(this, buff.Comment, FloatingNumber.Type.Other, () =>
        {
            callback(this);
        });
    }

    public void SetOutline(bool show)
    {
        SpriteOutline.SetOutline(show);
    }

    protected HitType CheckHit(BattleCharacter executor)
    {
        float misssRate;
        misssRate = (float)(AGI - executor.SEN) / (float)AGI; //迴避率

        if (misssRate >= 0) //迴避率為正,骰迴避
        {
            if (misssRate < UnityEngine.Random.Range(0f, 1f))
            {
                return HitType.Hit;
            }
            else
            {
                return HitType.Miss;
            }
        }
        else //迴避率為負,骰爆擊
        {
            if (misssRate < UnityEngine.Random.Range(0f, 1f) * -1f)
            {
                return HitType.Critical;
            }
            else
            {
                return HitType.Hit;
            }
        }
    }

    protected int CalculateDamage(BattleCharacter executor, SkillData.RootObject skill, bool isCritical)
    {
        int damage;
        if (skill.IsMagic)
        {
            damage = Mathf.RoundToInt(((float)executor.MTK / (float)MEF) * skill.Damage);
        }
        else
        {
            damage = Mathf.RoundToInt(((float)executor.ATK / (float)DEF) * skill.Damage);
        }
        if (isCritical)
        {
            damage = (int)(damage * 1.5f);
            Debug.Log("爆擊");
        }
        if (IsSleeping)
        {
            damage = (int)(damage * 2f);
        }
        damage = (int)(damage * (UnityEngine.Random.Range(100f, 110f) / 100f)); //加上10%的隨機傷害

        return damage;
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

    private float GetBuffATK()
    {
        float buffAtk = 1;
        Buff buff;

        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value is Buff)
            {
                buff = (Buff)item.Value;
                buffAtk *= buff.ATK;
            }
        }

        return buffAtk;
    }

    private float GetBuffDEF()
    {
        float buffDef = 1;
        Buff buff;

        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value is Buff)
            {
                buff = (Buff)item.Value;
                buffDef *= buff.DEF;
            }
        }

        return buffDef;
    }

    private float GetBuffMTK()
    {
        float buffMtk = 1;
        Buff buff;

        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value is Buff)
            {
                buff = (Buff)item.Value;
                buffMtk *= buff.MTK;
            }
        }

        return buffMtk;
    }

    private float GetBuffMEF()
    {
        float buffMef = 1;
        Buff buff;

        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value is Buff)
            {
                buff = (Buff)item.Value;
                buffMef *= buff.MEF;
            }
        }

        return buffMef;
    }

    private float GetBuffAGI()
    {
        float buffAgi = 1;
        Buff buff;

        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value is Buff)
            {
                buff = (Buff)item.Value;
                buffAgi *= buff.AGI;
            }
        }

        return buffAgi;
    }

    private float GetBuffSEN()
    {
        float buffSen = 1;
        Buff buff;

        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value is Buff)
            {
                buff = (Buff)item.Value;
                buffSen *= buff.SEN;
            }
        }

        return buffSen;
    }
}
