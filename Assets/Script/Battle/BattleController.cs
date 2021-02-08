using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByTheTale.StateMachine;

public class BattleController : MachineBehaviour
{
    private static readonly int _maxPower = 50;

    public enum ResultType
    {
        Win,
        Lose,
        None,
    }

    public Action InitHandler;
    public Action TurnStartHandler;
    public Action TurnEndHandler;
    public Action<Action> ShowEndHandler;
    public Action SelectActionStartHandler;

    public static BattleController Instance;

    [HideInInspector]
    public int Turn = 1;
    [HideInInspector]
    public BattleCharacter SelectedCharacter;
    [HideInInspector]
    public List<BattleCharacter> CharacterList = new List<BattleCharacter>();
    public List<Skill> ItemSkillList = new List<Skill>(); //回復藥等道具技能

    //特殊場景
    public int BattlefieldId;
    public int Exp;
    public BattleCharacter[] Enemys;

    public int Power
    {
        get
        {
            return _power;
        }
    }
    private int _power = 0;

    private Action _winCallback;
    private Action _loseCallback;

    private int _exp;
    private List<int> _dropItemList = new List<int>();
    private List<BattleCharacter> _actionQueue = new List<BattleCharacter>();
    private List<BattleCharacter> _playerList = new List<BattleCharacter>(); //戰鬥結束時需要的友方資料

    public void Init(int battlefieldId, BattleGroupData.RootObject battleGroupData, Action winCallback, Action loseCallback)
    {
        BattlefieldGenerator.Instance.Generate(battlefieldId, battleGroupData.EnemyList.Count);

        Turn = 1;
        _power =  TeamManager.Instance.Power;
        _exp = battleGroupData.Exp;
        _winCallback = winCallback;
        _loseCallback = loseCallback;
        CharacterList.Clear();

        BattleCharacter character;

        TeamMember member;
        List<TeamMember> memberList = TeamManager.Instance.GetAttendList();
        for (int i = 0; i < memberList.Count; i++)
        {
            member = memberList[i];
            character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            character.name = member.Data.Animator;
            character.Init(member, TeamManager.Instance.Lv);
            CharacterList.Add(character);
            _playerList.Add(character);
            character.SetPosition((Vector2)(member.Formation + BattleFieldManager.Instance.Center));

            if (character.LiveState == BattleCharacter.LiveStateEnum.Dead)
            {
                character.Sprite.color = Color.clear;
            }
        }

        EnemyData.RootObject data;
        List<int> itemList = new List<int>(); //單隻怪的掉落物
        List<KeyValuePair<int, int>> enemyList = battleGroupData.GetEnemy();
        for (int i = 0; i < enemyList.Count; i++)
        {
            data = EnemyData.GetData(enemyList[i].Key);
            itemList = data.GetDropItemList();
            for (int j = 0; j < itemList.Count; j++)
            {
                _dropItemList.Add(itemList[j]);
            }
            character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            character.Init(enemyList[i].Key, enemyList[i].Value);
            CharacterList.Add(character);
            character.SetPosition(BattleFieldManager.Instance.EnemyPositionList[i]);
        }

        int itemId;
        ItemData.RootObject itemData;
        SkillData.RootObject skillData;
        Skill skill;
        List<Item> medicineList = ItemManager.Instance.GetItemListByType(ItemManager.Type.Bag, ItemData.TypeEnum.Medicine);

        if (medicineList != null)
        {
            foreach (Item item in medicineList)
            {
                itemId = item.ID;
                itemData = ItemData.GetData(itemId);
                skillData = SkillData.GetData(itemData.Skill);
                skill = SkillFactory.GetNewSkill(skillData, null, 1);
                skill.ItemID = itemId;
                ItemSkillList.Add(skill);
            }
        }

        Camera.main.transform.position = new Vector3(BattleFieldManager.Instance.Center.x, BattleFieldManager.Instance.Center.y, Camera.main.transform.position.z);
        AudioSystem.Instance.Play("Battle", true);
        ChangeSceneUI.Instance.EndClock(() =>
        {
            BattleUI.Open();
            BattleUI.Instance.Init(_power, CharacterList, battleGroupData.CanEscape);
            ChangeState<TurnStartState>();
        });
    }
    public void SpecialInit(Action winCallback, Action loseCallback) //特殊場景的 Init
    {
        BattlefieldGenerator.Instance.GenerateFromTilemap(BattlefieldId);

        Turn = 1;
        _power = TeamManager.Instance.Power;
        _exp = Exp;
        _winCallback = winCallback;
        _loseCallback = loseCallback;
        CharacterList.Clear();

        BattleCharacter character;

        TeamMember member;
        List<TeamMember> memberList = TeamManager.Instance.GetAttendList();
        for (int i = 0; i < memberList.Count; i++)
        {
            member = memberList[i];
            character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            character.name = member.Data.Animator;
            character.Init(member, TeamManager.Instance.Lv);
            CharacterList.Add(character);
            _playerList.Add(character);
            character.SetPosition((Vector2)(member.Formation + BattleFieldManager.Instance.Center));

            if (character.LiveState == BattleCharacter.LiveStateEnum.Dead)
            {
                character.Sprite.color = Color.clear;
            }
        }

        EnemyData.RootObject data;
        List<int> itemList = new List<int>(); //單隻怪的掉落物
        for (int i = 0; i < Enemys.Length; i++)
        {
            character = Enemys[i];
            character.InitByInspector();
            CharacterList.Add(character);

            data = character.Info.EnemyData;
            itemList = data.GetDropItemList();
            for (int j = 0; j < itemList.Count; j++)
            {
                _dropItemList.Add(itemList[j]);
            }
        }

        int itemId;
        ItemData.RootObject itemData;
        SkillData.RootObject skillData;
        Skill skill;
        List<Item> medicineList = ItemManager.Instance.GetItemListByType(ItemManager.Type.Bag, ItemData.TypeEnum.Medicine);

        if (medicineList != null)
        {
            foreach (Item item in medicineList)
            {
                itemId = item.ID;
                itemData = ItemData.GetData(itemId);
                skillData = SkillData.GetData(itemData.Skill);
                skill = SkillFactory.GetNewSkill(skillData, null, 1);
                skill.ItemID = itemId;
                ItemSkillList.Add(skill);
            }
        }

        Camera.main.transform.position = new Vector3(BattleFieldManager.Instance.Center.x, BattleFieldManager.Instance.Center.y, Camera.main.transform.position.z);
        AudioSystem.Instance.Play("Battle", true);
        ChangeSceneUI.Instance.EndClock(() =>
        {
            BattleUI.Open();
            BattleUI.Instance.Init(_power, CharacterList, false);

            if (InitHandler != null)
            {
                InitHandler();
            }

            ChangeState<TurnStartState>();
        });
    }

    public override void AddStates()
    {
        AddState<BattleState>(); //空白的state,初始化用
        AddState<TurnStartState>();
        AddState<SelectCharacterState>();
        AddState<SelectActionState>();
        AddState<MoveState>();
        AddState<SelectSkillState>();
        AddState<ConfirmState>();
        AddState<AIState>();
        AddState<ShowState>();
        AddState<TurnEndState>();
        AddState<WinState>();
        AddState<LoseState>();
        AddState<EscapeState>();

        SetInitialState<BattleState>();
    }

    public BattleCharacter GetCharacterByPosition(Vector2 position) //取得該格子上的角色
    {
        for (int i = 0; i < CharacterList.Count; i++)
        {
            if (CharacterList[i].LiveState != BattleCharacter.LiveStateEnum.Dead && (Vector2)CharacterList[i].transform.position == position)
            {
                return CharacterList[i];
            }
        }
        return null;
    }

    public void ScreenOnClick(Vector2Int position)
    {
        ((BattleState)currentState).ScreenOnClick(position);
    }

    public void ChangeToMoveState()
    {
        ChangeState<MoveState>();
    }

    public void ChangeToSelectSkillState()
    {
        ChangeState<SelectSkillState>();
    }

    public void ChangeToEscapeState()
    {
        ChangeState<EscapeState>();
    }

    public void MoveConfirm()
    {
        TilePainter.Instance.Clear(2);
        TilePainter.Instance.Clear(3);
        SelectedCharacter.MoveDone();
        //SelectedCharacter.InitOrignalPosition();
        if (SelectedCharacter.Info.ActionCount > 0)
        {
            ChangeState<SelectActionState>();
        }
        else
        {
            ChangeState<SelectCharacterState>();
        }
    }

    public void MoveCancel()
    {
        SelectedCharacter.ReturnToOriginalPosition();
        TilePainter.Instance.Clear(2);
        TilePainter.Instance.Clear(3);
        ChangeState<SelectActionState>();
    }

    public void SelectSkill(Skill skill)
    {
        SelectedCharacter.SelectSkill(skill);
    }

    public void SetIdle() //角色待機
    {
        ChangeState<SelectCharacterState>();
    }

    public void MoveUndo() //返回上一步的位置
    {
        SelectedCharacter.MoveUndo();
    }

    public void AddPower(int value) 
    {
        _power += value;
        if (_power > _maxPower)
        {
            _power = _maxPower;
        }
        BattleUI.Instance.SetPower(_power);
    }

    public void MinusPower(int value) 
    {
        _power -= value;
        BattleUI.Instance.SetPower(_power);
    }

    public void AddCharacer(BattleCharacter character, bool needSort)
    {
        character.InitActionCount();
        CharacterList.Add(character);
        BattleUI.Instance.InitCharacter(character);

        _actionQueue.Insert(0, character);
        if (needSort)
        {
            SortCharacter();
        }
        BattleUI.Instance.InitPriorityQueue(new List<BattleCharacter>(_actionQueue));
    }


    public void SetCharacerActive(BattleCharacter character)
    {
        character.InitActionCount();
        character.SetActive(true);

        _actionQueue.Add(character);
        BattleUI.Instance.InitPriorityQueue(new List<BattleCharacter>(_actionQueue));
    }

    public void RemoveCharacer(BattleCharacter character)
    {
        _actionQueue.Remove(character);
        BattleUI.Instance.ScrollPriorityQueue(character);
    }

    public void GiveUp()
    {
        ChangeState<LoseState>();
    }

    private ResultType CheckResult()
    {
        int partnerCount = 0;
        int enemyCount = 0;

        for (int i=0; i<CharacterList.Count; i++)
        {
            if (CharacterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
            {
                if (CharacterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Partner)
                {
                    partnerCount++;
                }
                else if (CharacterList[i].Info.Camp == BattleCharacterInfo.CampEnum.Enemy)
                {
                    enemyCount++;
                }
            }
        }

        if (enemyCount == 0)
        {
            return ResultType.Win;
        }
        else if (partnerCount == 0)
        {
            return ResultType.Lose;
        }
        else
        {
            return ResultType.None;
        }
    }

    private void SortCharacter()
    {
        _actionQueue.Sort((x, y) =>
        {
            if (x.Info.AGI == y.Info.AGI) //比較角色的敏捷 * 技能優先值
            {
                if (x.Info.Camp == BattleCharacterInfo.CampEnum.Enemy && y.Info.Camp == BattleCharacterInfo.CampEnum.Partner) //若相同,則玩家優先
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return (y.Info.AGI).CompareTo(x.Info.AGI);
            }
        });
    }

    //
    //State
    //
    private class BattleState : State
    {
        protected BattleController parent;

        public override void Enter()
        {
            parent = (BattleController)machine;
        }

        public virtual void ScreenOnClick(Vector2Int position) { }
    }

    private class TurnStartState : BattleState
    {
        public override void Enter()
        {
            base.Enter();

            parent.SelectedCharacter = null;
            parent._actionQueue.Clear();
            for (int i = 0; i < parent.CharacterList.Count; i++)
            {
                if (parent.CharacterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive && parent.CharacterList[i].IsActive)
                {
                    parent.CharacterList[i].InitActionCount();
                    parent.CharacterList[i].InitGetDamage();
                    parent._actionQueue.Add(parent.CharacterList[i]);
                }
            }

            parent.SortCharacter();

            BattleUI.Instance.SetTurnLabel(parent.Turn, ()=> 
            {
                BattleUI.Instance.InitPriorityQueue(new List<BattleCharacter>(parent._actionQueue));

                if (parent.TurnStartHandler != null)
                {
                    parent.TurnStartHandler();
                }

                parent.ChangeState<SelectCharacterState>();
            });
        }
    }

    private class SelectCharacterState : BattleState //依照角色敏捷值來決定行動的先後順序
    {
        private Timer _timer = new Timer();

        public override void Enter()
        {
            base.Enter();

            if (parent._actionQueue.Count > 0)
            {
                BattleUI.Instance.ScrollPriorityQueue(parent.SelectedCharacter);

                BattleCharacter character = parent._actionQueue[0];
                parent._actionQueue.RemoveAt(0);
                if (character.LiveState == BattleCharacter.LiveStateEnum.Alive && character.Info.ActionCount > 0)
                {
                    CameraController.Instance.SetParent(character.Sprite.transform, true, () =>
                    {
                         if (parent.SelectedCharacter != null)
                         {
                             parent.SelectedCharacter.SetOutline(false);
                         }

                         parent.SelectedCharacter = character;
                         parent.SelectedCharacter.SetOutline(true);
                         //BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);

                        BattleStatus status;
                        if (character.CanAct(out status))
                        {
                            if (!parent.SelectedCharacter.Info.IsAI)
                            {
                                parent.ChangeState<SelectActionState>();
                            }
                            else
                            {
                                parent.ChangeState<AIState>();
                            }
                            return;
                        }
                        else
                        {
                            BattleUI.Instance.SetFloatingNumber(character, status.Message, FloatingNumber.Type.Other);
                            _timer.Start(0.5f, ()=> 
                            {
                                parent.ChangeState<SelectCharacterState>();
                            });
                        }
                    });
                }
                else
                {
                    parent.ChangeState<SelectCharacterState>();
                }
            }
            else
            {
                parent.ChangeState<TurnEndState>();
            }
        }
    }

    private class SelectActionState : BattleState //玩家選擇行動
    {
        public override void Enter()
        {
            base.Enter();

            BattleUI.Instance.SetActionGroupVisible(true);
            BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);

            if (parent.SelectActionStartHandler != null)
            {
                parent.SelectActionStartHandler();
            }
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null)
            {
                BattleUI.Instance.SetInfo(true, character);

                if (character.Info.Camp == BattleCharacterInfo.CampEnum.Enemy)
                {
                    character.GetDetectRange();
                    character.ShowDetectRange();
                }
            }
            else
            {
                TilePainter.Instance.Clear(3);
                BattleUI.Instance.SetInfo(false);
            }

            BattleField battleField = BattleFieldManager.Instance.GetField(position);
            if (battleField != null)
            {
                BattleUI.Instance.SetBattleFieldVisible(true);
                BattleUI.Instance.SetBattleFieldData(battleField);
                TilePainter.Instance.Clear(4);
                TilePainter.Instance.Painting("Select", 4, position);
            }
        }

        public override void Exit()
        {
            base.Exit();

            BattleUI.Instance.SetActionGroupVisible(false);
            BattleUI.Instance.SetBattleFieldVisible(false);
            TilePainter.Instance.Clear(4);
        }
    }

    private class MoveState : BattleState //玩家移動角色
    {
        public override void Enter()
        {
            base.Enter();

            parent.SelectedCharacter.GetMoveRange();
            parent.SelectedCharacter.ShowMoveRange();

            BattleUI.Instance.SetMoveConfirmVisible(true);
            BattleUI.Instance.SetReturnActionVisible(true);
            BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null)
            {
                BattleUI.Instance.SetInfo(true, character);
                if (character.Info.Camp == BattleCharacterInfo.CampEnum.Enemy)
                {
                    character.GetDetectRange();
                    character.ShowDetectRange();
                }
            }
            else
            {
                BattleUI.Instance.SetInfo(false);

                if (parent.SelectedCharacter.InMoveRange(position))
                {
                    parent.SelectedCharacter.ReturnToOriginalPosition();
                    Queue<Vector2Int> path = parent.SelectedCharacter.GetPath(position);
                    parent.SelectedCharacter.StartMove(path, null);
                }
                else
                {
                    TilePainter.Instance.Clear(3);
                }
            }

            BattleField battleField = BattleFieldManager.Instance.GetField(position);
            if (battleField != null)
            {
                BattleUI.Instance.SetBattleFieldVisible(true);
                BattleUI.Instance.SetBattleFieldData(battleField);
                TilePainter.Instance.Clear(4);
                TilePainter.Instance.Painting("Select", 4, position);
            }
        }

        public override void Exit()
        {
            BattleUI.Instance.SetMoveConfirmVisible(false);
            BattleUI.Instance.SetReturnActionVisible(false);
            BattleUI.Instance.SetBattleFieldVisible(false);
            TilePainter.Instance.Clear(4);
        }
    }

    private class SelectSkillState : BattleState //玩家選擇技能
    {
        public override void Enter()
        {
            base.Enter();

            parent.SelectedCharacter.SelectedSkill = null;
            BattleUI.Instance.SetInfo(false);
            BattleUI.Instance.SetReturnActionVisible(true);
            BattleUI.Instance.SetSkillScrollViewVisible(true);
            BattleUI.Instance.RemoveSelectedSkill();
            BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null)
            {
                BattleUI.Instance.SetInfo(true, character);
                if (character.Info.Camp == BattleCharacterInfo.CampEnum.Enemy)
                {
                    character.GetDetectRange();
                    character.ShowDetectRange();
                }
            }
            else
            {
                BattleUI.Instance.SetInfo(false);
            }

            if (parent.SelectedCharacter.IsInSkillDistance(position))
            {
                parent.SelectedCharacter.SetTarget(position);
                parent.ChangeState<ConfirmState>();
            }

            BattleField battleField = BattleFieldManager.Instance.GetField(position);
            if (battleField != null)
            {
                BattleUI.Instance.SetBattleFieldVisible(true);
                BattleUI.Instance.SetBattleFieldData(battleField);
                TilePainter.Instance.Clear(4);
                TilePainter.Instance.Painting("Select", 4, position);
            }
        }

        public override void Exit()
        {
            TilePainter.Instance.Clear(3);
            BattleUI.Instance.SetReturnActionVisible(false);
            BattleUI.Instance.SetSkillScrollViewVisible(false);
            BattleUI.Instance.SetBattleFieldVisible(false);
            TilePainter.Instance.Clear(4);
        }
    }

    private class ConfirmState : BattleState //玩家確定是否施放技能
    {
        private List<BattleCharacter> _targetList = new List<BattleCharacter>();

        public override void Enter()
        {
            base.Enter();

            Vector2Int target;
            List<Vector2Int> rangeList;
            parent.SelectedCharacter.GetSkillRange(out target, out rangeList);
            for (int i=0; i<rangeList.Count; i++)
            {
                if (rangeList[i] == target)
                {
                    TilePainter.Instance.Painting("FrontSight", 4, rangeList[i]);
                }
                else
                {
                    TilePainter.Instance.Painting("YellowGrid", 2, rangeList[i]);
                }
            }

            _targetList.Clear();
            if (parent.SelectedCharacter.SelectedSkill is AttackSkill)
            {
                AttackSkill attackSkill = (AttackSkill)parent.SelectedCharacter.SelectedSkill;
                _targetList = attackSkill.GetTargetList();
                int damage;
                for (int i=0; i< _targetList.Count; i++)
                {
                    damage = attackSkill.CalculateDamage(parent.SelectedCharacter.Info, _targetList[i].Info, false, false);
                    BattleUI.Instance.SetPredictionHP(_targetList[i], damage);
                }
            }
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            if (parent.SelectedCharacter.IsTarget(position))
            {
                parent.ChangeState<ShowState>();
            }
            else
            {
                parent.ChangeState<SelectSkillState>();
            }

            for (int i = 0; i < _targetList.Count; i++)
            {
                BattleUI.Instance.StopPredictionHP(_targetList[i]);
            }
        }

        public override void Exit()
        {
            TilePainter.Instance.Clear(2);
            TilePainter.Instance.Clear(3);
            TilePainter.Instance.Clear(4);
        }
    }

    private class AIState : BattleState //AI行動
    {
        public override void Enter()
        {
            base.Enter();

            //BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);
            parent.SelectedCharacter.SetPosition(parent.SelectedCharacter.transform.position); //這一行是為了方便 debug,有時我會在 run time 的時候移動編輯器上的角色
            parent.SelectedCharacter.StartAI(() =>
            {
                parent.ChangeState<ShowState>();
            });



            //parent.ChangeState<SelectCharacterState>();
        }

        public override void ScreenOnClick(Vector2Int position)
        {
        }

        public override void Exit()
        {
            TilePainter.Instance.Clear(2);
            TilePainter.Instance.Clear(3);
            TilePainter.Instance.Clear(4);
        }
    }

    private class ShowState : BattleState //戰鬥演出
    {
        public override void Enter()
        {
            base.Enter();

            BattleUI.Instance.SetInfo(false);
            if (parent.SelectedCharacter.Info.IsAI && !parent.SelectedCharacter.CanHitTarget)
            {
                if (parent.ShowEndHandler != null)
                {
                    parent.ShowEndHandler(() => { parent.ChangeState<SelectCharacterState>(); });
                }
                else
                {
                    parent.ChangeState<SelectCharacterState>();
                }
            }
            else
            {
                TilePainter.Instance.Clear(2);
                parent.SelectedCharacter.UseSkill(() =>
                {
                    if (parent.ShowEndHandler != null)
                    {
                        parent.ShowEndHandler(() => { SetNextState(); });
                    }
                    else
                    {
                        SetNextState();
                    }
                });
            }
        }

        private void SetNextState() 
        {
            ResultType result = parent.CheckResult();
            if (result == ResultType.None)
            {
                parent.SelectedCharacter.SkillDone();
                if (parent.SelectedCharacter.Info.ActionCount > 0)
                {
                    if (!parent.SelectedCharacter.Info.IsAI)
                    {
                        parent.ChangeState<SelectActionState>();
                    }
                    else
                    {
                        parent.ChangeState<AIState>();
                    }
                }
                else
                {
                    parent.ChangeState<SelectCharacterState>();
                }
            }
            else if (result == ResultType.Win)
            {
                parent.ChangeState<WinState>();
            }
            else if (result == ResultType.Lose)
            {
                parent.ChangeState<LoseState>();
            }
        }

        public override void Exit()
        {
        }
    }

    private class TurnEndState : BattleState //回合結束,做異常狀態傷害等判定
    {
        private Timer _timer = new Timer();
        private Queue<BattleCharacter> _poisonQueue = new Queue<BattleCharacter>();

        public override void Enter()
        {
            base.Enter();

            for (int i = 0; i < parent.CharacterList.Count; i++)
            {
                if (parent.CharacterList[i].LiveState != BattleCharacter.LiveStateEnum.Dead && parent.CharacterList[i].Info.IsPoisoning)
                {
                    _poisonQueue.Enqueue(parent.CharacterList[i]);
                }
            }

            SetPoison();
        }

        public override void ScreenOnClick(Vector2Int position) { }

        public override void Exit() { }

        private void SetPoison()
        {
            if (_poisonQueue.Count > 0)
            {
                BattleCharacter character = _poisonQueue.Dequeue();
                CameraController.Instance.SetParent(character.Sprite.transform, true, () =>
                {
                    character.SetPoisonDamage();
                    _timer.Start(0.5f, SetPoison);
                });
            }
            else
            {
                parent.Turn++;

                if (parent.TurnEndHandler != null)
                {
                    parent.TurnEndHandler();
                }

                ResultType result = parent.CheckResult();
                if (result == ResultType.None)
                {
                    parent.ChangeState<TurnStartState>();
                }
                else if (result == ResultType.Win)
                {
                    parent.ChangeState<WinState>();
                }
                else if (result == ResultType.Lose)
                {
                    parent.ChangeState<LoseState>();
                }
            }
        }
    }

    private class WinState : BattleState
    {
        public override void Enter()
        {
            base.Enter();

            ItemManager.Instance.AddItem(parent._dropItemList, ItemManager.Type.Bag);
            TeamManager.Instance.Refresh(parent._power, parent._playerList);
            int orignalLv = TeamManager.Instance.Lv;
            int orignalExp = TeamManager.Instance.Exp;
            TeamManager.Instance.AddExp(parent._exp);
            BattleUI.Instance.SetResult(true, parent._winCallback, orignalLv, orignalExp, parent._dropItemList);
        }

        public override void ScreenOnClick(Vector2Int position) { }

        public override void Exit() { }
    }

    private class LoseState : BattleState
    {
        public override void Enter()
        {
            base.Enter();

            BattleUI.Instance.SetResult(false, parent._loseCallback);
        }

        public override void ScreenOnClick(Vector2Int position) { }

        public override void Exit() { }
    }

    private class EscapeState : BattleState 
    {
        public override void Enter()
        {
            base.Enter();

            bool success = true;
            int random1;
            int random2;
            BattleCharacter character;
            for (int i=0; i<parent.CharacterList.Count; i++)
            {
                character = parent.CharacterList[i];
                if (character.IsActive && character.LiveState == BattleCharacter.LiveStateEnum.Alive && character.Info.Camp == BattleCharacterInfo.CampEnum.Enemy)
                {
                    random1 = UnityEngine.Random.Range(1, parent.SelectedCharacter.Info.AGI + 1);
                    random2 = UnityEngine.Random.Range(1, character.Info.AGI + 1);
                    if (random1 < random2)
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (success)
            {
                BattleUI.Instance.TipLabel.SetLabel("逃跑成功", false, ()=> 
                {
                    AudioSystem.Instance.Stop(false);
                    MySceneManager.Instance.ChangeScene(MySceneManager.Instance.LastScene, () =>
                    {
                        ExploreController.Instance.SetFloorFromMemo();
                    });
                });
            }
            else
            {
                parent.SelectedCharacter.EscapeFail();
                BattleUI.Instance.TipLabel.SetLabel("逃跑失敗", false, ()=> 
                {
                    if (parent.SelectedCharacter.Info.ActionCount > 0)
                    {
                        parent.ChangeState<SelectActionState>();
                    }
                    else
                    {
                        parent.ChangeState<SelectCharacterState>();
                    }
                });
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public override void Update()
    {
        base.Update();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ChangeState<WinState>();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ChangeState<LoseState>();
        }
#endif
    }
}
