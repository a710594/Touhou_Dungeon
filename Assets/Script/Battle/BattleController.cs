using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByTheTale.StateMachine;

public class BattleController : MachineBehaviour
{
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

    private int _turn = 1;
    private int _power = 0;
    private List<int> _enemyList = new List<int>();
    private List<BattleCharacter> _actionQueue = new List<BattleCharacter>();
    private List<BattleCharacterPlayer> _playerList = new List<BattleCharacterPlayer>(); //戰鬥結束時需要的友方資料

    public void Init(int battlefieldId, List<KeyValuePair<int, int>> enemyList)
    {
        BattlefieldGenerator.Instance.Generate(battlefieldId);

        _turn = 1;
        _power = 0;
        CharacterList.Clear();

        TeamMember member;
        BattleCharacterPlayer characterPlayer;
        for (int i = 0; i < TeamManager.Instance.MemberList.Count; i++)
        {
            characterPlayer = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacterPlayer", ResourceManager.Type.Other).GetComponent<BattleCharacterPlayer>();
            member = TeamManager.Instance.MemberList[i];
            characterPlayer.Init(member);
            CharacterList.Add(characterPlayer);
            _playerList.Add(characterPlayer);
            characterPlayer.transform.position = (Vector2)(TeamManager.Instance.MemberPositionDic[member] + BattleFieldManager.Instance.Center);
        }

        _enemyList.Clear();
        BattleCharacterAI characterAI;
        for (int i = 0; i < enemyList.Count; i++)
        {
            _enemyList.Add(enemyList[i].Key);
            characterAI = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacterAI", ResourceManager.Type.Other).GetComponent<BattleCharacterAI>();
            characterAI.Init(enemyList[i].Key, enemyList[i].Value);
            CharacterList.Add(characterAI);
            //AICharacterList.Add(characterAI);
            characterAI.transform.position = BattleFieldManager.Instance.GetRandomPosition(false);
        }

        Camera.main.transform.position = new Vector3(BattleFieldManager.Instance.Center.x, BattleFieldManager.Instance.Center.y, Camera.main.transform.position.z);
        //ChangeSceneUI.Instance.EndClock(() =>
        //{
        BattleUI.Open();
        BattleUI.Instance.Init(CharacterList);
        //BattleUI.Instance.SetPriorityQueueVisible(true);
        ChangeState<TurnStartState>();
        //});
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
        SelectedCharacter.ActionDone();
        SelectedCharacter.InitOrignalPosition();
        if (SelectedCharacter.ActionCount > 0)
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
        ((BattleCharacterPlayer)SelectedCharacter).ReturnToOriginalPosition();
        TilePainter.Instance.Clear(2);
        TilePainter.Instance.Clear(3);
        ChangeState<SelectActionState>();
    }

    public void SelectSkill(Skill skill)
    {
        BattleCharacterPlayer character = (BattleCharacterPlayer)SelectedCharacter;
        character.SelectSkill(skill);
        //ChangeState<SelectTargetState>();
    }

    public void SetIdle() //角色待機
    {
        SelectedCharacter.ActionDoneCompletely();
        ChangeState<SelectCharacterState>();
    }

    public void AddPower(int value) 
    {
        _power += value;
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
            if (x.AGI  == y.AGI)
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
                return (y.AGI).CompareTo(x.AGI);
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
                    parent.CharacterList[i].InitOrignalPosition();
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
                             if (parent.SelectedCharacter != null && parent.SelectedCharacter.LiveState == BattleCharacter.LiveStateEnum.Alive)
                             {
                                 parent.SelectedCharacter.SetOutline(false);
                             }

                             parent.SelectedCharacter = character;
                             parent.SelectedCharacter.SetOutline(true);
                             BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);

                             if (parent.SelectedCharacter is BattleCharacterPlayer)
                             {
                                 parent.ChangeState<SelectActionState>();
                             }
                             else if (parent.SelectedCharacter is BattleCharacterAI)
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
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null)
            {
                BattleUI.Instance.SetInfo(true, character);

                if (character is BattleCharacterAI)
                {
                    BattleCharacterAI characterAI = (BattleCharacterAI)character;
                    characterAI.GetDetectRange();
                    characterAI.ShowDetectRange();

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

                if (character is BattleCharacterAI)
                {
                    BattleCharacterAI characterAI = (BattleCharacterAI)character;
                    characterAI.GetDetectRange();
                    characterAI.ShowDetectRange();

                }
            }
            else
            {
                BattleUI.Instance.SetInfo(false);

                if (parent.SelectedCharacter.InMoveRange(position))
                {
                    ((BattleCharacterPlayer)parent.SelectedCharacter).ReturnToOriginalPosition();
                    ((BattleCharacterPlayer)parent.SelectedCharacter).GetPath(position);
                    ((BattleCharacterPlayer)parent.SelectedCharacter).Move();
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
            BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null)
            {
                BattleUI.Instance.SetInfo(true, character);

                if (character is BattleCharacterAI)
                {
                    BattleCharacterAI characterAI = (BattleCharacterAI)character;
                    characterAI.GetDetectRange();
                    characterAI.ShowDetectRange();

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
            else
            {
                TilePainter.Instance.Clear(2);
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
            ((BattleCharacterAI)parent.SelectedCharacter).StartAI(() =>
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
            if (parent.SelectedCharacter is BattleCharacterAI && ((BattleCharacterAI)parent.SelectedCharacter).Target == null)
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
                        parent.SelectedCharacter.ActionDone();
                        if (parent.SelectedCharacter.ActionCount > 0)
                        {
                            if (parent.SelectedCharacter is BattleCharacterPlayer)
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
                if (parent.CharacterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive && parent.CharacterList[i].IsPoisoning)
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

            int getExp = 0;
            EnemyData.RootObject data;
            List<int> itemList = new List<int>(); //單隻怪的掉落物
            List<int> dropItemList = new List<int>(); //全部的掉落物
            for (int i=0; i<parent._enemyList.Count; i++)
            {
                data = EnemyData.GetData(parent._enemyList[i]);
                itemList = data.GetDropItemList();
                for (int j = 0; j < itemList.Count; j++)
                {
                    dropItemList.Add(itemList[j]);
                }
                getExp += data.Exp;
            }


            ItemManager.Instance.AddItem(dropItemList, ItemManager.Type.Bag);
            TeamManager.Instance.Refresh(parent._playerList);
            List<int> orignalLvList = TeamManager.Instance.GetLvList();
            List<int> orignalExpList = TeamManager.Instance.GetExpList();
            TeamManager.Instance.AddExp(getExp);
            BattleUI.Instance.SetResult(true, orignalLvList, orignalExpList, dropItemList);
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

    void Awake()
    {
        Instance = this;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState<WinState>();
        }
    }
}
