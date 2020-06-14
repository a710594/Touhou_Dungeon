using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterInfo
{
    private static readonly int _maxActionCount = 2;

    public enum LiveStateEnum
    {
        Alive,
        Dying,
        Dead,
    }

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
            }
            else if (_currentHP > MaxHP)
            {
                _currentHP = MaxHP;
            }
        }
    }

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

    public int _atk; //不含buff的總和攻擊力
    public int ATK
    {
        get
        {
            return Mathf.RoundToInt((float)_atk * GetBuffATK() * BattleFieldManager.Instance.GetFieldBuff(_position).ATK);
        }
    }

    public int EquipATK; //裝備的攻擊力

    public int _def; //不含buff的總和防禦力
    public int DEF
    {
        get
        {
            return Mathf.RoundToInt((float)_def * GetBuffDEF() * BattleFieldManager.Instance.GetFieldBuff(_position).DEF);
        }
    }

    public int EquipDEF; //裝備的防禦力

    public int _mtk; //不含buff的總和魔法攻擊力
    public int MTK
    {
        get
        {
            return Mathf.RoundToInt((float)_mtk * GetBuffMTK() * BattleFieldManager.Instance.GetFieldBuff(_position).MTK);
        }
    }

    public int EquipMTK; //裝備的魔法攻擊力

    public int _mef; //不含buff的總和魔法防禦力
    public int MEF
    {
        get
        {
            return Mathf.RoundToInt((float)_mef * GetBuffMEF() * BattleFieldManager.Instance.GetFieldBuff(_position).MEF);
        }
    }

    public int EquipMEF; //裝備的魔法防禦力

    public int _agi; //不含buff的敏捷(影響迴避)
    public int AGI
    {
        get
        {
            return Mathf.RoundToInt((float)_agi * GetBuffAGI() * BattleFieldManager.Instance.GetFieldBuff(_position).AGI);
        }
    }

    public int _sen; //不含buff的感知(影響命中)
    public int SEN
    {
        get
        {
            return Mathf.RoundToInt((float)_sen * GetBuffSEN() * BattleFieldManager.Instance.GetFieldBuff(_position).SEN);
        }
    }

    public int _mov; //不含buff的移動距離
    public int MOV
    {
        get
        {
            return _mov;
        }
    }

    public bool IsPoisoning
    {
        get
        {
            return PoisonDic.Count > 0;
        }
    }

    public bool IsSleeping
    {
        get
        {
            return SleepingId != -1;
        }
    }

    public bool IsParalysis
    {
        get
        {
            return Random.Range(0, 100) < ParalysisProbability;
        }
    }

    private int _actionCount;
    public int ActionCount
    {
        get
        {
            return _actionCount;
        }
    }

    public bool HasUseSkill = false;
    public int ID;
    public int Lv;
    public string Name;
    public LiveStateEnum LiveState;
    public FoodBuff FoodBuff = null;

    public Dictionary<int, BattleStatus> StatusDic = new Dictionary<int, BattleStatus>();
    public int SleepingId = -1;
    public float ParalysisProbability = 0;
    public Dictionary<int, int> PoisonDic = new Dictionary<int, int>(); //id, damage

    private Vector2 _position = new Vector2();

    public void Init(TeamMember member) //for player
    {
        ID = member.Data.ID;
        Lv = member.Lv;
        Name = member.Data.GetName();
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
        _mov = member.MOV;
        EquipATK = member.Weapon.ATK;
        EquipDEF = member.Armor.DEF;
        EquipMTK = member.Weapon.MTK;
        EquipMEF = member.Armor.MEF;
        if (!member.FoodBuff.IsEmpty)
        {
            FoodBuff = member.FoodBuff;
        }
    }

    public void Init(BattlePlayerMemo memo)
    {
        ID = memo.ID;
        Lv = memo.Lv;
        Name = JobData.GetData(memo.ID).GetName();
        MaxHP = memo.MaxHP;
        CurrentHP = memo.CurrentHP;
        MaxMP = memo.MaxMP;
        CurrentMP = memo.CurrentMP;
        _atk = memo.ATK;
        _def = memo.DEF;
        _mtk = memo.MTK;
        _mef = memo.MEF;
        _agi = memo.AGI;
        _sen = memo.SEN;
        _mov = memo.MOV;
        EquipATK = memo.EquipATK;
        EquipDEF = memo.EquipDEF;
        EquipMTK = memo.EquipMTK;
        EquipMEF = memo.EquipMEF;
        _actionCount = memo.ActionCount;
        HasUseSkill = memo.HasUseSkill;
        FoodBuff = memo.FoodBuff;

        StatusDic = memo.StatusDic;
        SleepingId = memo.SleepingId;
        ParalysisProbability = memo.ParalysisProbability;
        PoisonDic = memo.PoisonDic;
    }

    public virtual void Init(int id, int lv) //for enemy
    {
        EnemyData.RootObject data = EnemyData.GetData(id);

        ID = id;
        Lv = lv;
        Name = data.Name;
        MaxHP = Mathf.RoundToInt(data.HP * (1 + (lv - 1) * 0.1f));
        CurrentHP = MaxHP;
        _atk = Mathf.RoundToInt(data.ATK * (1 + (lv - 1) * 0.1f));
        _def = Mathf.RoundToInt(data.DEF * (1 + (lv - 1) * 0.1f));
        _mtk = Mathf.RoundToInt(data.MTK * (1 + (lv - 1) * 0.1f));
        _mef = Mathf.RoundToInt(data.MEF * (1 + (lv - 1) * 0.1f));
        _agi = Mathf.RoundToInt(data.AGI * (1 + (lv - 1) * 0.1f));
        _sen = Mathf.RoundToInt(data.SEN * (1 + (lv - 1) * 0.1f));
        _mov = data.MOV;
        EquipATK = data.Equip_ATK;
        EquipDEF = data.Equip_DEF;
        EquipMTK = data.Equip_MTK;
        EquipMEF = data.Equip_MEF;
    }

    public void Init(BattleEnemyMemo memo) 
    {
        Init(memo.ID, memo.Lv);
        CurrentHP = memo.CurrentHP;
        StatusDic = memo.StatusDic;
        SleepingId = memo.SleepingId;
        ParalysisProbability = memo.ParalysisProbability;
        PoisonDic = memo.PoisonDic;
    }

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public void CheckBattleStatus()
    {
        //不能在 foreach 中刪除元件,所以要用 for
        List<int> keyList = new List<int>(StatusDic.Keys);
        List<BattleStatus> statusList = new List<BattleStatus>(StatusDic.Values);

        for (int i = 0; i < keyList.Count; i++)
        {
            if (statusList[i].RemainTurn != -1) //-1代表永久
            {
                statusList[i].RemainTurn--;
                if (statusList[i].RemainTurn == 0)
                {
                    if (statusList[i] is Poison)
                    {
                        PoisonDic.Remove(keyList[i]);
                    }
                    else if (statusList[i] is Paralysis)
                    {
                        ParalysisProbability = 0;
                    }
                    else if (statusList[i] is Sleeping)
                    {
                        SleepingId = -1;
                    }

                    StatusDic.Remove(keyList[i]);
                }
            }
        }
    }

    public void InitActionCount()
    {
        HasUseSkill = false;
        _actionCount = _maxActionCount;
    }

    public void MoveDone()
    {
        _actionCount--;
    }

    public void MoveUndo()
    {
        _actionCount++;
    }

    public void SkillDone()
    {
        HasUseSkill = true;
        _actionCount--;
    }

    public void ActionDoneCompletely()
    {
        HasUseSkill = true;
        _actionCount = 0;
    }

    public void RemoveSleep()
    {
        StatusDic.Remove(SleepingId);
        SleepingId = -1;
    }

    public void SetPoison(int id, int damage)
    {
        Poison poison;

        if (!StatusDic.ContainsKey(id))
        {
            poison = new Poison(id);
            StatusDic.Add(id, poison);
            PoisonDic.Add(id, damage);
        }
        else
        {
            poison = (Poison)StatusDic[id];
            poison.ResetTurn();
        }
    }

    public List<int> GetPoisonDamageList()
    {
        return new List<int>(PoisonDic.Values);
    }

    public void ClearAbnormal()
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
                        PoisonDic.Remove(statusList[i].Data.ID);
                    }
                    else if (statusList[i] is Paralysis)
                    {
                        ParalysisProbability = 0;
                    }
                    else if (statusList[i] is Sleeping)
                    {
                        SleepingId = -1;
                    }

                    statusList[i].RemainTurn = 0;
                    StatusDic.Remove(keyList[i]);
                }
            }
        }
    }

    public void SetBuff(int id)
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
