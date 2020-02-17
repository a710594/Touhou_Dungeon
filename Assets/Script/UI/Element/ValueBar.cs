using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ValueBar : MonoBehaviour
{
    [SerializeField]
    private Image _bar;
    [SerializeField]
    private Text _label;

    private bool _isTweening = false;
    private int _maxHP;
    private Timer _timer = new Timer();

    public void SetValue(int current, int max)
    {
        _bar.fillAmount = (float)current / (float)max;
        _label.text = current.ToString() + "/" + max.ToString();
    }

    public void SetValueTween(int current, int max, Action callback)
    {
        _isTweening = true;
        _maxHP = max;
        _bar.DOFillAmount((float)current / (float)max, 0.5f).OnComplete(()=> 
        {
            _timer.Start(0.5f, callback);
            _isTweening = false;
        });

    }

    public void SetValueTween(int from, int to, int max, Action callback)
    {
        _isTweening = true;
        _maxHP = max;
        _bar.fillAmount = (float)from / (float)max;
        _bar.DOFillAmount((float)to / (float)max, 0.5f).OnComplete(() =>
        {
            _timer.Start(0.5f, callback);
            _isTweening = false;
        });

    }

    void Update()
    {
        if (_isTweening)
        {
            _label.text = Mathf.RoundToInt(_maxHP * _bar.fillAmount).ToString() + "/" + _maxHP.ToString();
        }
    }

    private void OnDestroy()
    {
        _timer.Stop();
    }
}
