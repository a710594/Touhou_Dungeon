using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ConversationData
{
    public enum MotionEnum 
    {
        Jump = 1,
        Shake,
    }
    public class RootObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Comment_Chinese { get; set; }
        public string Background { get; set; }
        public string BGM { get; set; }
        public string Image_1 { get; set; }
        public MotionEnum Motion_1 { get; set; }
        public string Image_2 { get; set; }
        public MotionEnum Motion_2 { get; set; }
        //public int NextID { get; set; }

        public string[] Images = new string[2];
        public MotionEnum[] Motions = new MotionEnum[2];

        public Dictionary<LanguageSystem.Language, string> CommentDic = new Dictionary<LanguageSystem.Language, string>();

        public string GetComment()
        {
            return CommentDic[LanguageSystem.Instance.CurrentLanguage];
        }
    }

    private static Dictionary<int, RootObject> _dataDic = new Dictionary<int, RootObject>();

    public static void Load()
    {
        string path = Application.streamingAssetsPath + "/Conversation.json";
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
            dataList[i].Images[0] = dataList[i].Image_1;
            dataList[i].Images[1] = dataList[i].Image_2;

            dataList[i].Motions[0] = dataList[i].Motion_1;
            dataList[i].Motions[1] = dataList[i].Motion_2;

            dataList[i].CommentDic.Add(LanguageSystem.Language.Chinese, dataList[i].Comment_Chinese);

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
