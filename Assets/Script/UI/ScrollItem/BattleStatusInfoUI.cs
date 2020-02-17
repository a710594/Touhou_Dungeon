using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleStatusInfoUI : MonoBehaviour
{
    public LoopScrollView ScrollView;
    public ButtonPlus CloseButton;

    private bool _isScrollViewShow = false;
    private bool _isClickable = true;
    private Action _closeCallback;
    private CanvasGroup _scrollViewCanvasGroup;

    public void Open(List<BattleStatus> statusList, Action openCallback, Action closeCallback)
    {
        if (_isClickable && !_isScrollViewShow)
        {
            _isClickable = false;
            _isScrollViewShow = true;

            //GetComponent<RectTransform>().SetAsLastSibling();
            ScrollView.SetData(new ArrayList(statusList));
            _scrollViewCanvasGroup.DOFade(1, 0.5f).OnComplete(() =>
            {
                _isClickable = true;
                _scrollViewCanvasGroup.blocksRaycasts = true;
            });

            if (openCallback != null)
            {
                openCallback();
            }
            _closeCallback = closeCallback;
        }
    }

    private void Close(object data)
    {
        if (_isClickable && _isScrollViewShow)
        {
            _isClickable = false;
            _isScrollViewShow = false;

            _scrollViewCanvasGroup.blocksRaycasts = false;
            _scrollViewCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                _isClickable = true;
            });
        }
    }

    private void Awake()
    {
        _scrollViewCanvasGroup = ScrollView.GetComponent<CanvasGroup>();
        _scrollViewCanvasGroup.alpha = 0;
        _scrollViewCanvasGroup.blocksRaycasts = false;

        CloseButton.ClickHandler = Close;
    }
}
