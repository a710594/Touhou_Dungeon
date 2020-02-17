using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TipLabel : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public Text Label;

    public void SetLabel(string text)
    {
        Label.text = text;
        CanvasGroup.alpha = 1;
        CanvasGroup.DORestart();
        CanvasGroup.DOFade(0, 2f).SetEase(Ease.InExpo);
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

    void Start()
    {
        CanvasGroup.alpha = 0;
    }

}
