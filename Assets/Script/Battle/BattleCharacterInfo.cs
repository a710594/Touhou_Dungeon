using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterInfo
{
    public enum CampEnum
    {
        Partner = 0,
        Enemy,
        All,
        None,
    }

    private static readonly int _maxActionCount = 2;

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
            if (value < 0)
            {
                if (_currentHP > 0)
                {
                    _currentHP = 0;
                }
                else
                {
                    _currentHP = -1;
                }
            }
            else
            {
                _currentHP = value;
                if (_currentHP > MaxHP)
                {
                    _currentHP = MaxHP;
                }
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
            //return Mathf.RoundToInt((float)_atk * GetBuffATK() * BattleFieldManager.Instance.GetFieldBuff(Position).ATK);
            return Mathf.RoundToInt((float)_atk * GetBuff(BattleStatusData.TypeEnum.ATK));
        }
    }

    public int EquipATK; //裝備的攻擊力

    public int _def; //不含buff的總和防禦力
    public int DEF
    {
        get
        {
            //return Mathf.RoundToInt((float)_def * GetBuffDEF() * BattleFieldManager.Instance.GetFieldBuff(Position).DEF);
            return Mathf.RoundToInt((float)_def * GetBuff(BattleStatusData.TypeEnum.DEF));
        }
    }

    public int EquipDEF; //裝備的防禦力

    public int _mtk; //不含buff的總和魔法攻擊力
    public int MTK
    {
        get
        {
            //return Mathf.RoundToInt((float)_mtk * GetBuffMTK() * BattleFieldManager.Instance.GetFieldBuff(Position).MTK);
            return Mathf.RoundToInt((float)_mtk * GetBuff(BattleStatusData.TypeEnum.MTK));
        }
    }

    public int EquipMTK; //裝備的魔法攻擊力

    public int _mef; //不含buff的總和魔法防禦力
    public int MEF
    {
        get
        {
            //return Mathf.RoundToInt((float)_mef * GetBuffMEF() * BattleFieldManager.Instance.GetFieldBuff(Position).MEF);
            return Mathf.RoundToInt((float)_mef * GetBuff(BattleStatusData.TypeEnum.MEF));
        }
    }

    public int EquipMEF; //裝備的魔法防禦力

    public int _agi; //不含buff的敏捷(影響迴避)
    public int AGI
    {
        get
        {
            //return Mathf.RoundToInt((float)_agi * GetBuffAGI() * BattleFieldManager.Instance.GetFieldBuff(Position).AGI);
            return Mathf.RoundToInt((float)_agi * GetBuff(BattleStatusData.TypeEnum.AGI));
        }
    }

    public int _sen; //不含buff的感知(影響命中)
    public int SEN
    {
        get
        {
            //return Mathf.RoundToInt((float)_sen * GetBuffSEN() * BattleFieldManager.Instance.GetFieldBuff(Position).SEN);
            return Mathf.RoundToInt((float)_sen * GetBuff(BattleStatusData.TypeEnum.SEN));
        }
    }

    public int _mov; //不含buff的移動距離
    public int MOV
    {
        get
        {
            //return Mathf.RoundToInt((float)_mov + GetBuffMOV() + BattleFieldManager.Instance.GetFieldBuff(Position).MOV);
            return Mathf.RoundToInt((float)_mov + GetBuff(BattleStatusData.TypeEnum.MOV));
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

    public bool IsStriking
    {
        get
        {
            return StrikingId != -1;
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

    public bool IsAI = false;
    public bool IsTeamMember = false;
    public bool HasUseSkill = false;
    public bool IsActive = true; //對 NPC 而言,代表是否會行動.對 PC 而言則必為 true
    public int Lv;
    public int CurrentPriority;
    public string Name;
    public Vector2 Position = new Vector2();
    public CampEnum Camp;
    public FoodBuff FoodBuff = null;
    public JobData.RootObject JobData;
    public EnemyData.RootObject EnemyData;

    public Queue<int> HPQueue = new Queue<int>();

    public Dictionary<int, BattleStatus> StatusDic = new Dictionary<int, BattleStatus>();
    public int SleepingId = -1;
    public int StrikingId = -1;
    public Dictionary<int, int> ParalysisDic = new Dictionary<int, int>(); //id, Probability
    public Dictionary<int, int> PoisonDic = new Dictionary<int, int>(); //id, damage

    public List<int> PriorityList = new List<int>(); //技能中有的優先值
    public List<Skill> SkillList = new List<Skill>();
    public List<Skill> SpellCardList = new List<Skill>();

    public void Init(TeamMember member) //for player
    {
        IsAI = false;
        IsTeamMember = true;
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
        Camp = CampEnum.Partner;
        JobData = member.Data;
        if (!member.FoodBuff.IsEmpty)
        {
            FoodBuff = member.FoodBuff;
        }

        int id;
        SkillData.RootObject skillData;
        Skill skill;
        foreach (KeyValuePair<int, int> item in member.SkillDic)
        {
            id = item.Key;
            skillData = SkillData.GetData(id);
            skill = SkillFactory.GetNewSkill(skillData, this, item.Value);
            //if (skillData.CD > 0 && BattleController.Instance != null)
            //{
            //    BattleController.Instance.TurnEndHandler += skill.SetCD;
            //}

            if (!PriorityList.Contains(skill.Data.Priority))
            {
                PriorityList.Add(skill.Data.Priority);
            }

            SkillList.Add(skill);
        }

        foreach (KeyValuePair<int, int> item in member.SpellCardDic)
        {
            id = item.Key;
            skillData = SkillData.GetData(id);
            skill = SkillFactory.GetNewSkill(skillData, this, item.Value);
            //if (skillData.CD > 0 && BattleController.Instance != null)
            //{
            //    BattleController.Instance.TurnEndHandler += skill.SetCD;
            //}

            if (!PriorityList.Contains(skill.Data.Priority))
            {
                PriorityList.Add(skill.Data.Priority);
            }

            SpellCardList.Add(skill);
        }
    }

    public void Init(BattleCharacterMemo memo)
    {
        IsActive = memo.IsActive;
        IsAI = memo.IsAI;
        IsTeamMember = memo.IsTeamMember;
        Lv = memo.Lv;
        HPQueue = memo.HPQueue;
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
        Camp = (CampEnum)memo.Camp;
        CurrentPriority = memo.CurrentPriority;
        FoodBuff = memo.FoodBuff;
        if (memo.JobID != 0)
        {
            JobData = global::JobData.GetData(memo.JobID);
            Name = JobData.GetName();
        }
        else if (memo.EnemyID != 0)
        {
            EnemyData = global::EnemyData.GetData(memo.EnemyID);
        }

        StatusDic = memo.StatusDic;
        SleepingId = memo.SleepingId;
        StrikingId = memo.StrikingId;
        ParalysisDic = memo.ParalysisDic;
        PoisonDic = memo.PoisonDic;

        SkillData.RootObject skillData;
        Skill skill;
        for (int i = 0; i < memo.SkillList.Count; i++)
        {
            skillData = SkillData.GetData(memo.SkillList[i]);
            skill = SkillFactory.GetNewSkill(skillData, this, memo.SkillLvList[i]);
            skill.CurrentCD = memo.SkillCdList[i];
            if (skillData.CD > 0)
            {
                BattleController.Instance.TurnEndHandler += skill.SetCD;
            }

            if (!PriorityList.Contains(skill.Data.Priority))
            {
                PriorityList.Add(skill.Data.Priority);
            }

            SkillList.Add(skill);
        }

        for (int i = 0; i < memo.SpellCardList.Count; i++)
        {
            skillData = SkillData.GetData(memo.SpellCardList[i]);
            skill = SkillFactory.GetNewSkill(skillData, this, memo.SpellCardLvList[i]);
            skill.CurrentCD = memo.SpellCardCdList[i];
            if (skillData.CD > 0)
            {
                BattleController.Instance.TurnEndHandler += skill.SetCD;
            }

            if (!PriorityList.Contains(skill.Data.Priority))
            {
                PriorityList.Add(skill.Data.Priority);
            }

            SpellCardList.Add(skill);
        }
    }

    public virtual void Init(int id, int lv) //for enemy
    {
        EnemyData = global::EnemyData.GetData(id);

        IsAI = true;
        IsTeamMember = false;
        Lv = lv;
        Name = EnemyData.Name;
        HPQueue = new Queue<int>(EnemyData.HPList);
        MaxHP = Mathf.RoundToInt(HPQueue.Dequeue() * (1 + (lv - 1) * 0.1f));
        CurrentHP = MaxHP;
        _atk = Mathf.RoundToInt(EnemyData.ATK * (1 + (lv - 1) * 0.1f));
        _def = Mathf.RoundToInt(EnemyData.DEF * (1 + (lv - 1) * 0.1f));
        _mtk = Mathf.RoundToInt(EnemyData.MTK * (1 + (lv - 1) * 0.1f));
        _mef = Mathf.RoundToInt(EnemyData.MEF * (1 + (lv - 1) * 0.1f));
        _agi = Mathf.RoundToInt(EnemyData.AGI * (1 + (lv - 1) * 0.1f));
        _sen = Mathf.RoundToInt(EnemyData.SEN * (1 + (lv - 1) * 0.1f));
        _mov = EnemyData.MOV;
        EquipATK = EnemyData.Equip_ATK;
        EquipDEF = EnemyData.Equip_DEF;
        EquipMTK = EnemyData.Equip_MTK;
        EquipMEF = EnemyData.Equip_MEF;
        Camp = CampEnum.Enemy;

        SkillData.RootObject skillData = SkillData.GetData(EnemyData.SkillList[0]);
        PriorityList.Add(skillData.Priority);
    }

    public virtual void Init(int id, int lv, int EquipAtk,int EquipMtk, int EquipDef, int EquipMef) //計算機中的玩家
    {
        JobData = global::JobData.GetData(id);

        IsAI = false;
        IsTeamMember = true;
        Lv = lv;
        Name = JobData.GetName();
        MaxHP = Mathf.RoundToInt(JobData.HP * (1 + (lv - 1) * 0.1f));
        CurrentHP = MaxHP;
        _atk = Mathf.RoundToInt(JobData.ATK * (1 + (lv - 1) * 0.1f));
        _def = Mathf.RoundToInt(JobData.DEF * (1 + (lv - 1) * 0.1f));
        _mtk = Mathf.RoundToInt(JobData.MTK * (1 + (lv - 1) * 0.1f));
        _mef = Mathf.RoundToInt(JobData.MEF * (1 + (lv - 1) * 0.1f));
        _agi = Mathf.RoundToInt(JobData.AGI * (1 + (lv - 1) * 0.1f));
        _sen = Mathf.RoundToInt(JobData.SEN * (1 + (lv - 1) * 0.1f));
        _mov = JobData.MOV;
        EquipATK = EquipAtk;
        EquipDEF = EquipDef;
        EquipMTK = EquipMtk;
        EquipMEF = EquipMef;
        Camp = CampEnum.Partner;

        SkillData.RootObject skillData;
        for (int i = 0; i < JobData.SkillList.Count; i++)
        {
            skillData = SkillData.GetData(JobData.SkillList[i]);
            if (!PriorityList.Contains(skillData.Priority))
            {
                PriorityList.Add(skillData.Priority);
            }
        }
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
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
                        ParalysisDic.Remove(keyList[i]);
                    }
                    else if (statusList[i] is Sleeping)
                    {
                        SleepingId = -1;
                    }
                    else if (statusList[i] is Striking)
                    {
                        StrikingId = -1;
                    }

                    StatusDic.Remove(keyList[i]);
                }
            }
        }
    }

    public bool CanAct(out BattleStatus battleStatus)
    {
        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value is Sleeping)
            {
                battleStatus = item.Value;
                return false;
            }
            else if (item.Value is Paralysis && Random.Range(0, 100) < ((Paralysis)item.Value).Probability)
            {
                battleStatus = item.Value;
                return false;
            }
        }
        battleStatus = null;
        return true;
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

    public void SetPoison(Poison poison, int damage)
    {
        int id = poison.Data.ID;
        if (!StatusDic.ContainsKey(id))
        {
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

    public void SetParalysis(int id, int lv)
    {
        Paralysis paralysis;

        if (!StatusDic.ContainsKey(id))
        {
            paralysis = new Paralysis(id, lv);
            StatusDic.Add(id, paralysis);
            ParalysisDic.Add(id, paralysis.Probability);
        }
        else
        {
            paralysis = (Paralysis)StatusDic[id];
            paralysis.ResetTurn();
        }
    }

    public void SetStriking(int id)
    {
        Striking striking;

        if (!StatusDic.ContainsKey(id))
        {
            striking = new Striking(id);
            StatusDic.Add(id, striking);
            StrikingId = id;
        }
        else
        {
            striking = (Striking)StatusDic[id];
            striking.ResetTurn();
        }
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
                    //statusList[i].RemainTurn = 0;
                    StatusDic.Remove(keyList[i]);
                }
            }
        }
        PoisonDic.Clear();
        ParalysisDic.Clear();
        SleepingId = -1;

    }

    public void SetBuff(int id, int lv)
    {
        Buff buff;

        if (!StatusDic.ContainsKey(id))
        {
            buff = new Buff(id, lv);
            StatusDic.Add(id, buff);
        }
        else
        {
            buff = (Buff)StatusDic[id];
            buff.ResetTurn();
        }
    }

    public void SetBuff(int atkBuff, int mtkBuff, int defBuff, int mefBuff) //計算機用的
    {
        Buff buff;
        StatusDic.Clear();
        if (atkBuff != 0)
        {
            buff = new Buff(BattleStatusData.TypeEnum.ATK, atkBuff);
            StatusDic.Add(0, buff);
        }
        if (mtkBuff != 0)
        {
            buff = new Buff(BattleStatusData.TypeEnum.MTK, mtkBuff);
            StatusDic.Add(1, buff);
        }
        if (defBuff != 0)
        {
            buff = new Buff(BattleStatusData.TypeEnum.DEF, defBuff);
            StatusDic.Add(2, buff);
        }
        if (mefBuff != 0)
        {
            buff = new Buff(BattleStatusData.TypeEnum.MEF, mefBuff);
            StatusDic.Add(3, buff);
        }
    }

    public void SetCurrentPriority(int priority)
    {
        CurrentPriority = priority;
    }

    private float GetBuff(BattleStatusData.TypeEnum valueType)
    {
        float value;
        float total;
        Buff buff;

        total = BattleFieldManager.Instance.GetFieldBuff(Position, valueType);

        foreach (KeyValuePair<int, BattleStatus> item in StatusDic)
        {
            if (item.Value is Buff && ((Buff)item.Value).Type == valueType)
            {
                buff = (Buff)item.Value;
                if (valueType == BattleStatusData.TypeEnum.MOV)
                {
                    value = buff.Value;
                    total += value;
                }
                else
                {
                    value = (float)buff.Value;
                    total *= value;
                }
            }
        }

        return total;
    }

    public void HPDequeue() 
    {
        MaxHP = Mathf.RoundToInt(HPQueue.Dequeue() * (1 + (Lv - 1) * 0.1f));
        CurrentHP = MaxHP;
    }
}
