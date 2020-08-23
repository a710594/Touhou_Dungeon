using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingUI : MonoBehaviour
{
    private static bool _exists;

    public Image Image;

    public static LoadingUI Instance;

    public void Open(Action callback) 
    {
        StartCoroutine(Loading(callback));
    }

    public IEnumerator Loading(Action callback)
    {
        Image.gameObject.SetActive(true);

        yield return null;
        callback();

        Image.gameObject.SetActive(false);
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
