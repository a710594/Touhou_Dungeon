using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconConfirmUI : MonoBehaviour
{
    public Button ConfirmButton;
    public LoopScrollView ScrollView;

    private Action _onFinishHandler; 

    private static IconConfirmUI _instance;

    public static void Open(List<int> idList, Action callback = null)
    {
        if (_instance == null)
        {
            _instance = ResourceManager.Instance.Spawn("IconConfirmUI", ResourceManager.Type.UI).GetComponent<IconConfirmUI>();
        }
        Time.timeScale = 0;
        _instance._onFinishHandler = callback;
        _instance.Init(idList);
    }

    private void Init(List<int> idList)
    {
        List<KeyValuePair<object, int>> itemPair = new List<KeyValuePair<object, int>>();
        for (int i = 0; i < idList.Count; i++)
        {
            itemPair.Add(new KeyValuePair<object, int>(idList[i], 0)); //數量設為0是為了不讓 scrollview 顯示數量.實際數量都是1,沒有顯示的必要
        }
        ScrollView.SetData(new ArrayList(itemPair));
    }

    private void Close()
    {
        Destroy(_instance.gameObject);
        _instance = null;
    }

    private void ConfirmOnClick()
    {
        Time.timeScale = 1;
        _instance.Close();
        if (_onFinishHandler != null)
        {
            _onFinishHandler();
            _onFinishHandler = null;
        }
    }

    void Awake()
    {
        ConfirmButton.onClick.AddListener(ConfirmOnClick);
    }
}
