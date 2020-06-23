using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeSceneUI : MonoBehaviour
{
    private static bool _exists;

    public static ChangeSceneUI Instance;

    public Image ClockImage;

    public void StartClock(Action callback = null)
    {
        ClockImage.DOFillAmount(1, 0.5f).OnComplete(()=> 
        {
            if (callback != null)
            {
                callback();
            }
        });
    }

    public void EndClock(Action callback = null)
    {
        ClockImage.fillAmount = 1;
        ClockImage.DOFillAmount(0, 0.5f).OnComplete(() =>
        {
            if (callback != null)
            {
                callback();
            }
        });
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
