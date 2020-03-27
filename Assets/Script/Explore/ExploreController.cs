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

    private int _floor = 1;
    private MapInfo _mapInfo;
    private ExploreCharacter _player;
    private Vector2 _playerPosition;
    private List<Vector2Int> _exploredList = new List<Vector2Int>(); //走過的地圖範圍
    private List<Vector2Int> _wallList = new List<Vector2Int>(); //已被發現的牆壁的範圍
    private List<FieldEnemy> _fieldEnemyList = new List<FieldEnemy>();
    private Vector2Int[] _directions = new Vector2Int[4] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    public void SetData(MapInfo info)
    {
        DungeonPainter.Instance.Paint(info);

        _mapInfo = info;
        _player = GameObject.Find("ExploreCharacter").GetComponent<ExploreCharacter>();
        _player.transform.position = (Vector2)info.Start;
        SetVisibleRange(true);
        ExploreUI.Open();
        ExploreUI.Instance.InitLittleMap(Vector2Int.RoundToInt(_player.transform.position), _mapInfo.MapBound, _mapInfo.MapList, _exploredList);
        ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _exploredList, _wallList);

        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));

        GenerateEnemy();
    }

    public void Reload()
    {
        DungeonPainter.Instance.Paint(_mapInfo);

        _player = GameObject.Find("ExploreCharacter").GetComponent<ExploreCharacter>();
        _player.transform.position = _playerPosition;

        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
        ExploreUI.Instance.ReloadLittleMap(Vector2Int.RoundToInt(_player.transform.position), _mapInfo.MapBound, _mapInfo.MapList, _exploredList);
        ExploreUI.Instance.RefreshLittleMap(Vector2Int.RoundToInt(_player.transform.position), _exploredList, _wallList);
        GenerateEnemy();
    }

    public void ChangeFloor(int floor)
    {
        MapInfo info;
        DungeonBuilder.Instance.Generate(floor, out info);
        SetData(info);
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
                /*if (newPosition == _mapInfo.Start)
                {
                    ExploreUI.Instance.OpenStairsGroup(_floor - 1);
                }
                else if (newPosition == _mapInfo.Goal)
                {
                    ExploreUI.Instance.OpenStairsGroup(_floor + 1);
                }*/

                SetVisibleRange(false);
                ExploreUI.Instance.RefreshLittleMap(newPosition, _exploredList, _wallList);
            });
        }
    }

    public void PlayerStop() 
    {
        _player.Stop();
    }

    public void Interactive(Vector2Int position)
    {
        if (_mapInfo.TreasureDic.ContainsKey(position))
        {
            for (int i=0; i< _mapInfo.TreasureDic[position].ItemList.Count; i++)
            {
                Debug.Log(ItemData.GetData(_mapInfo.TreasureDic[position].ItemList[i]).Name);
            }
            ItemManager.Instance.AddItem(_mapInfo.TreasureDic[position].ItemList, ItemManager.Type.Bag);
            TilePainter.Instance.Clear(1, position);
            IconConfirmUI.Open(_mapInfo.TreasureDic[position].ItemList);
            _mapInfo.TreasureDic.Remove(position);

            if (ItemManager.Instance.BagIsFull())
            {
                ConfirmUI.Open("背包已滿！請選擇要丟棄的道具。", "確定", () =>
                {
                    BagUI.Open(ItemManager.Type.Bag);
                });
            }
        }
        if (_mapInfo.DoorDic.ContainsKey(position))
        {
            if (ItemManager.Instance.UseKey())
            {
                TilePainter.Instance.Clear(1, position);
                _mapInfo.DoorDic.Remove(position);
                SetVisibleRange(false);
            }
            else
            {
                ConfirmUI.Open("需要一把鑰匙將鎖打開。", "確定", null);
            }
        }
        SetInteractive(Vector2Int.RoundToInt(_player.transform.position));
    }

    public void ForceEnterBattle() //作弊用,強迫進入戰鬥
    {
        Vector2Int newPosition = Vector2Int.RoundToInt(_player.transform.position);
        //EnterBattle();
    }

    public void BackToVilliage()
    {
        MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage, () =>
        {
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

        if (_mapInfo.MapList.Contains(position) && !_mapInfo.TreasureDic.ContainsKey(position) && !_mapInfo.DoorDic.ContainsKey(position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector2Int GetLegalPosition(List<Vector2Int> positionList) //取得合法位置(空地)
    {
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
            if (_mapInfo.TreasureDic.ContainsKey(position + _directions[i]) || _mapInfo.DoorDic.ContainsKey(position + _directions[i]))
            {
                interactiveList.Add(position + _directions[i]);
            }
        }
        ExploreUI.Instance.SetInetractiveButtonVisible(interactiveList);

        if (_mapInfo.KeyList.Contains(position))
        {
            ItemManager.Instance.AddKey();
            TilePainter.Instance.Clear(1, position);
            _mapInfo.KeyList.Remove(position);
            ExploreUI.Instance.TipLabel.SetLabel("得到了一把鑰匙");
        }
        else if (_mapInfo.MoneyDic.ContainsKey(position))
        {
            ItemManager.Instance.AddMoney(_mapInfo.MoneyDic[position]);
            TilePainter.Instance.Clear(1, position);
            ExploreUI.Instance.TipLabel.SetLabel("得到 " + _mapInfo.MoneyDic[position] + " $");
            _mapInfo.MoneyDic.Remove(position);
        }
        else if (_mapInfo.ExploreEventDIc.ContainsKey(position))
        {
            //ExploreEventData explorePointData = ExploreEventData.GetData(_mapInfo.ExploreEventDIc[position]);
            //ConversationUI.Open(explorePointData.ConversationID);
            //TilePainter.Instance.Clear(1, position);
            //_mapInfo.ExploreEventDIc.Remove(position);
        }
    }

    private void SetVisibleRange(bool isInit)
    {
        if (isInit)
        {
            _exploredList.Clear();
            _wallList.Clear();
        }

        Vector2Int playerPosition = Vector2Int.RoundToInt(_player.transform.position);
        Vector2Int circlePoint = new Vector2Int();
        List<Vector2Int> circleList = new List<Vector2Int>();
        List<Vector2Int> lineList = new List<Vector2Int>();
        List<Vector2Int> checkList = new List<Vector2Int>();

        TilePainter.Instance.Clear(2, playerPosition);
        circleList = Utility.GetCirclePositionList(playerPosition, 6, false);
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
                if (!_exploredList.Contains(lineList[j]))
                {
                    if (!_mapInfo.MapList.Contains(lineList[j]) || _mapInfo.DoorDic.ContainsKey(lineList[j]))
                    {
                        _wallList.Add(lineList[j]);
                        TilePainter.Instance.Clear(2, lineList[j]);
                        _mapInfo.MistList.Remove(lineList[j]);
                        break;
                    }

                    _exploredList.Add(lineList[j]);
                    circleList.Remove(lineList[j]);
                    if (lineList[j] == circlePoint)
                    {
                        i--;
                    }
                    if (j == lineList.Count - 1)
                    {
                        checkList.Add(lineList[j]);
                    }
                    TilePainter.Instance.Clear(2, lineList[j]);
                    _mapInfo.MistList.Remove(lineList[j]);
                }
            }
        }

        //for (int i=0; i<_exploredList.Count; i++) 
        //{
        //    TilePainter.Instance.Painting("Mask", 2, _exploredList[i]);
        //    if (_exploredList.Contains(_exploredList[i] + Vector2Int.up) && _exploredList.Contains(_exploredList[i] + Vector2Int.down) && _exploredList.Contains(_exploredList[i] + Vector2Int.left) && _exploredList.Contains(_exploredList[i] + Vector2Int.right))
        //    {
        //        TilePainter.Instance.Clear(2, _exploredList[i]);
        //    }

        //    if (_wallList.Contains(_exploredList[i] + Vector2Int.up) || _wallList.Contains(_exploredList[i] + Vector2Int.down) || _wallList.Contains(_exploredList[i] + Vector2Int.left) || _wallList.Contains(_exploredList[i] + Vector2Int.right))
        //    {
        //        TilePainter.Instance.Clear(2, _exploredList[i]);
        //    }
        //}
    }

    private void EnterBattle()
    {
        _playerPosition = _player.transform.position;

        int battleGroupId = DungeonBattleGroupData.GetRandomBattleGroup(_mapInfo.DungeonId);
        List<KeyValuePair<int, int>> enemyList = BattleGroupData.GetEnemy(battleGroupId);
        //AudioSystem.Instance.Stop(true);
        ChangeSceneUI.Instance.StartClock(()=> 
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle, () =>
            {
                BattleController.Instance.Init(1, enemyList);
            });
        });

        for (int i=0; i<_fieldEnemyList.Count; i++)
        {
            _fieldEnemyList[i].Stop();
        }
    }

    private void GenerateEnemy()
    {
        _fieldEnemyList.Clear();
        Vector2Int position;
        for (int i = 0; i < _mapInfo.RoomList.Count; i++)
        {
            for (int j=0; j<2; j++) //每個房間生兩隻怪
            {
                position = GetLegalPosition(_mapInfo.RoomList[i].PositionList);
                if (Vector2.Distance(position, _player.transform.position) > 10) //如果位置不會離玩家太近
                {
                    FieldEnemy enemy = ResourceManager.Instance.Spawn("FieldEnemy/FieldEnemyRandom", ResourceManager.Type.Other).GetComponent<FieldEnemy>();
                    enemy.OnPlayerEnterHandler += EnterBattle;
                    _fieldEnemyList.Add(enemy);
                    enemy.transform.position = (Vector2)position;
                }
            }
        }
    }
}
