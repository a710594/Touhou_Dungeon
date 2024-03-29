﻿using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingNumber : MonoBehaviour
{
    public enum Type
    {
        Damage,
        Recover,
        Poison,
        Miss,
        Critical,
        Paralysis,
        Sleeping,
        Confusion,
        Other,
    }

    public Text Label;
    public float Height;
    public float Duration;

    public void SetValue(float height, float duration)
    {
        Height = height;
        Duration = duration;
    }

    public void Play(string text, Type type, Vector2 position)
    {
        //if (Encoding.Default.GetByteCount(text) > 10)
        //{
        //    Label.fontSize = 30;
        //}
        //else
        //{
        //    Label.fontSize = 80;
        //}

        if (int.TryParse(text, out int n))
        {
            Label.fontSize = 80;
        }
        else
        {
            Label.fontSize = 30;
        }

        this.transform.position = position;
        Label.text = text;

        if (type == Type.Damage)
        {
            Label.color = Color.red;
        }
        else if (type == Type.Poison)
        {
            Label.color = new Color32(180, 0, 180, 255);
        }
        else if (type == Type.Recover)
        {
            Label.color = Color.green;
        }
        else if (type == Type.Miss)
        {
            Label.color = Color.blue;
        }
        else if (type == Type.Critical)
        {
            Label.color = Color.yellow;
        }
        else if (type == Type.Paralysis)
        {
            Label.color = new Color32(180, 140, 0, 255);
        }
        else if (type == Type.Sleeping)
        {
            Label.color = new Color32(0, 150, 200, 255);
        }
        else if (type == Type.Confusion)
        {
            Label.color = new Color32(100, 0, 200, 255);
        }
        else if (type == Type.Other)
        {
            Label.color = Color.black;
        }

        this.transform.DOMoveY(position.y + Height, Duration).SetEase(Ease.OutCubic).OnComplete(()=> 
        {
        });

        Label.DOFade(0, Duration).SetEase(Ease.InCubic).OnComplete(()=> 
        {
        });
    }

    void Awake()
    {
        Label.color = Color.clear;
    }
}
