using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleFrontUI : MonoBehaviour
{
    public static BattleFrontUI Instance;

    public GameObject Mask;
    public GameObject SpellCardGroup;
    public Text NameLabel;
    public Image Image;

    private Timer _timer = new Timer();

    public void ShowSpellCard(string name, string image, Action callback)
    {
        if (image != string.Empty)
        {
            Image.overrideSprite = Resources.Load<Sprite>("Image/Character/Large/" + image);
        }
        else
        {
            Image.overrideSprite = Resources.Load<Sprite>("Image/Character/Medium/Large_M");
        }
        Image.SetNativeSize();

        Mask.SetActive(true);
        SpellCardGroup.SetActive(true);
        NameLabel.gameObject.SetActive(true);
        NameLabel.text = name;
        SpellCardGroup.transform.localPosition = Vector3.right * 1280;
        SpellCardGroup.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            _timer.Start(1f, () =>
            {
                SpellCardGroup.transform.DOLocalMoveX(-1280, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    Mask.SetActive(false);
                    NameLabel.gameObject.SetActive(false);
                    SpellCardGroup.SetActive(false);
                    if (callback != null)
                    {
                        callback();
                    }
                });
            });
        });
    }

    private void Awake()
    {
        Instance = this;
    }
}
