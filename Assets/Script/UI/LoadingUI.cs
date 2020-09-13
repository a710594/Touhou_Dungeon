using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingUI : MonoBehaviour
{
    public static LoadingUI Instance;

    private static bool _exists;

    public Image Image;
    public Text Label;

    private int _count = 0;
    private float _startTime = -1;

    public void Open(Action callback)
    {
        StartCoroutine(Loading(callback));
        _startTime = Time.time;
    }

    public void Close()
    {
        Image.gameObject.SetActive(false);
        _startTime = -1;
    }

    public IEnumerator Loading(Action callback)
    {
        Image.gameObject.SetActive(true);

        yield return null;
        callback();

        //Image.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_startTime != -1 && (Time.time - _startTime) > 0.1f)
        {
            string text = "Loading.";
            for (int i=0; i<_count; i++)
            {
                text += ".";
            }
            Label.text = text;
            _count = (_count + 1) % 3;
            _startTime = Time.time;
        }
    }

    void Awake()
    {
        if (!_exists)
        {
            _exists = true;
            Instance = this; //temp
            DontDestroyOnLoad(transform.gameObject);//使物件切換場景時不消失
        }
        else
        {
            Destroy(gameObject); //破壞場景切換後重複產生的物件
            return;
        }
    }
}
