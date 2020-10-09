using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DungeonData
{
    public class RootObject
    {
        public int ID { get; set; }
        public int Group { get; set; }
        public string FloorName { get; set; }
        public int LastFloor { get; set; }
        public int NextFloor { get; set; }
        public int SceneIndex { get; set; }
        public int RoomAmount { get; set; }
        public int Room_1 { get; set; }
        public int Probability_1 { get; set; }
        public int Room_2 { get; set; }
        public int Probability_2 { get; set; }
        public int Room_3 { get; set; }
        public int Probability_3 { get; set; }

        public int BattleGroup_1 { get; set; }
        public int BattleGroupProbability_1 { get; set; }
        public int BattleGroup_2 { get; set; }
        public int BattleGroupProbability_2 { get; set; }
        public int BattleGroup_3 { get; set; }
        public int BattleGroupProbability_3 { get; set; }
        public int GoalBattleGroup { get; set; }

        public List<int> RoomList = new List<int>();
        public int GetRandomRoomID()
        {
            return RoomList[UnityEngine.Random.Range(0, RoomList.Count)];
        }

        public List<int> BattleGroupList = new List<int>();
        public int GetRandomBattleGroup()
        {
            return BattleGroupList[Random.Range(0, BattleGroupList.Count)];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();
    private static Dictionary<int, List<RootObject>> _floorDic = new Dictionary<int, List<RootObject>>();

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

            if (dataList[i].BattleGroup_1 != 0)
            {
                for (int j = 0; j < dataList[i].BattleGroupProbability_1; j++)
                {
                    dataList[i].BattleGroupList.Add(dataList[i].BattleGroup_1);
                }
            }
            if (dataList[i].BattleGroup_2 != 0)
            {
                for (int j = 0; j < dataList[i].BattleGroupProbability_2; j++)
                {
                    dataList[i].BattleGroupList.Add(dataList[i].BattleGroup_2);
                }
            }
            if (dataList[i].BattleGroup_3 != 0)
            {
                for (int j = 0; j < dataList[i].BattleGroupProbability_3; j++)
                {
                    dataList[i].BattleGroupList.Add(dataList[i].BattleGroup_3);
                }
            }

            _dataDic.Add(dataList[i].ID, dataList[i]);


            if (!_floorDic.ContainsKey(dataList[i].Group))
            {
                _floorDic.Add(dataList[i].Group, new List<RootObject>());
            }
            _floorDic[dataList[i].Group].Add(dataList[i]);
        }
    }

    public static RootObject GetData(int id)
    {
        RootObject data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }

    public static List<RootObject> GetFloorList(int group, int arriveFloor)
    {
        List<RootObject> floorList = null;
        _floorDic.TryGetValue(group, out floorList);
        for (int i=0; i<floorList.Count; i++) 
        {
            if (floorList[i].ID > arriveFloor)
            {
                floorList.RemoveAt(i);
                i--;
            }
        }
        return floorList;
    }
}
