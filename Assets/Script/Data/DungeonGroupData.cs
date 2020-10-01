using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DungeonGroupData
{
    public class RootObject
    {
        public int ID;
        public string Name;
        public int UnlockFloor;
        public string GroundTile;
        public string WallTile;
        public string DoorTile;
        public string GrassTile;
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/DungeonGroup.json";
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
            _dataDic.Add(dataList[i].ID, dataList[i]);
        }
    }

    public static RootObject GetData(int id)
    {
        RootObject data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }

    public static List<RootObject> GetGroupList(int arriveFloor) 
    {
        List<RootObject> lsit = new List<RootObject>();
        foreach (KeyValuePair<int, RootObject> item in _dataDic)
        {
            if (item.Value.UnlockFloor <= arriveFloor)
            {
                lsit.Add(item.Value);
            }
        }
        return lsit;
    }
}
