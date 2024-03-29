﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EnemyData
{
    public class RootObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string LargeImage { get; set; }
        public string Animator { get; set; }
        public int HP_1 { get; set; }
        public int HP_2 { get; set; }
        public int HP_3 { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MTK { get; set; }
        public int MEF { get; set; }
        public int AGI { get; set; }
        public int SEN { get; set; }
        public int Equip_ATK { get; set; }
        public int Equip_DEF { get; set; }
        public int Equip_MTK { get; set; }
        public int Equip_MEF { get; set; }
        public int MOV { get; set; }
        public string AI { get; set; }
        public string Comment { get; set; }
        public int Skill_1 { get; set; }
        public int Skill_2 { get; set; }
        public int Skill_3 { get; set; }
        public int Item_1 { get; set; }
        public int Probability_1 { get; set; }
        public int Item_2 { get; set; }
        public int Probability_2 { get; set; }
        public int Item_3 { get; set; }
        public int Probability_3 { get; set; }
        public int ItemAmount { get; set; }

        public List<int> HPList = new List<int>();
        public List<int> SkillList = new List<int>();
        public List<int> ItemList = new List<int>();

        public List<int> GetDropItemList()
        {
            int itemId;
            List<int> dropList = new List<int>();
            for (int i = 0; i < ItemAmount; i++)
            {
                itemId = ItemList[Random.Range(0, ItemList.Count)];
                dropList.Add(itemId);
            }

            return dropList;
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/Enemy.json";
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
            if (dataList[i].HP_1 != 0)
                dataList[i].HPList.Add(dataList[i].HP_1);

            if (dataList[i].HP_2 != 0)
                dataList[i].HPList.Add(dataList[i].HP_2);

            if (dataList[i].HP_3 != 0)
                dataList[i].HPList.Add(dataList[i].HP_3);

            if (dataList[i].Skill_1 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_1);

            if (dataList[i].Skill_2 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_2);

            if (dataList[i].Skill_3 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_3);

            if (dataList[i].Item_1 != 0)
            {
                for (int j=0; j<dataList[i].Probability_1; j++)
                {
                    dataList[i].ItemList.Add(dataList[i].Item_1);
                }
            }

            if (dataList[i].Item_2 != 0)
            {
                for (int j = 0; j < dataList[i].Probability_2; j++)
                {
                    dataList[i].ItemList.Add(dataList[i].Item_2);
                }
            }

            if (dataList[i].Item_3 != 0)
            {
                for (int j = 0; j < dataList[i].Probability_3; j++)
                {
                    dataList[i].ItemList.Add(dataList[i].Item_3);
                }
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
}
