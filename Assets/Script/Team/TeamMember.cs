using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMember
{
    private static readonly int _maxSkillLv = 5;

    public int MaxHP;

    private int _currentHP;
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

    private int _currentMP;
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

    public int ATK
    {
        get
        {
            return Mathf.RoundToInt(_atk * FoodBuff.ATK);
        }
    }
    public int DEF
    {
        get
        {
            return Mathf.RoundToInt(_def * FoodBuff.DEF);
        }
    }
    public int MTK
    {
        get
        {
            return Mathf.RoundToInt(_mtk * FoodBuff.MTK);
        }
    }
    public int MEF
    {
        get
        {
            return Mathf.RoundToInt(_mef * FoodBuff.MEF);
        }
    }
    public int AGI
    {
        get
        {
            return Mathf.RoundToInt(_agi * FoodBuff.AGI);
        }
    }
    public int SEN
    {
        get
        {
            return Mathf.RoundToInt(_sen * FoodBuff.SEN);
        }
    }
    public int MOV;
    public Vector2Int Formation; //隊伍位置
    public JobData.RootObject Data;
    public Equip Weapon = new Equip(EquipData.TypeEnum.Weapon);
    public Equip Armor = new Equip(EquipData.TypeEnum.Armor);
    public FoodBuff FoodBuff = new FoodBuff(); //食物的效果一次只會有一個,戰鬥結束後就會消失
    public Dictionary<int, int> SkillDic = new Dictionary<int, int>(); //id, lv
    public Dictionary<int, int> SpellCardDic = new Dictionary<int, int>(); //id, lv

    //這些屬性被設為 public 是為了讓 TeamMemberMemo 讀,不會被其他 class 使用
    public int _atk;
    public int _def;
    public int _mtk;
    public int _mef;
    public int _agi;
    public int _sen;

    private Equip _defaultWeapon = new Equip(EquipData.TypeEnum.Weapon);
    private Equip _defaultArmor = new Equip(EquipData.TypeEnum.Armor);

    public void Init(int jobId, int lv = 1)
    {
        Data = JobData.GetData(jobId);
        MaxHP = Mathf.RoundToInt(Data.HP * (1 + ((lv - 1) * 0.1f)));
        CurrentHP = MaxHP;
        MaxMP =Data.MP;
        CurrentMP = MaxMP;
        _atk = Mathf.RoundToInt(Data.ATK * (1 + ((lv - 1) * 0.1f)));
        _def = Mathf.RoundToInt(Data.DEF * (1 + ((lv - 1) * 0.1f)));
        _mtk = Mathf.RoundToInt(Data.MTK * (1 + ((lv - 1) * 0.1f)));
        _mef= Mathf.RoundToInt(Data.MEF * (1 + ((lv - 1) * 0.1f)));
        _agi = Mathf.RoundToInt(Data.AGI * (1 + ((lv - 1) * 0.1f)));
        _sen = Mathf.RoundToInt(Data.SEN * (1 + ((lv - 1) * 0.1f)));
        MOV = Data.MOV;
        //SkillList = Data.GetUnlockSkill(Lv);
        for (int i=0; i<Data.SkillList.Count; i++)
        {
            SkillDic.Add(Data.SkillList[i], 1);
        }
        for (int i = 0; i < Data.SpellCardList.Count; i++)
        {
            SpellCardDic.Add(Data.SpellCardList[i], 1);
        }
    }

    public void Init(TeamMemberMemo memo) 
    {
        Data = JobData.GetData(memo.DataId);
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
        MOV = memo.MOV;
        SkillDic = memo.SkillList;
        SpellCardDic = memo.SpellCardList;
        Formation = memo.Formation;
        Weapon = memo.Weapon;
        Armor = memo.Armor;
        FoodBuff = memo.FoodBuff;
    }

    public void Refresh(BattleCharacter character)
    {
        CurrentHP = character.Info.CurrentHP;
        CurrentMP = character.Info.CurrentMP;
    }

    public void LvUp(int lv)
    {
        int originalMaxHP = MaxHP;
        int originalMaxMP = MaxMP;

        MaxHP = Mathf.RoundToInt(Data.HP * (1 + ((lv - 1) * 0.1f)));
        CurrentHP += (MaxHP - originalMaxHP);
        //MaxMP = Mathf.RoundToInt(Data.MP * (1 + ((Lv - 1) * 0.1f)));
        CurrentMP += (MaxMP - originalMaxMP);
        _atk = Mathf.RoundToInt(Data.ATK * (1 + ((lv - 1) * 0.1f)));
        _def = Mathf.RoundToInt(Data.DEF * (1 + ((lv - 1) * 0.1f)));
        _mtk = Mathf.RoundToInt(Data.MTK * (1 + ((lv - 1) * 0.1f)));
        _mef = Mathf.RoundToInt(Data.MEF * (1 + ((lv - 1) * 0.1f)));
        _agi = Mathf.RoundToInt(Data.AGI * (1 + ((lv - 1) * 0.1f)));
        _sen = Mathf.RoundToInt(Data.SEN * (1 + ((lv - 1) * 0.1f)));

        //SkillDic = Data.GetUnlockSkill(Lv);
    }

    public void SkillLvUp(int id)
    {
        if (SkillDic[id] < _maxSkillLv)
        {
            SkillDic[id]++;
        }
    }

    public void SpellCardLvUp(int id)
    {
        if (SpellCardDic[id] < _maxSkillLv)
        {
            SpellCardDic[id]++;
        }
    }

    public void SetEquip(int id)
    {
        SetEquip(new Equip(id), out Equip oldEquip);
    }

    public void SetEquip(Equip equip, out Equip oldEquip)
    {
        oldEquip = null;
        if (equip.EquipType == EquipData.TypeEnum.Weapon)
        {
            oldEquip = Weapon;
            Weapon = equip;
        }
        else if (equip.EquipType == EquipData.TypeEnum.Armor)
        {
            oldEquip = Armor;
            Armor = equip;
        }
        equip.SetOwner(Data.GetName());
        if (oldEquip != null)
        {
            oldEquip.SetOwner(string.Empty);
        }
    }

    public void TakeOffEquip(EquipData.TypeEnum type, out Equip oldEquip)
    {
        oldEquip = null;
        if (type == EquipData.TypeEnum.Weapon)
        {
            //Weapon = _defaultWeapon;
            SetEquip(_defaultWeapon, out oldEquip);
        }
        else if (type == EquipData.TypeEnum.Armor)
        {
            //Armor = _defaultArmor;
            SetEquip(_defaultArmor, out oldEquip);
        }

        //ItemManager.Instance.AddEquip(type, equip);
    }

    public void SetFoodBuff(ItemEffectData.RootObject data)
    {
        FoodBuff = new FoodBuff(data);
    }

    public void ClearFoodBuff()
    {
        FoodBuff.Clear();
    }

    public bool IsUnlockSkill(int skillId)
    {
        return SkillDic.ContainsKey(skillId);
    }

    public void AddHP(int addHP) 
    {
        CurrentHP += addHP;
    }

    public void AddMP(int addMP)
    {
        CurrentMP += addMP;
    }

    public void RecoverCompletelyHP() 
    {
        CurrentHP = MaxHP;
    }

    public void RecoverCompletelyMP()
    {
        CurrentMP = MaxMP;
    }
}
