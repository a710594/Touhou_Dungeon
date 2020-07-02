using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByTheTale.StateMachine;

public class BattleController : MachineBehaviour
{
    private static readonly int _maxPower = 100;

    public enum ResultType
    {
        Win,
        Lose,
        None,
    }

    public Action TurnEndHandler;

    public static BattleController Instance;

    public BattleCharacter SelectedCharacter;
    public List<BattleCharacter> CharacterList = new List<BattleCharacter>();

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

    private int _turn = 1;
    private int _exp;
    private BattleMemo _memo = new BattleMemo();
    private List<int> _dropItemList = new List<int>();
    private List<BattleCharacter> _actionQueue = new List<BattleCharacter>();
    private List<BattleCharacter> _playerList = new List<BattleCharacter>(); //戰鬥結束時需要的友方資料

    public void Init(int battlefieldId, BattleGroupData.RootObject battleGroupData, Action winCallback = null, Action loseCallback = null)
    {
        BattlefieldGenerator.Instance.Generate(battlefieldId, battleGroupData.EnemyList.Count);

        _turn = 1;
        _power =  TeamManager.Instance.Power;
        _exp = battleGroupData.Exp;
        _winCallback = winCallback;
        _loseCallback = loseCallback;
        CharacterList.Clear();

        BattleCharacter character;

        TeamMember member;
        for (int i = 0; i < TeamManager.Instance.MemberList.Count; i++)
        {
            member = TeamManager.Instance.MemberList[i];
            character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            character.Init(member);
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

        Camera.main.transform.position = new Vector3(BattleFieldManager.Instance.Center.x, BattleFieldManager.Instance.Center.y, Camera.main.transform.position.z);
        ChangeSceneUI.Instance.EndClock(() =>
        {
            BattleUI.Open();
            BattleUI.Instance.Init(_power, CharacterList);
            //BattleUI.Instance.SetPriorityQueueVisible(true);
            ChangeState<TurnStartState>();
        });
    }

    public void InitFromMemo(Action winCallback = null, Action loseCallback = null)
    {
        _winCallback = winCallback;
        _loseCallback = loseCallback;

        BattleMemo memo = Caretaker.Instance.Load<BattleMemo>();
        BattlefieldGenerator.Instance.Generate(memo.MapDic);
        BattleFieldManager.Instance.MapBound = memo.MapBound;
        BattleFieldManager.Instance.MapDic = new Dictionary<Vector2Int, BattleField>();
        foreach (KeyValuePair<string, BattleField> item in memo.MapDic)
        {
            BattleFieldManager.Instance.MapDic.Add(Utility.StringToVector2Int(item.Key), item.Value);
        }

        _turn = memo.Turn;
        _power = memo.Power;
        _exp = memo.Exp;
        _actionQueue = new List<BattleCharacter>(memo.QueueLength);
        _dropItemList = memo.DropItemList;
        CharacterList.Clear();

        BattleCharacter character;

        for (int i = 0; i < memo.CharacterList.Count; i++)
        {
            character = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacter", ResourceManager.Type.Other).GetComponent<BattleCharacter>();
            character.Init(memo.CharacterList[i]);
            CharacterList.Add(character);
            _playerList.Add(character);
            character.SetPosition(memo.CharacterList[i].Position);
            _actionQueue[memo.CharacterList[i].QueueIndex] = character;

            if (memo.CharacterList[i].IsSelected)
            {
                SelectedCharacter = character;
            }

            if (character.LiveState == BattleCharacter.LiveStateEnum.Dead)
            {
                character.Sprite.color = Color.clear;
            }
        }

        Camera.main.transform.position = new Vector3(SelectedCharacter.Sprite.transform.position.x, SelectedCharacter.Sprite.transform.position.y, Camera.main.transform.position.z);
        CameraController.Instance.SetParent(SelectedCharacter.Sprite.transform, true, () =>
        {
            SelectedCharacter.SetOutline(true);
            BattleUI.Open();
            BattleUI.Instance.Init(_power, CharacterList);
            //BattleUI.Instance.SetPriorityQueueVisible(true);
            ChangeState<SelectActionState>();
        });
    }

    public void Save() 
    {
        Caretaker.Instance.Save<BattleMemo>(_memo);
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
        SelectedCharacter.ActionDoneCompletely();
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

    private void SortCharacter(List<BattleCharacter> list) //按照敏捷和技能優先權來排序
    {
        list.Sort((x, y) =>
        {
            if (x.Info.AGI  == y.Info.AGI)
            {
                if (x.Camp == BattleCharacter.CampEnum.Enemy && y.Camp == BattleCharacter.CampEnum.Partner)
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

    private ResultType CheckResult()
    {
        int partnerCount = 0;
        int enemyCount = 0;

        for (int i=0; i<CharacterList.Count; i++)
        {
            if (CharacterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
            {
                if (CharacterList[i].Camp == BattleCharacter.CampEnum.Partner)
                {
                    partnerCount++;
                }
                else if (CharacterList[i].Camp == BattleCharacter.CampEnum.Enemy)
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

    private void Write()
    {
        _memo.MapBound = BattleFieldManager.Instance.MapBound;
        _memo.MapDic = new Dictionary<string, BattleField>();
        foreach (KeyValuePair<Vector2Int, BattleField> item in BattleFieldManager.Instance.MapDic)
        {
            _memo.MapDic.Add(Utility.Vector2IntToString(item.Key), item.Value);
        }

        _memo.Turn = _turn;
        _memo.Power = Power;
        _memo.Exp = _exp;
        _memo.QueueLength = _actionQueue.Count;
        _memo.DropItemList = _dropItemList;

        _memo.CharacterList.Clear();
        for (int i = 0; i < CharacterList.Count; i++)
        {
            _memo.CharacterList.Add(new BattleCharacterMemo(CharacterList[i].Info));
            _memo.CharacterList[_memo.CharacterList.Count - 1].IsSelected = GameObject.ReferenceEquals(CharacterList[i], SelectedCharacter);
            for (int j = 0; j < _actionQueue.Count; j++)
            {
                if (GameObject.ReferenceEquals(CharacterList[i], _actionQueue[j]))
                {
                    _memo.CharacterList[_memo.CharacterList.Count - 1].QueueIndex = j;
                }
            }
        }
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

            BattleUI.Instance.SetTurnLabel(parent._turn);

            parent.SortCharacter(parent.CharacterList);

            parent._actionQueue.Clear();
            for (int i = 0; i < parent.CharacterList.Count; i++)
            {
                if (parent.CharacterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
                {
                    parent._actionQueue.Add(parent.CharacterList[i]);
                    parent.CharacterList[i].InitActionCount();
                    //parent.CharacterList[i].InitOrignalPosition();
                }
            }
            parent.ChangeState<SelectCharacterState>();
        }
    }

    private class SelectCharacterState : BattleState //依照角色敏捷值來決定行動的先後順序
    {
        public override void Enter()
        {
            base.Enter();

            if (parent._actionQueue.Count > 0)
            {
                if (parent._actionQueue.Count == parent.CharacterList.Count)
                {
                    //BattleUI.Instance.SetPriorityQueueData(new List<BattleCharacter>(parent._actionQueue));
                }
                else
                {
                    //BattleUI.Instance.ScrollPriorityQueue(new List<BattleCharacter>(parent._actionQueue));
                }

                BattleCharacter character = parent._actionQueue[0];
                parent._actionQueue.RemoveAt(0);
                if (character.LiveState == BattleCharacter.LiveStateEnum.Alive)
                {
                    CameraController.Instance.SetParent(character.Sprite.transform, true, () =>
                     {
                         BattleCharacter.NotActReason reason;
                         if (character.CanAct(out reason))
                         {
                             if (parent.SelectedCharacter != null)
                             {
                                 parent.SelectedCharacter.SetOutline(false);
                             }

                             parent.SelectedCharacter = character;
                             parent.SelectedCharacter.SetOutline(true);
                             BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);

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
                             string text = "";
                             FloatingNumber.Type type = FloatingNumber.Type.Other;
                             if (reason == BattleCharacter.NotActReason.Paralysis)
                             {
                                 text = "麻痺";
                                 type = FloatingNumber.Type.Paralysis;
                             }
                             else if (reason == BattleCharacter.NotActReason.Sleeping)
                             {
                                 text = "睡眠";
                                 type = FloatingNumber.Type.Sleeping;
                             }

                             BattleUI.Instance.SetStatus(character, text, type, () =>
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
            parent.Write();
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null)
            {
                BattleUI.Instance.SetInfo(true, character);

                character.GetDetectRange();
                character.ShowDetectRange();
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
            }
        }

        public override void Exit()
        {
            base.Exit();

            BattleUI.Instance.SetActionGroupVisible(false);
            BattleUI.Instance.SetBattleFieldVisible(false);
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
                character.GetDetectRange();
                character.ShowDetectRange();
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
            }
        }

        public override void Exit()
        {
            BattleUI.Instance.SetMoveConfirmVisible(false);
            BattleUI.Instance.SetReturnActionVisible(false);
            BattleUI.Instance.SetBattleFieldVisible(false);
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

                character.GetDetectRange();
                character.ShowDetectRange();
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
            }
        }

        public override void Exit()
        {
            TilePainter.Instance.Clear(3);
            BattleUI.Instance.SetReturnActionVisible(false);
            BattleUI.Instance.SetSkillScrollViewVisible(false);
            BattleUI.Instance.SetBattleFieldVisible(false);
        }
    }

    private class ConfirmState : BattleState //玩家確定是否施放技能
    {
        public override void Enter()
        {
            base.Enter();

            parent.SelectedCharacter.GetSkillRange();
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

            BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);
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
                parent.ChangeState<SelectCharacterState>();
            }
            else
            {
                parent.SelectedCharacter.UseSkill(() =>
                {
                    TilePainter.Instance.Clear(2);

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
                });
            }
        }
    }

    private class TurnEndState : BattleState //回合結束,做異常狀態傷害等判定
    {
        private Queue<BattleCharacter> _poisonQueue = new Queue<BattleCharacter>();

        public override void Enter()
        {
            base.Enter();

            for (int i = 0; i < parent.CharacterList.Count; i++)
            {
                if (parent.CharacterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive && parent.CharacterList[i].Info.IsPoisoning)
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
                    character.SetPoisonDamage(SetPoison);
                });
            }
            else
            {
                parent._turn++;

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
            List<int> orignalLvList = TeamManager.Instance.GetLvList();
            List<int> orignalExpList = TeamManager.Instance.GetExpList();
            TeamManager.Instance.AddExp(parent._exp);
            BattleUI.Instance.SetResult(true, orignalLvList, orignalExpList, parent._dropItemList, parent._winCallback, parent._loseCallback);
        }

        public override void ScreenOnClick(Vector2Int position) { }

        public override void Exit() { }
    }

    private class LoseState : BattleState
    {
        public override void Enter()
        {
            base.Enter();

            Debug.Log("Lose");
            BattleUI.Instance.SetResult(false);
        }

        public override void ScreenOnClick(Vector2Int position) { }

        public override void Exit() { }
    }

    private class EscapeState : BattleState 
    {
        public override void Enter()
        {
            base.Enter();

            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
            {
                ExploreController.Instance.SetFloorFromMemo();
            });
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ChangeState<WinState>();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ChangeState<LoseState>();
        }
    }
}
