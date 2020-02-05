using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Enemy
{
    public class Data
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int HP { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MTK { get; set; }
        public int MEF { get; set; }
        public int AGI { get; set; }
        public int SEN { get; set; }
        public int MoveDistance { get; set; }
        public string AI { get; set; }
        public string Comment { get; set; }
        public int Skill_1 { get; set; }
        public int Skill_2 { get; set; }
        public int Item_1 { get; set; }
        public int Probability_1 { get; set; }
        public int Item_2 { get; set; }
        public int Probability_2 { get; set; }
        public int ItemAmount { get; set; }
        public int Exp { get; set; }

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

    private static Dictionary<int, Data> _dataDic = new Dictionary<int, Data>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Enemy");
        string jsonString = textAsset.text;
        var dataList = JsonConvert.DeserializeObject<List<Data>>(jsonString);

        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].Skill_1 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_1);

            if (dataList[i].Skill_2 != 0)
                dataList[i].SkillList.Add(dataList[i].Skill_2);

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

            _dataDic.Add(dataList[i].ID, dataList[i]);
        }
    }

    public static Data GetData(int id)
    {
        Data data = null;
        _dataDic.TryGetValue(id, out data);
        return data;
    }
}
