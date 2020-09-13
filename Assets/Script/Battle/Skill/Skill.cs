using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skill
{
    public enum HitType
    {
        Miss,
        Hit,
        Critical,
        NoDamage,
    }

    public SkillData.RootObject Data;
    public int CurrentCD = 0;
    public int ItemID = 0;
    public int Lv;

    protected int _skillCallBackCount = 0;
    protected int _targetCount = 0;
    protected int _value;
    protected Vector3 _targetPosition;
    protected Action _skillCallback;
    protected Skill _subSkill;
    protected BattleCharacterInfo _user;
    protected List<Vector2Int> _skillDistanceList = new List<Vector2Int>();
    protected List<Vector2Int> _skillRangeList = new List<Vector2Int>();
    protected List<BattleCharacter> _targetList = new List<BattleCharacter>();

    public Skill() { }

    public Skill(SkillData.RootObject data, BattleCharacterInfo user, int lv)
    {
        _user = user;
        Data = data;
        Lv = lv;
        if (data.SubID != 0)
        {
            SkillData.RootObject skillData = SkillData.GetData(Data.SubID);
            _subSkill = SkillFactory.GetNewSkill(skillData, user, lv);
        }
    }

    public bool IsSpellCard
    {
        get
        {
            return Data.NeedPower > 0;
        }
    }
    public void SetCD()
    {
        if (CurrentCD > 0)
        {
            CurrentCD--;
        }
    }

    public virtual bool CanUse(out string notUseReason)
    {
        notUseReason = "";

        if (_user.CurrentMP < Data.MP)
        {
            notUseReason = "MP 不足";
            return false;
        }
        else if (CurrentCD > 0)
        {
            notUseReason = "還要 " + CurrentCD + " 回合才能使用";
            return false;
        }
        else if (Data.NeedPower > BattleController.Instance.Power)
        {
            notUseReason = "Power 不足";
            return false;
        }
        else if (Data.Priority < _user.CurrentPriority)
        {
            notUseReason = "還沒到可使用該技能的時機";
            return false;
        }
        //else if (Data.Priority > _user.CurrentPriority)
        //{
        //    notUseReason = "已經過了可使用該技能的時機";
        //    return false;
        //}

        return true;
    }

    //有可能出現沒有任何格子可選的情況,有空要修
    public List<Vector2Int> GetDistance(BattleCharacter executor)
    {
        TilePainter.Instance.Clear(2);
        _skillDistanceList.Clear();

        Vector2Int orign = Vector2Int.FloorToInt(executor.transform.position);
        List<Vector2Int> positionList = Utility.GetRhombusPositionList(Data.Distance, orign, false);
        List<BattleCharacter> characterList = BattleController.Instance.CharacterList;
        positionList = RemovePosition(executor, characterList, positionList);
        positionList = BattleFieldManager.Instance.RemoveBound(positionList);

        for (int i = 0; i < positionList.Count; i++)
        {
            _skillDistanceList.Add(positionList[i]);
        }
        return _skillDistanceList;
    }

    public virtual List<Vector2Int> GetRange(Vector2Int target, BattleCharacter executor, List<BattleCharacter> characterList)
    {
        _targetPosition = new Vector3(target.x, target.y, Camera.main.transform.position.z);
        List<Vector2Int> positionList = new List<Vector2Int>();
        TilePainter.Instance.Clear(2);
        _skillRangeList.Clear();

        if (Data.RangeType == SkillData.RangeTypeEnum.Point)
        {
            positionList.Add(target);
            positionList = RemovePosition(executor, characterList, positionList);
        }
        else if (Data.RangeType == SkillData.RangeTypeEnum.Circle)
        {
            positionList = Utility.GetRhombusPositionList(Data.Range_1 - 1, target, false);
            positionList = RemovePosition(executor, characterList, positionList);
            positionList = BattleFieldManager.Instance.RemoveBound(positionList);
        }
        else if (Data.RangeType == SkillData.RangeTypeEnum.Rectangle)
        {
            Vector2Int orign = Vector2Int.RoundToInt(executor.transform.position);
            positionList = Utility.GetRectanglePositionList(Data.Range_1, Data.Range_2, orign, target - orign);
            positionList = RemovePosition(executor, characterList, positionList);
            positionList = BattleFieldManager.Instance.RemoveBound(positionList);
        }
        else if (Data.RangeType == SkillData.RangeTypeEnum.All)
        {
            positionList = new List<Vector2Int>(BattleFieldManager.Instance.MapDic.Keys);
            positionList = RemovePosition(executor, characterList, positionList);
            positionList = BattleFieldManager.Instance.RemoveBound(positionList);
        }
        _skillRangeList = positionList;

        return _skillRangeList;
    }

    public bool IsInDistance(Vector2Int position)
    {
        return _skillDistanceList.Contains(position);
    }

    public List<BattleCharacter> GetTargetList()
    {
        BattleCharacter target;
        _targetList.Clear();
        for (int i = 0; i < _skillRangeList.Count; i++)
        {
            target = BattleController.Instance.GetCharacterByPosition(_skillRangeList[i]);
            if (target != null)
            {
                _targetList.Add(target);
            }
        }
        _targetCount = _targetList.Count;
        return _targetList;
    }

    public void SetUser(BattleCharacterInfo user)
    {
        _user = user;
        if (_subSkill != null)
        {
            _subSkill.SetUser(_user);
        }
    }

    public virtual void Use(Action callback)
    {
        if (Data.CD > 0)
        {
            CurrentCD = Data.CD + 1; //要加一是因為本回合不減CD
        }
        InitSkillCallbackCount();
        if (_subSkill != null)
        {
            _subSkill.InitSkillCallbackCount();
        }
        _skillCallback = callback;
        GetTargetList();

        if (IsSpellCard)
        {
            string largeImage = "";
            if (_user.JobData != null)
            {
                largeImage = _user.JobData.LargeImage;
            }
            else if (_user.EnemyData != null)
            {
                largeImage = _user.EnemyData.LargeImage;
            }

            BattleUI.Instance.ShowSpellCard(Data.GetName(), largeImage, ()=> 
            {
                CameraMove(UseCallback);
            });
        }
        else
        {
            BattleUI.Instance.SetSkillLabel(true, Data.GetName());

            Camera.main.transform.DOMove(_targetPosition + Vector3.up, Vector2.Distance(_user.Position, _targetPosition) / 4f).OnComplete(() =>
            {
                CameraMove(UseCallback);
            });
        }

        if (ItemID != 0)
        {
            ItemManager.Instance.MinusItem(ItemID, 1, ItemManager.Type.Bag);
        }
    }

    public void InitSkillCallbackCount()
    {
        _skillCallBackCount = 0;
        if (_subSkill != null)
        {
            _subSkill.InitSkillCallbackCount();
        }
    }

    protected HitType hitType;
    public virtual void SetEffect(BattleCharacter target)
    {
        if (target != null)
        {
            hitType = CheckHit(_user, target.Info, target.LiveState);
        }
    }

    protected virtual HitType CheckHit(BattleCharacterInfo executor, BattleCharacterInfo target, BattleCharacter.LiveStateEnum targetLiveState)
    {
        float misssRate;
        misssRate = (float)(target.AGI - executor.SEN * (Data.HitRate / 100f)) / (float)target.AGI; //迴避率

        if (misssRate >= 0) //迴避率為正,骰迴避
        {
            if (misssRate < UnityEngine.Random.Range(0f, 1f))
            {
                return HitType.Hit;
            }
            else
            {
                if (targetLiveState == BattleCharacter.LiveStateEnum.Dying)
                {
                    return HitType.Hit;
                }
                else
                {
                    return HitType.Miss;
                }
            }
        }
        else //迴避率為負,骰爆擊
        {
            //if (misssRate < UnityEngine.Random.Range(0f, 1f) * -1f)
            //{
            //    return HitType.Critical;
            //}
            //else
            //{
            //    return HitType.Hit;
            //}
            return HitType.Hit;
        }
    }

    protected virtual void UseCallback() //Use 之後會呼叫的方法
    {
        if (_targetList.Count > 0 && _user.Camp == BattleCharacterInfo.CampEnum.Partner && !IsSpellCard)
        {
            BattleController.Instance.AddPower(_targetList.Count * Data.AddPower);
            BattleUI.Instance.DropPowerPoint(_targetList);
        }

        if (Data.ParticleName != "x")
        {
            GameObject particle;
            for (int i = 0; i < _skillRangeList.Count; i++)
            {
                particle = ResourceManager.Instance.Spawn("Particle/" + Data.ParticleName, ResourceManager.Type.Other);
                particle.transform.position = _skillRangeList[i] + Vector2.up; // + Vector2.up 是為了調整特效生成的位置
            }
        }
    }

    protected void CheckSkillCallback(BattleCharacter target)
    {
        if (_subSkill != null)
        {
            _subSkill.SetCallback(_targetCount, _skillCallback);
            if (_subSkill is FieldSkill)
            {
                ((FieldSkill)_subSkill).SetSkillRange(_skillRangeList);
                _subSkill.SetEffect(null);
            }
            else
            {
                _subSkill.SetEffect(target);
            }
        }
        else
        {
            OnSkillEnd();
        }
    }

    protected void OnSkillEnd()
    {
        //這幾種技能的目標比較特殊,不走一般計算目標數的流程
        if (Data.Type == SkillData.TypeEnum.Field || Data.Type == SkillData.TypeEnum.CureLeastHP || Data.Type == SkillData.TypeEnum.Summon)
        {
            BattleUI.Instance.SetSkillLabel(false);
            _skillCallback();
        }
        else
        {
            _skillCallBackCount++;
            if (_skillCallBackCount == _targetCount && _skillCallback != null)
            {
                BattleUI.Instance.SetSkillLabel(false);
                _skillCallback();
            }
        }
    }

    //移除非技能目標
    protected List<Vector2Int> RemovePosition(BattleCharacter executor, List<BattleCharacter> characterList, List<Vector2Int> positionList)
    {
        if (Data.Target == SkillData.TargetType.None)
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
            }
        }
        else if (Data.Target == SkillData.TargetType.Us)
        {
            if (executor.Info.Camp == BattleCharacterInfo.CampEnum.Enemy)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Partner && characterList[i].LiveState != BattleCharacter.LiveStateEnum.Dead)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
            else if (executor.Info.Camp == BattleCharacterInfo.CampEnum.Partner)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Enemy && characterList[i].LiveState != BattleCharacter.LiveStateEnum.Dead)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
        }
        else if (Data.Target == SkillData.TargetType.Them)
        {
            if (executor.Info.Camp == BattleCharacterInfo.CampEnum.Enemy)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Enemy && characterList[i].LiveState != BattleCharacter.LiveStateEnum.Dead)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
            else if (executor.Info.Camp == BattleCharacterInfo.CampEnum.Partner)
            {
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Partner && characterList[i].LiveState != BattleCharacter.LiveStateEnum.Dead)
                    {
                        positionList.Remove(Vector2Int.RoundToInt(characterList[i].transform.position));
                    }
                }
            }
        }

        return positionList;
    }

    private void SetCallback(int targetCount, Action callback) //給 subSkill 用的
    {
        _targetCount = targetCount;
        _skillCallback = callback;
    }

    private void CameraMove(Action callback)
    {
        float distance = Vector2.Distance(_user.Position, _targetPosition);

        if (distance > 1)
        {
            Camera.main.transform.DOMove(_targetPosition + Vector3.up, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                callback();
            });
        }
        else
        {
            callback();
        }
    }
}
