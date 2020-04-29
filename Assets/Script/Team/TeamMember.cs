using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMember
{
    public int Lv;
    public int Exp;
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
            return Mathf.RoundToInt((_atk + Weapon.ATK + Helmet.ATK + Armor.ATK + Jewelry.ATK) * FoodBuff.ATK);
        }
    }
    public int DEF
    {
        get
        {
            return Mathf.RoundToInt((_def + Weapon.DEF + Helmet.DEF + Armor.DEF + Jewelry.DEF) * FoodBuff.DEF);
        }
    }
    public int MTK
    {
        get
        {
            return Mathf.RoundToInt((_mtk + Weapon.MTK + Helmet.MTK + Armor.MTK + Jewelry.MTK) * FoodBuff.MTK);
        }
    }
    public int MEF
    {
        get
        {
            return Mathf.RoundToInt((_mef + Weapon.MEF + Helmet.MEF + Armor.MEF + Jewelry.MEF) * FoodBuff.MEF);
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
    public int MoveDistance;
    public JobData.RootObject Data;
    public Equip Weapon = new Equip(EquipData.TypeEnum.Weapon);
    public Equip Helmet = new Equip(EquipData.TypeEnum.Helmet);
    public Equip Armor = new Equip(EquipData.TypeEnum.Armor);
    public Equip Jewelry = new Equip(EquipData.TypeEnum.Jewelry);
    public bool HasFoodBuff = false;
    public FoodBuff FoodBuff = new FoodBuff(); //食物的效果一次只會有一個,戰鬥結束後就會消失
    public List<int> SkillList = new List<int>();

    //這些是不含裝備的角色數值
    private int _atk;
    private int _def;
    private int _mtk;
    private int _mef;
    private int _agi;
    private int _sen;
    private Equip _defaultWeapon = new Equip(EquipData.TypeEnum.Weapon);
    private Equip _defaultHelmet = new Equip(EquipData.TypeEnum.Helmet);
    private Equip _defaultArmor = new Equip(EquipData.TypeEnum.Armor);
    private Equip _defaultJewelry = new Equip(EquipData.TypeEnum.Jewelry);

    public void Init(int jobId, int lv = 1)
    {
        Data = JobData.GetData(jobId);
        Lv = lv;
        MaxHP = Mathf.RoundToInt(Data.HP * (1 + ((Lv - 1) * 0.1f)));
        CurrentHP = MaxHP;
        MaxMP = Mathf.RoundToInt(Data.MP * (1 + ((Lv - 1) * 0.1f)));
        CurrentMP = MaxMP;
        _atk = Mathf.RoundToInt(Data.ATK * (1 + ((Lv - 1) * 0.1f)));
        _def = Mathf.RoundToInt(Data.DEF * (1 + ((Lv - 1) * 0.1f)));
        _mtk = Mathf.RoundToInt(Data.MTK * (1 + ((Lv - 1) * 0.1f)));
        _mef= Mathf.RoundToInt(Data.MEF * (1 + ((Lv - 1) * 0.1f)));
        _agi = Mathf.RoundToInt(Data.AGI * (1 + ((Lv - 1) * 0.1f)));
        _sen = Mathf.RoundToInt(Data.SEN * (1 + ((Lv - 1) * 0.1f)));
        MoveDistance = Data.MoveDistance;
        SkillList = Data.GetUnlockSkill(Lv);
    }

    public void Refresh(BattleCharacterPlayer character)
    {
        CurrentHP = character.Info.CurrentHP;
        CurrentMP = character.Info.CurrentMP;
    }

    public void LvUp(int lv, int exp)
    {
        int originalMaxHP = MaxHP;
        int originalMaxMP = MaxMP;

        Lv = lv;
        Exp = exp;
        MaxHP = Mathf.RoundToInt(Data.HP * (1 + ((Lv - 1) * 0.1f)));
        CurrentHP += (MaxHP - originalMaxHP);
        MaxMP = Mathf.RoundToInt(Data.MP * (1 + ((Lv - 1) * 0.1f)));
        CurrentMP += (MaxMP - originalMaxMP);
        _atk = Mathf.RoundToInt(Data.ATK * (1 + ((Lv - 1) * 0.1f)));
        _def = Mathf.RoundToInt(Data.DEF * (1 + ((Lv - 1) * 0.1f)));
        _mtk = Mathf.RoundToInt(Data.MTK * (1 + ((Lv - 1) * 0.1f)));
        _mef = Mathf.RoundToInt(Data.MEF * (1 + ((Lv - 1) * 0.1f)));
        _agi = Mathf.RoundToInt(Data.AGI * (1 + ((Lv - 1) * 0.1f)));
        _sen = Mathf.RoundToInt(Data.SEN * (1 + ((Lv - 1) * 0.1f)));

        SkillList = Data.GetUnlockSkill(Lv);
    }

    public void SetEquip(int id) //for calculater
    {
        Equip oldEquip;
        SetEquip(new Equip(id), out oldEquip);
    }

    public void SetEquip(Equip equip, out Equip oldEquip)
    {
        oldEquip = null;
        if (equip.Type == EquipData.TypeEnum.Weapon)
        {
            oldEquip = Weapon;
            Weapon = equip;
        }
        else if (equip.Type == EquipData.TypeEnum.Helmet)
        {
            oldEquip = Helmet;
            Helmet = equip;
        }
        else if (equip.Type == EquipData.TypeEnum.Armor)
        {
            oldEquip = Armor;
            Armor = equip;
        }
        else if (equip.Type == EquipData.TypeEnum.Jewelry)
        {
            oldEquip = Jewelry;
            Jewelry = equip;
        }
    }

    public void TakeOffEquip(Equip equip, ItemManager.Type type)
    {
        if (equip.Type == EquipData.TypeEnum.Weapon)
        {
            Weapon = _defaultWeapon;
        }
        else if (equip.Type == EquipData.TypeEnum.Helmet)
        {
            Helmet = _defaultHelmet;
        }
        else if (equip.Type == EquipData.TypeEnum.Armor)
        {
            Armor = _defaultArmor;
        }
        else if (equip.Type == EquipData.TypeEnum.Jewelry)
        {
            Jewelry = _defaultJewelry;
        }

        ItemManager.Instance.AddItem(equip, 1, type);
    }

    public void SetFoodBuff(int id)
    {
        FoodBuff = new FoodBuff(id);
        HasFoodBuff = true;
    }

    public void ClearFoodBuff()
    {
        FoodBuff.Clear();
        HasFoodBuff = false;
    }

    public bool IsUnlockSkill(int skillId)
    {
        return SkillList.Contains(skillId);
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
