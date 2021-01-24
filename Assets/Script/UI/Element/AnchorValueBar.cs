using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnchorValueBar : ValueBar
{
    public Image SubBar;
    public Text PredictionLabel;
    public GameObject[] HPQueue;

    private Transform _anchor;
    private Color _originalColor;

    public void SetAnchor(Transform anchor)
    {
        _anchor = anchor;
    }

    public void SetHPQueue(int count)
    {
        for (int i=0; i<HPQueue.Length; i++)
        {
            HPQueue[i].SetActive(i < count);
        }
    }

    public int GetHPQueueAmount()
    {
        int amount = 0;
        for (int i = 0; i < HPQueue.Length; i++)
        {
            if (HPQueue[i].activeSelf)
            {
                amount++;
            }
        }
        return amount;
    }

    public void SetPrediction(int origin, int prediction, int max) //預覽傷害後的血量
    {
        Bar.DOColor(Color.clear, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        SubBar.fillAmount = (float)prediction / (float)max;
        PredictionLabel.text = origin + "→" + prediction;
    }

    public void StopPrediction()
    {
        Bar.DOKill();
        Bar.color = _originalColor;
        SubBar.fillAmount = 0;
        PredictionLabel.text = string.Empty;
    }

    protected override void UpdateData()
    {
        base.UpdateData();
        if (_anchor != null)
        {
            this.transform.position = Camera.main.WorldToScreenPoint(_anchor.position);
        }
    }

    private void Awake()
    {
        _originalColor = Bar.color;
    }
}
