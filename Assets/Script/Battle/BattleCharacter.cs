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
    public Animator Animator;
    public Transform ValueBarAnchor;
    public Skill SelectedSkill;
    public SpriteOutline SpriteOutline;
    public GrayScale GrayScale;
    public List<Skill> SkillList = new List<Skill>();
    public List<Skill> SpellCardList = new List<Skill>();
    public Dictionary<int, BattleStatus> StatusDic = new Dictionary<int, BattleStatus>();

    protected int _sleepingId = -1;
    protected float _paralysisProbability = 0;
    protected Vector2 _originalPosition = new Vector2();
    protected Vector2Int _lookAt = Vector2Int.right;
    protected Queue<Vector2Int> _path;
    protected Dictionary<int, int> _poisonDic = new Dictionary<int, int>(); //id, damage
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
            return _poisonDic.Count > 0;
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
        //不能在 foreach 中刪除元件,所以要用 for
        List<int> keyList = new List<int>(StatusDic.Keys);
        List<BattleStatus> statusList = new List<BattleStatus>(StatusDic.Values);

        for(int i=0; i<keyList.Count; i++)
        {
            if (statusList[i].RemainTurn != -1) //-1代表永久
            {
                statusList[i].RemainTurn--;
                if (statusList[i].RemainTurn == 0)
                {
                    if (statusList[i] is Poison)
                    {
                        _poisonDic.Remove(keyList[i]);
                    }
                    else if (statusList[i] is Paralysis)
                    {
                        _paralysisProbability = 0;
                    }
                    else if (statusList[i] is Sleeping)
                    {
                        _sleepingId = -1;
                    }

                    StatusDic.Remove(keyList[i]);
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
        _actionCount = _maxActionCount;
    }

    public void ActionDone()
    {
        _actionCount--;
    }

    public void ActionDoneCompletely()
    {
        _actionCount = 0;
    }

    public void GetSkillDistance()
    {
        SelectedSkill.GetSkillDistance(this, BattleController.Instance.CharacterList);
    }

    public bool IsInSkillDistance(Vector2Int position)
    {
        if (SelectedSkill != null)
        {
            return SelectedSkill.IsInSkillDistance(position);
        }
        else
        {
            return false;
        }
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

    public virtual void UseSkill(Action callback)
    {
        SelectedSkill.Use(this, callback);
    }

    public virtual void SetDamage(int damage, AttackSkill.HitType hitType, Action<BattleCharacter> callback)
    {
        CurrentHP -= damage;

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
            if (CurrentHP <= 0)
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

        if (IsSleeping)
        {
            //解除睡眠狀態
            StatusDic.Remove(_sleepingId);
            _sleepingId = -1;
        }
    }

    public virtual void SetPoisonDamage(Action callback) //回合結束時計算毒傷害
    {
        int damage;
        int callbackCount = 0;
        foreach (KeyValuePair<int, int> item in _poisonDic)
        {
            CurrentHP -= item.Value;

            BattleUI.Instance.SetFloatingNumber(this, item.Value.ToString(), FloatingNumber.Type.Poison, () =>
            {
                callbackCount++;
                if (CurrentHP <= 0)
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
                    if (callbackCount == _poisonDic.Count && callback != null)
                    {
                        callback();
                    }
                }
            });
        }
    }

    public virtual void SetRecover(int recover, Action<BattleCharacter> callback)
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

    public void ClearAbnormal(Action<BattleCharacter> callback)
    {
        List<int> keyList = new List<int>(StatusDic.Keys);
        List<BattleStatus> statusList = new List<BattleStatus>(StatusDic.Values);

        for (int i = 0; i < keyList.Count; i++)
        {
            if (statusList[i].RemainTurn != -1)
            {
                if (statusList[i] is Poison || statusList[i] is Paralysis || statusList[i] is Sleeping)
                {
                    if (statusList[i] is Poison)
                    {
                        _poisonDic.Remove(statusList[i].Data.ID);
                    }
                    else if (statusList[i] is Paralysis)
                    {
                        _paralysisProbability = 0;
                    }
                    else if (statusList[i] is Sleeping)
                    {
                        _sleepingId = -1;
                    }

                    statusList[i].RemainTurn = 0;
                    StatusDic.Remove(keyList[i]);
                }
            }
        }

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

    public void SetPoison(int id, int damage, Action<BattleCharacter> callback)
    {
        Poison poison;

        if (!StatusDic.ContainsKey(id))
        {
            poison = new Poison(id);
            StatusDic.Add(id, poison);
            _poisonDic.Add(id, damage);
        }
        else
        {
            poison = (Poison)StatusDic[id];
            poison.ResetTurn();
        }

        BattleUI.Instance.SetStatus(this, poison.Comment, FloatingNumber.Type.Other, () =>
        {
            if (callback != null)
            {
                callback(this);
            }
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
