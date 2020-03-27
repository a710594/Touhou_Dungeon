﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DungeonData
{
    public class RootObject
    {
        public int ID { get; set; }
        public int Floor { get; set; }
        public int RoomAmount { get; set; }
        public int Room_1 { get; set; }
        public int Probability_1 { get; set; }
        public int Room_2 { get; set; }
        public int Probability_2 { get; set; }
        public int Room_3 { get; set; }
        public int Probability_3 { get; set; }
        public string GroundTile { get; set; }
        public string WallTile { get; set; }
        public string DoorTile { get; set; }
        public string GrassTile { get; set; }
        public int MinExplorePoint { get; set; }
        public int MaxExplorePoint { get; set; }
        public int Event_1 { get; set; }
        public int Event_2 { get; set; }

        public List<int> RoomList = new List<int>();
        public List<int> EventList = new List<int>();

        public int GetRandomRoomID()
        {
            return RoomList[UnityEngine.Random.Range(0, RoomList.Count)];
        }

        public int GetRandomEventID()
        {
            return EventList[UnityEngine.Random.Range(0, EventList.Count)];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Dungeon");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            dataList[i].RoomList.Add(dataList[i].Room_1);
            dataList[i].RoomList.Add(dataList[i].Room_2);
            dataList[i].RoomList.Add(dataList[i].Room_3);

            dataList[i].EventList.Add(dataList[i].Event_1);
            dataList[i].EventList.Add(dataList[i].Event_2);

            _dataDic.Add(dataList[i].ID, dataList[i]);
        }
    }

    public static RootObject GetData(int id)
    {
        RootObject data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }
}
