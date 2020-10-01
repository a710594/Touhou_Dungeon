using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    public static DungeonBuilder Instance;

    private int _loadingCount = 0;
    private List<Vector2Int> _mapList = new List<Vector2Int>();
    private List<Vector2Int> _normalRoomMapList = new List<Vector2Int>(); //一般房間的地圖
    private List<Vector2Int> _treasureRoomMapList = new List<Vector2Int>(); //寶藏房間的地圖
    private List<KeyValuePair<Vector2Int, Room>> _wallDirectionDic = new List<KeyValuePair<Vector2Int, Room>>();

    public void Generate(DungeonData.RootObject data)
    {
        StartCoroutine(generate(data));
    }

    public IEnumerator generate(DungeonData.RootObject data)
    {
        int roomId;
        int random;
        Room room;
        Vector2Int position;
        Vector2Int roomPosition;
        List<KeyValuePair<Vector2Int, Room>> tempWallDirectionDic = new List<KeyValuePair<Vector2Int, Room>>();
        List<Room> roomList = new List<Room>();
        List<Room> treasureRoomList = new List<Room>();

        _mapList.Clear();
        _normalRoomMapList.Clear();
        _treasureRoomMapList.Clear();
        _wallDirectionDic.Clear();

        //First Room
        roomId = data.RoomList[0];
        room = RoomData.GetRoomClassByID(roomId);
        room.SetData(RoomData.GetData(roomId));
        roomList.Add(room);
        room.SetPosition(Vector2Int.zero);
        SetRoom(room);
        AddWallDirectionList(room);

        yield return null;

        //The other room
        for (int i = 0; i < data.RoomAmount - 1; i++)
        {
            roomId = data.GetRandomRoomID();
            room = RoomData.GetRoomClassByID(roomId);
            room.SetData(RoomData.GetData(roomId));
            if (room is TreasureRoom)
            {
                treasureRoomList.Add(room);
            }
            else
            {
                roomList.Add(room);
            }

            tempWallDirectionDic = new List<KeyValuePair<Vector2Int, Room>>(_wallDirectionDic);
            while (tempWallDirectionDic.Count > 0)
            {
                random = Random.Range(0, tempWallDirectionDic.Count);
                GetRoomPosition(tempWallDirectionDic[random].Key, tempWallDirectionDic[random].Value, room, out roomPosition);
                tempWallDirectionDic.RemoveAt(random);
                if (ScanSpace(roomPosition - Vector2Int.one, room.Width + 2, room.Height + 2))
                {
                    room.SetPosition(roomPosition);
                    SetRoom(room);
                    AddWallDirectionList(room);
                    _wallDirectionDic.Remove(_wallDirectionDic[i]);
                    break;
                }
            }
            yield return null;
        }

        //取得房間以外的空間
        BoundsInt mapBound = Utility.GetMapBounds(_mapList);
        List<Vector2Int> outsideList = new List<Vector2Int>();
        for (int i = mapBound.xMin - 2; i <= mapBound.xMax + 2; i++)
        {
            for (int j = mapBound.yMin - 2; j <= mapBound.yMax + 2; j++)
            {
                position = new Vector2Int(i, j);
                if (!_mapList.Contains(position) && !_mapList.Contains(position + Vector2Int.left) && !_mapList.Contains(position + Vector2Int.right) && !_mapList.Contains(position + Vector2Int.up) && !_mapList.Contains(position + Vector2Int.down)
                    && !_mapList.Contains(position + Vector2Int.left + Vector2Int.up) && !_mapList.Contains(position + Vector2Int.left + Vector2Int.down) && !_mapList.Contains(position + Vector2Int.right + Vector2Int.up) && !_mapList.Contains(position + Vector2Int.right + Vector2Int.down))
                {
                    outsideList.Add(position);
                }
                if (CheckLoadingCount()) yield return null;
            }
        }

        //Cave
        for (int i = 0; i < 50; i++)
        {
            if (CaveGenerator.Instance.GetPosition(_mapList, _treasureRoomMapList, null, out position))
            {
                _mapList.Add(position);
            }
        }
        yield return null;

        //Generate path
        Vector2Int fromDoor;
        Vector2Int toDoor;
        List<Vector2Int> tempList = new List<Vector2Int>();
        List<Vector2Int> pathList = new List<Vector2Int>();
        for (int i = 1; i < roomList.Count; i++)
        {
            //GeneratePath(roomList[i - 1], roomList[i]);

            fromDoor = roomList[i - 1].GetDoor();
            toDoor = roomList[i].GetDoor();
            tempList = new List<Vector2Int>(outsideList);
            tempList.Add(fromDoor);
            tempList.Add(toDoor);
            if (!GeneratePath(fromDoor, toDoor, tempList)) //如果道路沒有生成功
            {
                i--;
            }
            yield return null;
        }

        //Random path
        List<Room> tempRoomList;
        for (int i = 0; i < 2; i++)
        {
            tempRoomList = new List<Room>(roomList);
            room = tempRoomList[Random.Range(0, tempRoomList.Count)];
            fromDoor = room.GetDoor();
            tempRoomList.Remove(room);
            room = tempRoomList[Random.Range(0, tempRoomList.Count)];
            toDoor = room.GetDoor();
            tempList = new List<Vector2Int>(outsideList);
            tempList.Add(fromDoor);
            tempList.Add(toDoor);
            if (!GeneratePath(fromDoor, toDoor, tempList)) //如果道路沒有生成功
            {
                i--;
            }
            yield return null;
        }

        //Treasure Room path
        List<Vector2Int> doorList = new List<Vector2Int>();
        for (int i = 0; i < treasureRoomList.Count; i++)
        {
            //GeneratePath(roomList[i - 1], roomList[i]);

            fromDoor = treasureRoomList[i].GetDoor();
            toDoor = roomList[Random.Range(0, roomList.Count)].GetDoor();
            tempList = new List<Vector2Int>(outsideList);
            tempList.Add(fromDoor);
            tempList.Add(toDoor);
            if (!GeneratePath(fromDoor, toDoor, tempList)) //如果道路沒有生成功
            {
                i--;
            }
            else
            {
                doorList.Add(fromDoor);
            }
            yield return null;
        }

        //Grass 
        //裝飾用的地形
        List<Vector2Int> grassList = new List<Vector2Int>();
        tempList = new List<Vector2Int>(_mapList);
        for (int i = 0; i < _mapList.Count / 16; i++) //起始草地的數量是土地的1/16
        {
            position = tempList[Random.Range(0, tempList.Count)];
            grassList.Add(position);
            tempList.Remove(position);
            if (CheckLoadingCount()) yield return null;
        }

        int count = 0;
        for (int i = 0; i < _mapList.Count / 4; i++) //旁生草地的數量是土地的1/4
        {
            if (CaveGenerator.Instance.GetPosition(grassList, null, _mapList, out position))
            {
                grassList.Add(position);
                count++;
                if (CheckLoadingCount()) yield return null;
            }
        }

        List<Vector2Int> spaceList = new List<Vector2Int>(_normalRoomMapList); //還沒有擺放物件的空地

        //Treasure
        int monetAmount; //錢的數量是寶箱房以外的寶箱的兩倍
        Dictionary<Vector2Int, Treasure> treasureDic = new Dictionary<Vector2Int, Treasure>();
        for (int i = 0; i < roomList.Count; i++)
        {
            foreach (KeyValuePair<Vector2Int, Treasure> item in roomList[i].TreasureDic)
            {
                treasureDic.Add(item.Key, item.Value);
                spaceList.Remove(item.Key);
            }
        }
        monetAmount = treasureDic.Count * 2;

        for (int i = 0; i < treasureRoomList.Count; i++)
        {
            foreach (KeyValuePair<Vector2Int, Treasure> item in treasureRoomList[i].TreasureDic)
            {
                treasureDic.Add(item.Key, item.Value);
                spaceList.Remove(item.Key);
            }
        }

        //Key
        List<Vector2Int> keyList = new List<Vector2Int>();

        for (int i = 0; i < treasureRoomList.Count; i++)
        {
            position = spaceList[Random.Range(0, spaceList.Count)];
            keyList.Add(position);
            spaceList.Remove(position);
        }

        //money
        Dictionary<Vector2Int, int> moneyDic = new Dictionary<Vector2Int, int>();

        for (int i = 0; i < roomList.Count; i++)
        {
            foreach (KeyValuePair<Vector2Int, int> item in roomList[i].MoneyDic)
            {
                moneyDic.Add(item.Key, item.Value);
                spaceList.Remove(item.Key);
            }
        }

        //explore point
        Dictionary<Vector2Int, Event> exploreEventDic = new Dictionary<Vector2Int, Event>();
        
        //賽錢箱會有一個
        position = spaceList[Random.Range(0, spaceList.Count)];
        exploreEventDic.Add(position, new RecoverEvent1());
        spaceList.Remove(position);

        //傳送點有 房間數量/2 個
        for (int i = 0; i < data.RoomAmount / 2; i++)
        {
            position = spaceList[Random.Range(0, spaceList.Count)];
            exploreEventDic.Add(position, new TeleportEvent1());
            spaceList.Remove(position);
        }

        //起點
        Vector2Int start = new Vector2Int();
        Room startRoom = roomList[0];
        tempList = new List<Vector2Int>(startRoom.PositionList);
        for (int i = 0; i < tempList.Count; i++)
        {
            position = tempList[Random.Range(0, tempList.Count)];
            if (spaceList.Contains(position)) //是空地
            {
                start = position;
                break;
            }
            tempList.Remove(position);
            i--;
        }

        //終點
        Vector2Int goal = new Vector2Int();
        Room goalRoom = roomList[1];
        for (int i = 2; i < roomList.Count; i++) //尋找距離起始房間距離最遠的房間
        {
            if (Vector2Int.Distance(startRoom.Position, goalRoom.Position) < Vector2Int.Distance(startRoom.Position, roomList[i].Position))
            {
                goalRoom = roomList[i];
            }
        }

        tempList = new List<Vector2Int>(goalRoom.PositionList);
        for (int i = 0; i < tempList.Count; i++)
        {
            position = tempList[Random.Range(0, tempList.Count)];
            if (spaceList.Contains(position)) //是空地
            {
                goal = position;
                break;
            }
            tempList.Remove(position);
            i--;
        }

        MapInfo mapInfo = new MapInfo();
        mapInfo.ID = data.ID;
        mapInfo.Group = data.Group;
        mapInfo.LastFloor = data.LastFloor;
        mapInfo.NextFloor = data.NextFloor;
        mapInfo.MapBound = Utility.GetMapBounds(_mapList);
        mapInfo.Start = start;
        mapInfo.Goal = goal;
        mapInfo.MapList = _mapList;
        mapInfo.GrassList = grassList;
        mapInfo.KeyList = keyList;
        mapInfo.MoneyDic = moneyDic;
        mapInfo.ExploreEventDic = exploreEventDic;
        mapInfo.TreasureDic = treasureDic;
        mapInfo.DoorList = doorList;

        for (int i=0; i<roomList.Count; i++)
        {
            mapInfo.RoomPositionList.Add(roomList[i].PositionList);
        }

        ExploreController.Instance.SetMapInfo(mapInfo);
        DungeonPainter.Instance.Paint(mapInfo);
    }

    private void AddWallDirectionList(Room room)
    {
        for (int i = 0; i < room.WallDirectionList.Count; i++)
        {
            _wallDirectionDic.Add(new KeyValuePair<Vector2Int, Room>(room.WallDirectionList[i], room));
        }
    }

    private void SetRoom(Room room) //在地圖上畫出房間
    {
        for (int i = 0; i < room.PositionList.Count; i++)
        {
            _mapList.Add(room.PositionList[i]);

            if (room is NormalRoom || room is MazeRoom)
            {
                _normalRoomMapList.Add(room.PositionList[i]);
            }
            else if (room is TreasureRoom)
            {
                _treasureRoomMapList.Add(room.PositionList[i]);
            }
        }
    }

    private bool ScanSpace(Vector2Int scanPosition, int width, int height) //掃描空間
    {
        Vector2Int tilePosition = new Vector2Int();

        for (int i = scanPosition.x; i < scanPosition.x + width; i++)
        {
            for (int j = scanPosition.y; j < scanPosition.y + height; j++)
            {
                tilePosition.x = i;
                tilePosition.y = j;
                if (_mapList.Contains(tilePosition) || _mapList.Contains(tilePosition + Vector2Int.left) || _mapList.Contains(tilePosition + Vector2Int.right) || _mapList.Contains(tilePosition + Vector2Int.up) || _mapList.Contains(tilePosition + Vector2Int.down)) //這個空間裡已經有其他東西了
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void GetRoomPosition(Vector2Int direction, Room lastRoom, Room room, out Vector2Int roomPostion)
    {
        int pathLength = Random.Range(5, 11);
        int offset = Random.Range(5, 11);
        roomPostion = new Vector2Int();

        if (direction == Vector2Int.right) //東
        {
            roomPostion = new Vector2Int(lastRoom.Position.x + lastRoom.Width + pathLength, lastRoom.Position.y + offset);
        }
        else if (direction == Vector2Int.left) //西
        {
            roomPostion = new Vector2Int(lastRoom.Position.x - pathLength - room.Width, lastRoom.Position.y + offset);
        }
        else if (direction == Vector2Int.down) //南
        {
            roomPostion = new Vector2Int(lastRoom.Position.x + offset, lastRoom.Position.y - pathLength - room.Height);
        }
        else if (direction == Vector2Int.up) //北
        {
            roomPostion = new Vector2Int(lastRoom.Position.x + offset, lastRoom.Position.y + lastRoom.Height + pathLength);
        }
    }

    private bool GeneratePath(Vector2Int fromDoor, Vector2Int toDoor, List<Vector2Int> pathFinderList)
    {
        List<Vector2Int> pathList = AStarAlgor.Instance.GetPath(fromDoor, toDoor, pathFinderList, true);
        if (pathList != null)
        {
            pathList.Add(fromDoor);
            pathList.Add(toDoor);
            SetPath(pathList);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetPath(List<Vector2Int> list)
    {
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!_mapList.Contains(list[i]))
                {
                    _mapList.Add(list[i]);
                }
            }
        }
    }

    private bool CheckLoadingCount()
    {
        _loadingCount++;
        if (_loadingCount == 1000)
        {
            _loadingCount = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
}
