using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TipLabel : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public Text Label;

    private Tweener _tweener;
    private Timer _timer = new Timer();

    public void SetLabel(string text, bool isTween = true, Action callback = null)
    {
        Label.text = text;
        CanvasGroup.alpha = 1;
        if (isTween)
        {
            CanvasGroup.DORestart();
            _tweener = CanvasGroup.DOFade(0, 2f).SetEase(Ease.InExpo).OnComplete(()=> 
            {
                if (callback != null)
                {
                    callback();
                }
            });
            _tweener.SetUpdate(true);
        }
        else
        {
            _timer.Start(2, ()=>
            {
                CanvasGroup.alpha = 0;
                if (callback != null)
                {
                    callback();
                }
            });
        }
    }

    public void SetVisible(bool isVisible)
    {
        CanvasGroup.DOKill();
        if (isVisible)
        {
            CanvasGroup.alpha = 1;
        }
        else
        {
            CanvasGroup.alpha = 0;
        }
    }

    public void Stop()
    {
        CanvasGroup.DOKill();
    }

    void Awake()
    {
        CanvasGroup.alpha = 0;
    }

}
