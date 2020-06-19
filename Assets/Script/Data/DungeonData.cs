using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DungeonData
{
    public class RootObject
    {
        public int ID { get; set; }
        public int Floor { get; set; }
        public int LastFloor { get; set; }
        public int NextFloor { get; set; }
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
        public int MinEventPoint { get; set; }
        public int MaxEventPoint { get; set; }
        public int Event_1 { get; set; }
        public int Event_2 { get; set; }
        public int Event_3 { get; set; }

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
        string path = Application.streamingAssetsPath + "/Dungeon.json";
        string jsonString;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        jsonString = File.ReadAllText(path);
#elif UNITY_ANDROID
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
        www.SendWebRequest();
        while (!www.isDone)
        {
        }
        jsonString = www.downloadHandler.text;
#endif
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].Room_1 != 0)
            {
                for (int j = 0; j < dataList[i].Probability_1; j++)
                {
                    dataList[i].RoomList.Add(dataList[i].Room_1);
                }
            }
            if (dataList[i].Room_2 != 0)
            {
                for (int j = 0; j < dataList[i].Probability_2; j++)
                {
                    dataList[i].RoomList.Add(dataList[i].Room_2);
                }
            }
            if (dataList[i].Room_3 != 0)
            {
                for (int j = 0; j < dataList[i].Probability_3; j++)
                {
                    dataList[i].RoomList.Add(dataList[i].Room_3);
                }
            }

            dataList[i].EventList.Add(dataList[i].Event_1);
            dataList[i].EventList.Add(dataList[i].Event_2);
            dataList[i].EventList.Add(dataList[i].Event_3);

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
