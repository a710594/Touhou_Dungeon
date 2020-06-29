using System;
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

    public int ArriveFloor = 1;

    private ExploreCharacter _player;
    private MapInfo _mapInfo;
    private MapMemo _memo; // 存檔用的
    private Vector2 _playerPosition;
    private List<GameObject> _coinList = new List<GameObject>();
    private List<FieldEnemy> _fieldEnemyList = new List<FieldEnemy>();
    private List<Vector2Int> _pathFindList = new List<Vector2Int>();
    private Vector2Int[] _directions = new Vector2Int[4] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    public void Init()
    {
        MapMemo memo = Caretaker.Instance.Load<MapMemo>();
        if (memo != null)
        {
            ArriveFloor = memo.ArriveFloor;
        }

    }

    public void GenerateFloor(int floor)
    {
        DungeonBuilder.Instance.Generate(floor);
        _mapInfo = DungeonBuilder.Instance.MapInfo;
        DungeonPainter.Instance.Paint(_mapInfo);

        _playerPosition = _mapInfo.Start;
        _mapInfo.GuardList.Add(_mapInfo.Goal);
        SetFloor();
    }

    public void SetFloor()
    {
        //if (_mapInfo == null)
        //{
        //    SetFloorFromMemo();
        //    return;
        //}

        DungeonPainter.Instance.Paint(_mapInfo);

        _player = GameObject.Find("ExploreCharacter").GetComponent<ExploreCharacter>();
        _player.transform.position = _playerPosition;
        ExploreUI.Open();
        ExploreUI.Instance.InitLittleMap(_mapInfo.DungeonData.Floor, Vector2Int.RoundToInt(_player.transform.position), _mapInfo.Start, _mapInfo.Goal, _mapInfo.MapBound, _mapInfo.MapList);
        SetVisibleRange(true);
        ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _mapInfo.ExploredList, _mapInfo.ExploredWallList);
        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));

        _pathFindList = new List<Vector2Int>(_mapInfo.MapList);
        foreach (KeyValuePair<Vector2Int, Treasure> item in _mapInfo.TreasureDic)
        {
            _pathFindList.Remove(item.Key);
        }
        for (int i=0; i<_mapInfo.DoorList.Count; i++)
        {
            _pathFindList.Remove(_mapInfo.DoorList[i]);
        }
        foreach (KeyValuePair<Vector2Int, int> item in _mapInfo.ExploreEventDic)
        {
            _pathFindList.Remove(item.Key);
        }

        GenerateCoin();
        GenerateEnemy();
    }

    public void SetFloorFromMemo()
    {
        if (_memo == null)
        {
            _memo = Caretaker.Instance.Load<MapMemo>();
            _mapInfo = new MapInfo(_memo);
        }

        _playerPosition = _memo.PlayerPosition;
        ArriveFloor = _memo.ArriveFloor;
        SetFloor();
    }

    public void Save()
    {
        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore)
        {
            _playerPosition = Vector2Int.RoundToInt(_player.transform.position);
            _memo = new MapMemo(ArriveFloor, _mapInfo, _playerPosition);
        }
        if (_memo != null)
        {
            Caretaker.Instance.Save<MapMemo>(_memo);
        }
    }

    public void ChangeFloor(int floor)
    {
        LoadingUI.Instance.Open(() =>
        {
            GenerateFloor(floor);
        });

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
                ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _mapInfo.ExploredList, _mapInfo.ExploredWallList);
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
            ItemManager.Instance.AddItem(_mapInfo.TreasureDic[position].ItemList, ItemManager.Type.Bag);
            TilePainter.Instance.Clear(2, position);
            ItemConfirmUI.Open(_mapInfo.TreasureDic[position].ItemList);
            _mapInfo.TreasureDic.Remove(position);
            _pathFindList.Add(position);

            if (ItemManager.Instance.BagIsFull())
            {
                ConfirmUI.Open("背包已滿！請選擇要丟棄的道具。", "確定", () =>
                {
                    BagUI.Open(ItemManager.Type.Bag);
                });
            }
            SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
        }
        else if (_mapInfo.DoorList.Contains(position))
        {
            if (ItemManager.Instance.UseKey())
            {
                TilePainter.Instance.Clear(2, position);
                _mapInfo.DoorList.Remove(position);
                _pathFindList.Add(position);
                SetVisibleRange(false);
                ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _mapInfo.ExploredList, _mapInfo.ExploredWallList);
            }
            else
            {
                ConfirmUI.Open("需要一把鑰匙將鎖打開。", "確定", null);
            }
            SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
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
                    _pathFindList.Add(position);
                    SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
                }
            });
        }
    }

    public void ForceEnterBattle() //事件或測試時使用
    {
        DungeonBattleGroupData.RootObject data = DungeonBattleGroupData.GetData(_mapInfo.DungeonData.ID);
        EnterBattle(BattleGroupData.GetData(data.GetRandomBattleGroup()));
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
        ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _mapInfo.ExploredList, _mapInfo.ExploredWallList);
        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
    }

    public void EnterBattle(BattleGroupData.RootObject data)
    {
        //AudioSystem.Instance.Stop(true);
        for (int i = 0; i < _fieldEnemyList.Count; i++)
        {
            _fieldEnemyList[i].Stop();
        }
        ExploreUI.Instance.StopTipLabel();

        ChangeSceneUI.Instance.StartClock(() =>
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle, () =>
            {
                BattleController.Instance.Init(1, data);
            });
        });

        _playerPosition = Vector2Int.RoundToInt(_player.transform.position);
        _player.Stop();
        _memo = new MapMemo(ArriveFloor, _mapInfo, _playerPosition);
    }

    public bool IsWall(Vector2Int position)
    {
        return !_mapInfo.MapList.Contains(position);
    }

    private Vector2Int GetLegalPosition(List<Vector2Int> positionList = null) //取得合法位置(空地)
    {
        if (positionList == null)
        {
            positionList = _mapInfo.RoomPositionList[UnityEngine.Random.Range(0, _mapInfo.RoomPositionList.Count)];
        }

        Vector2Int position;
        List<Vector2Int> tempList = new List<Vector2Int>(positionList);
        while (tempList.Count > 0)
        {
            position = tempList[UnityEngine.Random.Range(0, tempList.Count)];
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
            ExploreUI.Instance.TipLabel.SetLabel("得到 " + _mapInfo.MoneyDic[position] + " $");
            _mapInfo.MoneyDic.Remove(position);
        }
    }

    private void SetVisibleRange(bool isInit)
    {
        Vector2Int playerPosition = Vector2Int.RoundToInt(_player.transform.position);
        Vector2Int circlePoint = new Vector2Int();
        List<Vector2Int> circleList = new List<Vector2Int>();
        List<Vector2Int> lineList = new List<Vector2Int>();

        _mapInfo.ExploredList.Add(playerPosition);
        TilePainter.Instance.Clear(3, playerPosition);
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
                    if(!_mapInfo.ExploredWallList.Contains(lineList[j]))
                        _mapInfo.ExploredWallList.Add(lineList[j]);
                        
                    //if(!_exploredList.Contains(lineList[j]))
                    //    _exploredList.Add(lineList[j]);
                    //TilePainter.Instance.Clear(2, lineList[j]);
                    //_mapInfo.MistList.Remove(lineList[j]);
                    break;
                }

                if (!_mapInfo.ExploredList.Contains(lineList[j]))
                    _mapInfo.ExploredList.Add(lineList[j]);
                TilePainter.Instance.Clear(3, lineList[j]);
            }
        }
    }

    private void GenerateCoin()
    {
        for (int i = 0; i < _coinList.Count; i++)
        {
            if (_coinList[i] != null)
            {
                GameObject.Destroy(_coinList[i].gameObject);
            }
        }

        _coinList.Clear();
        GameObject coin;
        foreach (KeyValuePair<Vector2Int, int> item in _mapInfo.MoneyDic)
        {
            //TilePainter.Instance.Painting("Money", 2, item.Key);
            coin = ResourceManager.Instance.Spawn("Coin", ResourceManager.Type.Other);
            coin.transform.position = (Vector2)item.Key;
            _coinList.Add(coin);
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

                    if (enemy is FieldEnemyRandom)
                    {
                        ((FieldEnemyRandom)enemy).SetData(_player.transform, _pathFindList);
                    }
                }
            }
        }

        if (_mapInfo.GuardList.Contains(_mapInfo.Goal))
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
        _mapInfo.GuardList.Remove(Vector2Int.RoundToInt(position));
    }
}
