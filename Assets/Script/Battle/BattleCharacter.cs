using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharacter : MonoBehaviour
{
    public enum NotActReason
    {
        Sleeping,
        Paralysis,
        None,
    }

    public enum LiveStateEnum
    {
        Alive,
        Dying,
        Dead,
    }

    public Action<BattleCharacter> GetDamageHandler;
    public Action OnDeathHandler;

    public Vector2Int TargetPosition = new Vector2Int();
    public SpriteRenderer Sprite;
    public Animator Animator;
    public Transform ValueBarAnchor;
    public Skill SelectedSkill;
    public SpriteOutline SpriteOutline;
    public GrayScale GrayScale;
    public AI AI;
    public BattleCharacterInfo Info = new BattleCharacterInfo();

    public bool IsActive = true;
    public int EnemyId;
    public int Lv;

    private int _getDamage = 0; //該回合累積的傷害

    public LiveStateEnum LiveState;

    public bool CanHitTarget
    {
        get 
        {
            return AI.CanHitTarget;
        }
    }

    protected Vector2 _originalPosition = new Vector2();
    protected Vector2 _lastPosition = new Vector2(); //上一步的位置
    protected Vector2Int _lookAt = Vector2Int.left;
    protected List<Vector2Int> _moveRangeList = new List<Vector2Int>();
    private List<Vector2Int> _detectRangeList = new List<Vector2Int>();


    public void Init(TeamMember member, int lv)
    {
        Info.Init(member, lv);
        Init(member.Data);
    }

    private void Init(JobData.RootObject data)
    {
        _originalPosition = transform.position;

        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + data.SmallImage);
        if (data.Animator != string.Empty)
        {
            Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/" + data.Animator);
        }

        if (LiveState == LiveStateEnum.Dying)
        {
            GrayScale.SetScale(0);
            Animator.SetBool("IsDying", true);
        }

        for (int i=0; i<Info.SkillList.Count; i++)
        {
            if (Info.SkillList[i].Data.CD > 0)
            {
                BattleController.Instance.TurnEndHandler += Info.SkillList[i].SetCD;
            }
        }

        for (int i = 0; i < Info.SpellCardList.Count; i++)
        {
            if (Info.SpellCardList[i].Data.CD > 0)
            {
                BattleController.Instance.TurnEndHandler += Info.SpellCardList[i].SetCD;
            }
        }

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }

    public void Init(int id, int lv)
    {
        EnemyData.RootObject data = EnemyData.GetData(id);

        Info.Init(id, lv);
        _originalPosition = transform.position;
        Sprite.sprite = Resources.Load<Sprite>("Image/Character/Small/" + data.Image);
        if (data.Animator != string.Empty)
        {
            Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/" + data.Animator);
        }
        AI = GetComponent(Type.GetType(data.AI)) as AI;
        if (AI == null)
        {
            gameObject.AddComponent(Type.GetType(data.AI));
            AI = GetComponent(Type.GetType(data.AI)) as AI;
        }
        AI.Init(this, data.SkillList);

        BattleController.Instance.TurnEndHandler += CheckBattleStatus;
    }

    public void InitByInspector() //使用 Inspector 上的資料來 Init
    {
        Init(EnemyId, Lv);
        Info.IsActive = IsActive;
    }

    public void CheckBattleStatus()
    {
        Info.CheckBattleStatus();
    }

    public bool CanAct(out BattleStatus battleStatus)
    {
        return Info.CanAct(out battleStatus); 
    }

    public void ReturnToOriginalPosition()
    {
        transform.position = _originalPosition;
    }

    public List<Vector2Int> GetMoveRange()
    {
        _moveRangeList.Clear();

        Vector2Int orign = Vector2Int.FloorToInt(_originalPosition);
        List<Vector2Int> positionList = Utility.GetRhombusPositionList(Info.MOV, orign, false);
        for (int i = 0; i < positionList.Count; i++)
        {
            if (BattleController.Instance.GetCharacterByPosition(positionList[i]) == null)
            {
                int pathLength = BattleFieldManager.Instance.GetPathLength(orign, positionList[i], Info.Camp);
                if (pathLength != -1 && pathLength - BattleFieldManager.Instance.GetField(orign).MoveCost <= Info.MOV) //扣掉自己那一格
                {
                    _moveRangeList.Add(positionList[i]);
                }
            }
        }
        _moveRangeList.Add(orign);
        return _moveRangeList;
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
        Queue<Vector2Int> path;
        List<Vector2Int> list = BattleFieldManager.Instance.GetPath(transform.position, position, Info.Camp);
        if (list != null)
        {
            path = new Queue<Vector2Int>(list);
        }
        else
        {
            path = null;
        }

        return path;
    }

    public void StartMove(Queue<Vector2Int> path, Action callback)
    {
        StartCoroutine(Move(path, callback));
    }

    public void InitActionCount()
    {
        Info.InitActionCount();
    }

    public void InitGetDamage()
    {
        Info.LastTurnGetDamage = _getDamage;
        _getDamage = 0;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
        _originalPosition = position;
        Info.SetPosition(position);
    }

    public void MoveDone()
    {
        _lastPosition = _originalPosition;
        _originalPosition = transform.position;
        Info.MoveDone();
        Info.SetPosition(transform.position);
    }

    public void SkillDone()
    {
        Info.SkillDone();
    }

    public bool CanUndoMove()
    {
        if (Info.ActionCount < 2 && !Info.HasUseSkill)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MoveUndo()
    {
        transform.position = _lastPosition;
        _originalPosition = _lastPosition;
        Info.MoveUndo();
    }

    public void ActionDoneCompletely()
    {
        Info.ActionDoneCompletely();
    }

    public void EscapeFail()
    {
        Info.EscapeFail();
    }

    public bool IsInSkillDistance(Vector2Int position)
    {
        if (SelectedSkill != null)
        {
            return SelectedSkill.IsInDistance(position);
        }
        else
        {
            return false;
        }
    }

    public void GetSkillRange(out Vector2Int target, out List<Vector2Int> rangeList)
    {
        target = TargetPosition;
        rangeList = SelectedSkill.GetRange(TargetPosition, this, BattleController.Instance.CharacterList);
    }

    public List<Vector2Int> GetDetectRange() //偵查範圍:移動後可用技能擊中目標的範圍
    {
        if (SelectedSkill == null)
        {
            return null;
        }

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

    public void SetTarget(Vector2Int position)
    {
        TargetPosition = position;
    }

    public bool IsTarget(Vector2Int position)
    {
        return position == TargetPosition;
    }

    public void SelectSkill(Skill skill)
    {
        SelectedSkill = skill;
    }

    public virtual void UseSkill(Action callback)
    {
        SelectedSkill.Use(callback);

        if (Info.IsTeamMember)
        {
            Info.CurrentMP -= SelectedSkill.Data.MP;
            BattleController.Instance.MinusPower(SelectedSkill.Data.NeedPower);
        }
    }

    public virtual void SetDamage(int damage)
    {
        Info.CurrentHP -= damage;
        _getDamage += damage;

        if (Info.IsSleeping)
        {
            //解除睡眠狀態
            Info.RemoveSleep();
        }

        if (Info.CurrentHP <= 0)
        {
            if (Info.HPQueue.Count > 0)
            {
                Info.HPDequeue();
            }
            else
            {
                if (!Info.IsTeamMember)
                {
                    LiveState = LiveStateEnum.Dead;
                }
                else
                {
                    if (LiveState == LiveStateEnum.Alive)
                    {
                        LiveState = LiveStateEnum.Dying;
                    }
                    else if (LiveState == LiveStateEnum.Dying)
                    {
                        LiveState = LiveStateEnum.Dead;
                    }
                }
            }
        }


        if (GetDamageHandler != null)
        {
            GetDamageHandler(this);
        }
    }

    public void SetPoison(Poison poison, int damage, Skill.HitType hitType, Action<BattleCharacter> callback)
    {
        Info.SetPoison(poison, damage);
    }

    public virtual void SetPoisonDamage() //回合結束時計算毒傷害
    {
        int totalDamage = 0;
        List<int> damageList = Info.GetPoisonDamageList();
        for(int i=0; i< damageList.Count; i++)
        {
            totalDamage += damageList[i];
        }
        SetDamage(totalDamage);
        BattleUI.Instance.SetFloatingNumber(this, totalDamage.ToString(), FloatingNumber.Type.Poison);
        BattleUI.Instance.SetLittleHPBar(this, true);
        CheckLiveState();
    }

    public void SetParalysis(int id, int lv)
    {
        Info.SetParalysis(id, lv);
    }

    public void SetSleep(int id)
    {
        Info.SetSleep(id);
    }

    public void SetStriking(int id)
    {
        Info.SetStriking(id);
    }

    public void SetRecoverHP(int recover)
    {
        if (LiveState == LiveStateEnum.Dying)
        {
            LiveState = LiveStateEnum.Alive;
            GrayScale.SetScale(1);
            Animator.SetBool("IsDying", false);
        }

        Info.CurrentHP += recover;
    }

    public void SetRecoverMP(int recover)
    {
        Info.CurrentMP += recover;
    }

    public void ClearAbnormal()
    {
        Info.ClearAbnormal();
    }

    public void SetBuff(int id, int lv)
    {
        Info.SetBuff(id, lv);
    }

    public void SetNoDamage()
    {
        Info.SetNoDamage();
    }

    public void SetOutline(bool show)
    {
        SpriteOutline.SetOutline(show);
    }

    public void StartAI(Action callback)
    {
        AI.StartAI(callback);
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        Info.IsActive = isActive;
    }

    public void CheckLiveState()
    {
        if (Info.CurrentHP <= 0)
        {
            if (LiveState == BattleCharacter.LiveStateEnum.Dying)
            {
                SetDying();
            }
            else
            {
                if (Info.HPQueue.Count > 0)
                {
                    //HPDequeue();
                    BattleUI.Instance.SetLittleHPBar(this, true);
                }
                else
                {
                    SetDeath();
                }
            }
        }
    }

    //public void HPDequeue()
    //{
    //    Info.HPDequeue();
    //    BattleUI.Instance.SetLittleHPBar(this, true);

    //    if (OnHPDequeueHandler != null)
    //    {
    //        OnHPDequeueHandler(Info.HPQueue.Count);
    //    }
    //}

    public void SetDeath()
    {
        Sprite.DOFade(0, 0.5f).OnComplete(() =>
        {
            BattleUI.Instance.SetLittleHPBar(this, false);
            BattleController.Instance.RemoveCharacer(this);

            if (OnDeathHandler != null)
            {
                OnDeathHandler();
            }
        });
    }

    public void SetDying()
    {
        GrayScale.SetScale(0);
        Animator.SetBool("IsDying", true);
        BattleController.Instance.RemoveCharacer(this);
    }

    private IEnumerator Move(Queue<Vector2Int> path, Action callback)
    {
        Vector2Int position = new Vector2Int();

        if (Animator != null)
        {
            Animator.SetBool("IsMoving", true);
        }

        while (path.Count > 0)
        {
            position = path.Dequeue();

            if (transform.position.x - position.x > 0 && _lookAt == Vector2Int.right)
            {
                Sprite.flipX = false;
                _lookAt = Vector2Int.left;
            }
            else if (transform.position.x - position.x < 0 && _lookAt == Vector2Int.left)
            {
                Sprite.flipX = true;
                _lookAt = Vector2Int.right;
            }
            transform.DOMove((Vector2)position, 0.2f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.2f);
        }

        if (Animator != null)
        {
            Animator.SetBool("IsMoving", false);
        }

        if (callback != null)
        {
            callback();
        }
    }
}
