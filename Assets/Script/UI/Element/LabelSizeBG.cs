using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelSizeBG : MonoBehaviour //根據文字內容多寡來調整背景的大小
{
    public RectTransform Label;
    public RectTransform BG;
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    // Start is called before the first frame update
    void Start()
    {
        BG.sizeDelta = Label.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
