using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class RoomData
{
    public enum TypeEnum
    {
        Normal,
        Maze,
        Treasure,
    }

    public class RootObject
    {
        public int ID { get; set; }
        public TypeEnum Type { get; set; }
        public int MinWidth { get; set; }
        public int MaxWidth { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public int MinTreasureAmount { get; set; }
        public int MaxTreasureAmount { get; set; }
        public int Treasure_1 { get; set; }
        public int Probability_1 { get; set; }
        public int Treasure_2 { get; set; }
        public int Probability_2 { get; set; }
        public int Treasure_3 { get; set; }
        public int Probability_3 { get; set; }
        public int MinMoney { get; set; }
        public int MaxMoney { get; set; }
        public int MinMoneyAmount { get; set; }
        public int MaxMoneyAmount { get; set; }

        public List<int> TreasureList = new List<int>();

        public int GetRandomTreasureID()
        {
            return TreasureList[UnityEngine.Random.Range(0, TreasureList.Count)];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Room");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            for (int j=0; j<dataList[i].Probability_1; j++)
            {
                dataList[i].TreasureList.Add(dataList[i].Treasure_1);
            }
            for (int j = 0; j < dataList[i].Probability_2; j++)
            {
                dataList[i].TreasureList.Add(dataList[i].Treasure_2);
            }
            for (int j = 0; j < dataList[i].Probability_3; j++)
            {
                dataList[i].TreasureList.Add(dataList[i].Treasure_3);
            }

            _dataDic.Add(dataList[i].ID, dataList[i]);
        }
    }

    public static RootObject GetData(int id)
    {
        RootObject data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }

    public static Room GetRoomClassByID(int id)
    {
        if (_dataDic.ContainsKey(id))
        {
            if (_dataDic[id].Type == TypeEnum.Normal)
            {
                return new NormalRoom();
            }
            else if (_dataDic[id].Type == TypeEnum.Maze)
            {
                return new MazeRoom();
            }
            else if (_dataDic[id].Type == TypeEnum.Treasure)
            {
                return new TreasureRoom();
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
