using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreController
{
    private static ExploreController _instance;
    public static ExploreController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ExploreController();
            }
            return _instance;
        }
    }

    public int ArriveFloor = 1; //temp

    private MapInfo _mapInfo;
    private ExploreCharacter _player;
    private Vector2 _playerPosition;
    private List<Vector2Int> _exploredList = new List<Vector2Int>(); //走過的地圖範圍
    private List<Vector2Int> _exploredWallList = new List<Vector2Int>(); //已被發現的牆壁的範圍
    private List<Vector2Int> _guardList = new List<Vector2Int>(); //守衛型敵人的位置,遇到該敵人後會 remove
    private List<FieldEnemy> _fieldEnemyList = new List<FieldEnemy>();
    private Vector2Int[] _directions = new Vector2Int[4] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    public void GenerateFloor(int floor)
    {
        _exploredList.Clear();
        _exploredWallList.Clear();
        _guardList.Clear();

        DungeonBuilder.Instance.Generate(floor, out _mapInfo);
        DungeonPainter.Instance.Paint(_mapInfo);

        //_player = GameObject.Find("ExploreCharacter").GetComponent<ExploreCharacter>();
        //_player.transform.position = (Vector2)_mapInfo.Start;
        //ExploreUI.Open();
        //ExploreUI.Instance.InitLittleMap(_mapInfo.DungeonData.Floor, Vector2Int.RoundToInt(_player.transform.position), _mapInfo.Start, _mapInfo.Goal, _mapInfo.MapBound, _mapInfo.MapList);
        //SetVisibleRange(true);
        //ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _exploredList, _wallList);

        //SetInteractive(Vector2Int.RoundToInt(_player.transform.position));

        //_guardList.Add(_mapInfo.Goal);
        //GenerateEnemy();
        _playerPosition = _mapInfo.Start;
        _guardList.Add(_mapInfo.Goal);
        SetFloor();
    }

    public void SetFloor()
    {
        if (_mapInfo == null)
        {
            SetFloorFromMemo();
            return;
        }

        DungeonPainter.Instance.Paint(_mapInfo);

        _player = GameObject.Find("ExploreCharacter").GetComponent<ExploreCharacter>();
        _player.transform.position = _playerPosition;
        ExploreUI.Open();
        ExploreUI.Instance.InitLittleMap(_mapInfo.DungeonData.Floor, Vector2Int.RoundToInt(_player.transform.position), _mapInfo.Start, _mapInfo.Goal, _mapInfo.MapBound, _mapInfo.MapList);
        SetVisibleRange(true);
        ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _exploredList, _exploredWallList);
        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
        GenerateEnemy();
    }

    public void SetFloorFromMemo()
    {

        MapMemo memo = Caretaker.Instance.Load<MapMemo>();
        _mapInfo = new MapInfo(memo);
        DungeonPainter.Instance.Paint(_mapInfo);

        _player = GameObject.Find("ExploreCharacter").GetComponent<ExploreCharacter>();
        _player.transform.position = (Vector2)memo.PlayerPosition;
        ExploreUI.Open();
        ExploreUI.Instance.InitLittleMap(_mapInfo.DungeonData.Floor, Vector2Int.RoundToInt(_player.transform.position), _mapInfo.Start, _mapInfo.Goal, _mapInfo.MapBound, _mapInfo.MapList);
        SetVisibleRange(true);
        _exploredList = Utility.StringToVector2Int(memo.ExploredList);
        _exploredWallList = Utility.StringToVector2Int(memo.ExploredWallList);
        ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _exploredList, _exploredWallList);
        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
        _guardList = Utility.StringToVector2Int(memo.GuardList);
        GenerateEnemy();
    }

    //public void Load()
    //{
    //    _mapInfo = new MapInfo(Caretaker.Instance.Load<MapMemo>());
    //}

    public void Save()
    {
        _playerPosition = Vector2Int.RoundToInt(_player.transform.position);
        Caretaker.Instance.Save<MapMemo>(new MapMemo(ArriveFloor, _mapInfo, _playerPosition, _exploredList, _exploredWallList, _guardList));
    }

    public void ChangeFloor(int floor)
    {
        GenerateFloor(floor);

        if (floor > ArriveFloor)
        {
            ArriveFloor = floor;
        }
    }

    public void PlayerMove(Vector2Int direction)
    {
        Vector2Int newPosition = Vector2Int.RoundToInt(_player.transform.position) + direction;

        if (CanMove(newPosition, true, true))
        {
            _player.Move(direction, () =>
            {
                SetInteractive(newPosition);

                //不要刪
                if (newPosition == _mapInfo.Start)
                {
                    ExploreUI.Instance.OpenStairsGroup(_mapInfo.DungeonData.LastFloor);
                }
                else if (newPosition == _mapInfo.Goal)
                {
                    ExploreUI.Instance.OpenStairsGroup(_mapInfo.DungeonData.NextFloor);
                }

                SetVisibleRange(false);
                ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _exploredList, _exploredWallList);
            });
        }
    }

    public void PlayerPause() 
    {
        _player.Pause();
    }

    public void Interactive(Vector2Int position)
    {
        if (_mapInfo.TreasureDic.ContainsKey(position))
        {
            for (int i=0; i< _mapInfo.TreasureDic[position].ItemList.Count; i++)
            {
                Debug.Log(ItemData.GetData(_mapInfo.TreasureDic[position].ItemList[i]).GetName());
            }
            ItemManager.Instance.AddItem(_mapInfo.TreasureDic[position].ItemList, ItemManager.Type.Bag);
            TilePainter.Instance.Clear(2, position);
            ItemConfirmUI.Open(_mapInfo.TreasureDic[position].ItemList);
            _mapInfo.TreasureDic.Remove(position);

            if (ItemManager.Instance.BagIsFull())
            {
                ConfirmUI.Open("背包已滿！請選擇要丟棄的道具。", "確定", () =>
                {
                    BagUI.Open(ItemManager.Type.Bag);
                });
            }
        }
        else if (_mapInfo.DoorList.Contains(position))
        {
            if (ItemManager.Instance.UseKey())
            {
                TilePainter.Instance.Clear(2, position);
                _mapInfo.DoorList.Remove(position);
                SetVisibleRange(false);
                ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _exploredList, _exploredWallList);
            }
            else
            {
                ConfirmUI.Open("需要一把鑰匙將鎖打開。", "確定", null);
            }
        }
        else if (_mapInfo.ExploreEventDic.ContainsKey(position))
        {
            _player.Stop();
            EventUI.Open(_mapInfo.ExploreEventDic[position], (isDonothing) =>
            {
                _player.UnlockStop();
                if (!isDonothing)
                {
                    TilePainter.Instance.Clear(2, position);
                    _mapInfo.ExploreEventDic.Remove(position);
                }
            });
        }
        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
    }

    public void ForceEnterBattle() //作弊用,強迫進入戰鬥
    {
        DungeonBattleGroupData.RootObject data = DungeonBattleGroupData.GetData(_mapInfo.DungeonData.ID);
        EnterBattle(data.GetRandomBattleGroup());
    }

    public void BackToVilliage()
    {
        for (int i = 0; i < _fieldEnemyList.Count; i++)
        {
            _fieldEnemyList[i].Stop();
        }
        ExploreUI.Instance.StopTipLabel();

        MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage, () =>
        {
            ItemManager.Instance.PutBagItemIntoWarehouse();
            TeamManager.Instance.RecoverAllMember();
            //FarmManager.Instance.ChangeState();
            //VilliageController.Instance.SetData();
        });
    }

    public bool CanMove(Vector2Int position, bool ignorePlayer, bool ignoreEnemy) //ignorePlayer 忽略玩家, ignoreEnemy 忽略敵人
    {
        if (!ignorePlayer && position == Vector2Int.RoundToInt(_player.transform.position))
        {
            return false;
        }

        if (!ignoreEnemy)
        {
            for (int i = 0; i < _fieldEnemyList.Count; i++)
            {
                if (position == Vector2Int.RoundToInt(_fieldEnemyList[i].transform.position))
                {
                    return false;
                }
            }
        }

        if (_mapInfo.MapList.Contains(position) && !_mapInfo.TreasureDic.ContainsKey(position) && !_mapInfo.DoorList.Contains(position) && !_mapInfo.ExploreEventDic.ContainsKey(position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TeleportPlayer()
    {
        Vector2Int position = GetLegalPosition();
        _player.transform.position = (Vector2)position;
        SetVisibleRange(true);
        ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _exploredList, _exploredWallList);
        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
    }

    public void EnterBattle(int battleGroupId)
    {
        //AudioSystem.Instance.Stop(true);
        for (int i = 0; i < _fieldEnemyList.Count; i++)
        {
            _fieldEnemyList[i].Stop();
        }
        ExploreUI.Instance.StopTipLabel();
        Save();

        ChangeSceneUI.Instance.StartClock(() =>
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle, () =>
            {
                BattleController.Instance.Init(1, battleGroupId);
            });
        });

        _playerPosition = Vector2Int.RoundToInt(_player.transform.position);
        _player.Stop();
    }

    public bool IsWall(Vector2Int position)
    {
        return !_mapInfo.MapList.Contains(position);
    }

    private Vector2Int GetLegalPosition(List<Vector2Int> positionList = null) //取得合法位置(空地)
    {
        if (positionList == null)
        {
            positionList = _mapInfo.MapList;
        }

        Vector2Int position;
        List<Vector2Int> tempList = new List<Vector2Int>(positionList);
        while (tempList.Count > 0)
        {
            position = tempList[Random.Range(0, tempList.Count)];
            if (CanMove(position, false, false))
            {
                return position;
            }
            tempList.Remove(position);
        }
        return Vector2Int.zero;
    }

    private void SetInteractive(Vector2Int position)
    {
        List<Vector2Int> interactiveList = new List<Vector2Int>(); //寶箱等可互動物件的位置

        for (int i = 0; i < _directions.Length; i++)
        {
            if (_mapInfo.TreasureDic.ContainsKey(position + _directions[i]) || _mapInfo.DoorList.Contains(position + _directions[i]) || _mapInfo.ExploreEventDic.ContainsKey(position + _directions[i]))
            {
                interactiveList.Add(position + _directions[i]);
            }
        }
        ExploreUI.Instance.SetInetractiveButtonVisible(interactiveList);

        if (_mapInfo.KeyList.Contains(position))
        {
            ItemManager.Instance.AddKey();
            TilePainter.Instance.Clear(2, position);
            _mapInfo.KeyList.Remove(position);
            ExploreUI.Instance.TipLabel.SetLabel("得到了一把鑰匙");
        }
        else if (_mapInfo.MoneyDic.ContainsKey(position))
        {
            ItemManager.Instance.AddMoney(_mapInfo.MoneyDic[position]);
            //TilePainter.Instance.Clear(2, position);
            ExploreUI.Instance.TipLabel.SetLabel("得到 " + _mapInfo.MoneyDic[position] + " $");
            _mapInfo.MoneyDic.Remove(position);
        }
        /*else if (_mapInfo.ExploreEventDic.ContainsKey(position))
        {
            _player.Stop();
            EventUI.Open(_mapInfo.ExploreEventDic[position],(isDonothing)=> 
            {
                _player.UnlockStop();
                if (!isDonothing)
                {
                    TilePainter.Instance.Clear(2, position);
                    _mapInfo.ExploreEventDic.Remove(position);
                }
            });
        }*/
    }

    private void SetVisibleRange(bool isInit)
    {
        //if (isInit)
        //{
        //    _exploredList.Clear();
        //    _wallList.Clear();
        //}

        Vector2Int playerPosition = Vector2Int.RoundToInt(_player.transform.position);
        Vector2Int circlePoint = new Vector2Int();
        List<Vector2Int> circleList = new List<Vector2Int>();
        List<Vector2Int> lineList = new List<Vector2Int>();

        _exploredList.Add(playerPosition);
        TilePainter.Instance.Clear(3, playerPosition);
        _mapInfo.MistList.Remove(playerPosition);
        circleList = Utility.GetCirclePositionList(playerPosition, 5, !isInit);
        for (int i=0; i<circleList.Count; i++)
        {
            circlePoint = circleList[i];
            lineList = Utility.GetLinePositionList(playerPosition, circlePoint);

            if (lineList[0] != playerPosition)
            {
                lineList.Reverse();
            }
            lineList.RemoveAt(0);

            for (int j = 0; j < lineList.Count; j++)
            {
                if (!_mapInfo.MapList.Contains(lineList[j]) || _mapInfo.DoorList.Contains(lineList[j]))
                {
                    if(!_exploredWallList.Contains(lineList[j]))
                        _exploredWallList.Add(lineList[j]);
                        
                    //if(!_exploredList.Contains(lineList[j]))
                    //    _exploredList.Add(lineList[j]);
                    //TilePainter.Instance.Clear(2, lineList[j]);
                    //_mapInfo.MistList.Remove(lineList[j]);
                    break;
                }

                if (!_exploredList.Contains(lineList[j]))
                    _exploredList.Add(lineList[j]);
                TilePainter.Instance.Clear(3, lineList[j]);
                _mapInfo.MistList.Remove(lineList[j]);
            }
        }
    }

    private void GenerateEnemy()
    {
        for (int i=0; i< _fieldEnemyList.Count; i++)
        {
            if (_fieldEnemyList[i] != null)
            {
                _fieldEnemyList[i].Stop();
                GameObject.Destroy(_fieldEnemyList[i].gameObject);
            }
        }

        _fieldEnemyList.Clear();
        FieldEnemy enemy;

        Vector2Int position;
        for (int i = 0; i < _mapInfo.RoomPositionList.Count; i++)
        {
            for (int j = 0; j < 2; j++) //每個房間生兩隻怪
            {
                position = GetLegalPosition(_mapInfo.RoomPositionList[i]);
                if (Vector2.Distance(position, _player.transform.position) > 10) //如果位置不會離玩家太近
                {
                    enemy = ResourceManager.Instance.Spawn("FieldEnemy/FieldEnemyRandom", ResourceManager.Type.Other).GetComponent<FieldEnemy>();
                    enemy.OnPlayerEnterHandler += EnterBattle;
                    DungeonBattleGroupData.RootObject data = DungeonBattleGroupData.GetData(_mapInfo.DungeonData.ID);
                    enemy.Init(data.GetRandomBattleGroup(), position);
                    _fieldEnemyList.Add(enemy);
                }
            }
        }

        if (_guardList.Contains(_mapInfo.Goal))
        {
            enemy = ResourceManager.Instance.Spawn("FieldEnemy/FieldEnemyGuard", ResourceManager.Type.Other).GetComponent<FieldEnemy>();
            enemy.OnPlayerEnterHandler += EnterBattle;
            ((FieldEnemyGuard)enemy).CheckPositionHandler += EncounterGuard;
            DungeonBattleGroupData.RootObject data = DungeonBattleGroupData.GetData(_mapInfo.DungeonData.ID);
            enemy.Init(data.GoalBattleGroup, _mapInfo.Goal);
            _fieldEnemyList.Add(enemy);
        }
    }

    private void EncounterGuard(Vector2 position)
    {
        _guardList.Remove(Vector2Int.RoundToInt(position));
    }
}
