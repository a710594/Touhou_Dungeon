using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ShopData
{
    public enum TypeEnum
    {
        Item = 1, 
        Equip,
    }

    public class RootObject
    {
        public int ID { get; set; }
        public TypeEnum Type { get; set; }
        public int UnlockFloor { get; set; }
    }
    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();
    private static Dictionary<TypeEnum, List<RootObject>> _typeIdDic = new Dictionary<TypeEnum, List<RootObject>>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/Shop.json";
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
        _typeIdDic.Add(TypeEnum.Item, new List<RootObject>());
        _typeIdDic.Add(TypeEnum.Equip, new List<RootObject>());

        for (int i = 0; i < dataList.Count; i++)
        {
            _dataDic.Add(dataList[i].ID, dataList[i]);
            _typeIdDic[dataList[i].Type].Add(dataList[i]);
        }
    }

    public static RootObject GetData(int id)
    {
        RootObject data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }

    public static List<int> GetIDList(TypeEnum type)
    {
        List<int> list = new List<int>();
        for (int i=0; i<_typeIdDic[type].Count; i++) 
        {
            if (ExploreController.Instance.ArriveFloor >= _typeIdDic[type][i].UnlockFloor)
            {
                list.Add(_typeIdDic[type][i].ID);
            }
        }

        return list;
    }
}
