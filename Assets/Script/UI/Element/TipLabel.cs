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

    public void SetLabel(string text)
    {
        Label.text = text;
        CanvasGroup.alpha = 1;
        CanvasGroup.DORestart();
        _tweener = CanvasGroup.DOFade(0, 2f).SetEase(Ease.InExpo);
        _tweener.SetUpdate(true);
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
