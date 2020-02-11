using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByTheTale.StateMachine;

public class BattleController : MachineBehaviour
{
    public Action TurnEndHandler;

    public static BattleController Instance;

    public BattleCharacter SelectedCharacter;
    public List<BattleCharacter> CharacterList = new List<BattleCharacter>();

    private List<BattleCharacter> _actionQueue = new List<BattleCharacter>();

    public void Init(int battlefieldId, List<KeyValuePair<int, int>> enemyList)
    {
        BattlefieldGenerator.Instance.Generate(battlefieldId);

        CharacterList.Clear();

        TeamMember member;
        BattleCharacterPlayer characterPlayer;
        for (int i = 0; i < TeamManager.Instance.MemberList.Count; i++)
        {
            characterPlayer = ResourceManager.Instance.Spawn("BattleCharacter/BattleCharacterPlayer", ResourceManager.Type.Other).GetComponent<BattleCharacterPlayer>();
            member = TeamManager.Instance.MemberList[i];
            characterPlayer.Init(member);
            CharacterList.Add(characterPlayer);
            //PlayerCharacterList.Add(characterPlayer);
            characterPlayer.transform.position = (Vector2)(TeamManager.Instance.MemberPositionDic[member] + BattleFieldManager.Instance.Center);
        }

        BattleCharacterAI characterAI;
        for (int i = 0; i < enemyList.Count; i++)
        {
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
        //BattleUI.Instance.Init(CharacterList);
        //BattleUI.Instance.SetPriorityQueueVisible(true);
        ChangeState<TurnStartState>();
        //});
    }

    public override void AddStates()
    {
        AddState<TurnStartState>();
        AddState<SelectCharacterState>();
        AddState<SelectActionState>();
        AddState<MoveState>();
        AddState<AIState>();
        AddState<ShowState>();
        AddState<TurnEndState>();

        SetInitialState<TurnStartState>();
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

    public void MoveConfirm()
    {
        TilePainter.Instance.Clear(2);
        SelectedCharacter.ActionDone();
        if (SelectedCharacter.ActionCount > 0)
        {
            ChangeState<SelectActionState>();
        }
        else
        {
            ChangeState<SelectCharacterState>();
        }
    }

    public virtual void SelectSkill(Skill skill)
    {
        BattleCharacterPlayer character = (BattleCharacterPlayer)SelectedCharacter;

        character.SelectSkill(skill);
        ChangeState<SelectTargetState>();
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
        //public virtual void SelectSkill(Skill skill) { }
    }

    private class TurnStartState : BattleState
    {
        public override void Enter()
        {
            base.Enter();

            parent.SortCharacter(parent.CharacterList);

            parent._actionQueue.Clear();
            for (int i = 0; i < parent.CharacterList.Count; i++)
            {
                if (parent.CharacterList[i].LiveState == BattleCharacter.LiveStateEnum.Alive)
                {
                    parent._actionQueue.Add(parent.CharacterList[i]);
                    parent.CharacterList[i].InitActionCount();
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
                CameraController.Instance.SetParent(character.Sprite.transform, true,()=> 
                {

                    BattleCharacter.NotActReason reason;
                    if (character.CanAct(out reason))
                    {
                        parent.SelectedCharacter = character;
                        //BattleUI.Instance.SetInfo(true, parent.SelectedCharacter);

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

                        //BattleUI.Instance.SetStatus(character, text, type, () =>
                        //{
                        parent.ChangeState<SelectCharacterState>();
                        //});
                        character.SelectedSkill = null;
                    }
                });
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
        }

        public override void Exit()
        {
            base.Exit();

            BattleUI.Instance.SetActionGroupVisible(false);
        }
    }

    private class MoveState : BattleState //玩家移動角色
    {
        public override void Enter()
        {
            base.Enter();

            parent.SelectedCharacter.InitOrignalPosition();
            parent.SelectedCharacter.GetMoveRange();
            parent.SelectedCharacter.ShowMoveRange();

            BattleUI.Instance.SetMoveConfirmVisible(true);
            BattleUI.Instance.SetReturnActionVisible(true);
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null && character != parent.SelectedCharacter)
            {
                //BattleUI.Instance.SetSubInfo(true, character);
            }
            else
            {
                //BattleUI.Instance.SetSubInfo(false);

                if (parent.SelectedCharacter.InMoveRange(position))
                {
                    ((BattleCharacterPlayer)parent.SelectedCharacter).ReturnToOriginalPosition();
                    ((BattleCharacterPlayer)parent.SelectedCharacter).GetPath(position);
                    ((BattleCharacterPlayer)parent.SelectedCharacter).Move();
                }
            }
        }

        public override void Exit()
        {
            BattleUI.Instance.SetMoveConfirmVisible(false);
            BattleUI.Instance.SetReturnActionVisible(false);
        }
    }

    private class SelectSkillState : BattleState //玩家選擇技能
    {
        public override void Enter()
        {
            base.Enter();

            BattleUI.Instance.SetInfo(false);
            BattleUI.Instance.SetSkillData(parent.SelectedCharacter);
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null)
            {
                BattleUI.Instance.SetInfo(true, character);
            }
        }

        public override void Exit() { }
    }

    private class SelectTargetState : BattleState //玩家選擇技能的施放目標
    {
        public override void Enter()
        {
            base.Enter();

            parent.SelectedCharacter.GetSkillDistance();
        }

        public override void ScreenOnClick(Vector2Int position)
        {
            if (parent.SelectedCharacter.IsInSkillDistance(position))
            {
                parent.SelectedCharacter.SetTarget(position);
                parent.ChangeState<ConfirmState>();
            }
            else
            {
                parent.ChangeState<MoveState>();
            }

            BattleCharacter character = parent.GetCharacterByPosition(position);
            if (character != null)
            {
                BattleUI.Instance.SetInfo(true, character);
            }
        }

        public override void Exit() { }
    }

    private class AIState : BattleState //AI行動
    {
        public override void Enter()
        {
            base.Enter();

            //CameraController.Instance.SetParent(parent.SelectedCharacter.Sprite.transform, true);
            ((BattleCharacterAI)parent.SelectedCharacter).StartAI(() =>
            {
                //parent.ChangeState<ShowState>();
                parent.ChangeState<SelectCharacterState>();
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
        }
    }

    private class ShowState : BattleState //戰鬥演出
    {
        public override void Enter()
        {
            base.Enter();
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
                if (parent.CharacterList[i].IsPoisoning)
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
                    character.SetPoison(SetPoison);
                });
            }
            else
            {
                if (parent.TurnEndHandler != null)
                {
                    parent.TurnEndHandler();
                }
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }
}
