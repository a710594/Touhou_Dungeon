using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class Caretaker
{
    private string _prePath;

    private static Caretaker _instance;
    public static Caretaker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Caretaker();
            }
            return _instance;
        }
    }

    public void Init()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        _prePath = Path.Combine(Application.dataPath, "Resources/SaveData/StoryMode/");
#elif UNITY_ANDROID || UNITY_IOS
        _prePath = Application.persistentDataPath;
        _prePath = Path.Combine(Application.persistentDataPath, "SaveData/StoryMode/");
#endif

        if (!Directory.Exists(_prePath))
        {
            Directory.CreateDirectory(_prePath);
        }
    }

    public T Load<T>()
    {
        try
        {
            string path = Path.Combine(_prePath, typeof(T).Name + ".json");
            string jsonString = File.ReadAllText(path);
            T info = JsonConvert.DeserializeObject<T>(jsonString);
            return info;
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return default(T);
        }
    }

    public void Save<T>(T t)
    {
        string path = Path.Combine(_prePath, typeof(T).Name + ".json");
        File.WriteAllText(path, JsonConvert.SerializeObject(t));
    }

    public void ClearData<T>()
    {
        string path = Path.Combine(_prePath, typeof(T).Name + ".json");
        File.WriteAllText(path, String.Empty);
        File.Create(path).Close();
    }
}