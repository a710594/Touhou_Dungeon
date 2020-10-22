using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

public class NewCookData : MonoBehaviour
{
    public class RootObject
    {
        public int ID;
        public int ResultID;
        public int AddHP;
        public int AddMP;
        public int Material_1;
        public int Material_2;
        public int Material_3;
        public int Material_4;
        public int Material_5;
        public int Priority;

        public List<int> MaterialList = new List<int>();
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/NewCook.json";
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
            if (dataList[i].Material_1 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_1);
            }
            if (dataList[i].Material_2 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_2);
            }
            if (dataList[i].Material_3 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_3);
            }
            if (dataList[i].Material_4 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_4);
            }
            if (dataList[i].Material_5 != 0)
            {
                dataList[i].MaterialList.Add(dataList[i].Material_5);
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

    public static Food GetFood(List<Item> materialList) 
    {
        bool IsContain = true;
        Food food = null;
        List<int> idList = new List<int>();
        List<RootObject> resultList = new List<RootObject>();

        for (int i=0; i<materialList.Count; i++) 
        {
            idList.Add(materialList[i].ID);
        }

        foreach (KeyValuePair<int, RootObject> item in _dataDic) 
        {
            IsContain = true;
            for (int i=0; i<item.Value.MaterialList.Count; i++) 
            {
                if (!idList.Contains(item.Value.MaterialList[i]))
                {
                    IsContain = false;
                    continue;
                }
            }
            if (IsContain)
            {
                resultList.Add(item.Value);
            }
        }

        if (resultList.Count > 0)
        {
            resultList.Sort((x, y) =>
            {
                return (y.Priority).CompareTo(x.Priority);
            });

            RootObject result = resultList[0];
            int addHP = result.AddHP;
            int addMP = result.AddMP;
            ItemEffectData.RootObject itemEffectData;
            for (int i = 0; i < materialList.Count; i++)
            {
                itemEffectData = ItemEffectData.GetData(idList[i]);
                if (itemEffectData != null)
                {
                    addHP += itemEffectData.AddHP;
                    addMP += itemEffectData.AddMP;
                }
            }
            food = new Food(result.ResultID, 1, addHP, addMP);
        }
        return food;
    }
}
