﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EquipData
{
    public enum TypeEnum
    {
        Weapon,
        Helmet,
        Armor,
        Jewelry,
    }

    public class RootObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MTK { get; set; }
        public int MEF { get; set; }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Equip");
        string jsonString = textAsset.text;
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
}