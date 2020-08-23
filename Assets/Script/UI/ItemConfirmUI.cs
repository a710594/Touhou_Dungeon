using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemConfirmUI : MonoBehaviour
{
    public Button ConfirmButton;
    public LoopScrollView ScrollView;

    private Action _onFinishHandler; 

    private static ItemConfirmUI _instance;

    public static void Open(List<int> idList, Action callback = null)
    {
        if (_instance == null)
        {
            _instance = ResourceManager.Instance.Spawn("ItemConfirmUI", ResourceManager.Type.UI).GetComponent<ItemConfirmUI>();
        }
        ExploreController.Instance.StopEnemy();
        _instance._onFinishHandler = callback;
        _instance.Init(idList);
    }

    private void Init(List<int> idList)
    {
        List<KeyValuePair<int, int>> itemPair = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < idList.Count; i++)
        {
            itemPair.Add(new KeyValuePair<int, int>(idList[i], 1));
        }
        ScrollView.SetData(new ArrayList(itemPair));
    }

    private void Close()
    {
        ExploreController.Instance.ContinueEnemy();
        Destroy(_instance.gameObject);
        _instance = null;
    }

    private void ConfirmOnClick()
    {
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
