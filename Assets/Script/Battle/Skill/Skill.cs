﻿using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    protected static float _floatingNumberTime = 0.25f;

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
    protected bool _hasNoTarget; //有些種類的技能天生沒有目標
    protected Vector3 _targetPosition;
    protected Action _skillCallback;
    protected Skill _subSkill;
    protected Skill _partnerSkill;
    protected BattleCharacterInfo _user;
    protected List<Vector2Int> _skillDistanceList = new List<Vector2Int>();
    protected List<Vector2Int> _skillRangeList = new List<Vector2Int>();
    protected List<BattleCharacter> _targetList = new List<BattleCharacter>();
    protected Dictionary<BattleCharacter, List<FloatingNumberData>> _floatingNumberDic = new Dictionary<BattleCharacter, List<FloatingNumberData>>();

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
            _subSkill.SetPartnerSkill(this);
        }
    }

    public void SetPartnerSkill(Skill skill)
    {
        _partnerSkill = skill;
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

        if (this is AttackSkill && Data.Distance == 0)
        {
            positionList.Remove(Vector2Int.FloorToInt(executor.transform.position));
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
        _skillCallback = callback;
        GetTargetList();

        _floatingNumberDic.Clear();
        if (_hasNoTarget)
        {
            SetEffect(null, _floatingNumberDic);
        }
        else
        {
            for (int i = 0; i < _targetList.Count; i++)
            {
                SetEffect(_targetList[i], _floatingNumberDic);
            }
        }

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
                CameraMove();
            });
        }
        else
        {
            BattleUI.Instance.SetSkillLabel(true, Data.GetName());
            CameraMove();
        }

        if (ItemID != 0)
        {
            ItemManager.Instance.MinusItem(ItemID, 1, ItemManager.Type.Bag);
        }
    }

    public bool CheckSkillCallbackCount()
    {
        //這幾種技能的目標比較特殊,不走一般計算目標數的流程
        if (_hasNoTarget)
        {
            return true;
        }
        else
        {
            _skillCallBackCount++;
            if (_skillCallBackCount == _targetCount)
            {
                _skillCallBackCount = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public virtual void SetEffect(BattleCharacter target, Dictionary<BattleCharacter, List<FloatingNumberData>> floatingNumberDic)
    {
    }

    protected virtual HitType CheckHit(BattleCharacterInfo executor, BattleCharacterInfo target, BattleCharacter.LiveStateEnum targetLiveState)
    {
        float hitRate = _user.SEN * (Data.HitRate / 100f) / target.AGI;
        if (hitRate > UnityEngine.Random.Range(0f, 1f))
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

    protected void CheckSubSkill(BattleCharacter target, HitType hitType)
    {
        if (_subSkill != null && hitType != HitType.Miss && hitType != HitType.NoDamage)
        {
            _subSkill.SetCallback(_targetCount, _skillCallback);
            if (_subSkill is FieldSkill)
            {
                ((FieldSkill)_subSkill).SetSkillRange(_skillRangeList);
                _subSkill.SetEffect(null, _floatingNumberDic);
            }
            else
            {
                _subSkill.SetEffect(target, _floatingNumberDic);
            }
            //CheckSkillCallbackCount();
        }
        else
        {
            OnSkillEnd();
        }
    }

    protected void OnSkillEnd()
    {
        if (CheckSkillCallbackCount())
        {
            //float totalShowTime = Data.ShowTime;
            //Skill skill = this;
            //while (skill._partnerSkill!= null)
            //{
            //    skill = skill._partnerSkill;
            //    totalShowTime += skill.Data.ShowTime;
            //}

            //Timer timer = new Timer(totalShowTime / 2f, ()=> 
            //{
                //foreach (KeyValuePair<BattleCharacter, List<FloatingNumberData>> item in _floatingNumberDic)
                //{
                //    BattleUI.Instance.SetFloatingNumber(item.Key, item.Value);
                //    BattleUI.Instance.SetLittleHPBar(item.Key, true);
                //}

                //BattleUI.Instance.SetSkillLabel(false);
                //_skillCallback();
            //});
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
                    if (Data.RangeType == SkillData.RangeTypeEnum.All && characterList[i] == executor) //如果技能範圍是全體,則不排除自己為射程範圍
                    {
                        continue;
                    }

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
                    if (Data.RangeType == SkillData.RangeTypeEnum.All && characterList[i] == executor) //如果技能範圍是全體,則不排除自己為射程範圍
                    {
                        continue;
                    }

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

    private void CameraMove()
    {
        float distance = Vector2.Distance(_user.Position, _targetPosition);

        if (distance > 1)
        {
            Camera.main.transform.DOMove(_targetPosition + Vector3.up, Vector2.Distance(_user.Position, _targetPosition) / 4f).OnComplete(() =>
            {
                ShowAnimation();
                ShowFloatingNumber();
                RunCallback();
            });
        }
        else
        {
            ShowAnimation();
            ShowFloatingNumber();
            RunCallback();
        }
    }

    protected virtual void ShowAnimation()
    {
        if (Data.Animation != "x")
        {
            GameObject particle;
            particle = ResourceManager.Instance.Spawn("Skill/" + Data.Animation, ResourceManager.Type.Other);
            particle.transform.position = ((Vector2)_targetPosition) + Vector2.up; // + Vector2.up 是為了調整特效生成的位置

            SpriteRenderer sprite = particle.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                if (Data.RangeType == SkillData.RangeTypeEnum.Point || Data.RangeType == SkillData.RangeTypeEnum.Circle)
                {
                    if (_user.Position.x < _targetPosition.x)
                    {
                        sprite.flipX = true;
                    }
                }
                else if (Data.RangeType == SkillData.RangeTypeEnum.Rectangle)
                {
                    if (_user.Position - (Vector2)_targetPosition == Vector2.left)
                    {
                        sprite.flipX = true;
                    }
                    else if (_user.Position - (Vector2)_targetPosition == Vector2.up)
                    {
                        sprite.transform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    else if (_user.Position - (Vector2)_targetPosition == Vector2.down)
                    {
                        sprite.transform.rotation = Quaternion.Euler(0, 0, 270);
                    }
                }
            }
        }


        Timer timer = new Timer();
        timer.Start(Data.ShowTime, () =>
        {
            if (_targetList.Count > 0 && _user.Camp == BattleCharacterInfo.CampEnum.Partner && !IsSpellCard)
            {
                BattleController.Instance.AddPower(_targetList.Count * Data.AddPower);
                BattleUI.Instance.DropPowerPoint(_targetList);
            }
        });
    }

    protected void ShowFloatingNumber()
    {
        Timer timer = new Timer(Data.ShowTime / 2f, () =>
        {
            foreach (KeyValuePair<BattleCharacter, List<FloatingNumberData>> item in _floatingNumberDic)
            {
                BattleUI.Instance.SetFloatingNumber(item.Key, item.Value);
                BattleUI.Instance.SetLittleHPBar(item.Key, true);
                item.Key.CheckLiveState();
            }
        });
    }

    protected void RunCallback()
    {
        Timer timer = new Timer();
        timer.Start(Data.ShowTime + 0.5f, () =>
        {
            BattleUI.Instance.SetSkillLabel(false);
            _skillCallback();
        });
    }

    protected void SetFloatingNumberDic(BattleCharacter character, FloatingNumber.Type type, string text)
    {
        if (!_floatingNumberDic.ContainsKey(character))
        {
            _floatingNumberDic.Add(character, new List<FloatingNumberData>());
        }
        _floatingNumberDic[character].Add(new FloatingNumberData(type, text));
    }
}
