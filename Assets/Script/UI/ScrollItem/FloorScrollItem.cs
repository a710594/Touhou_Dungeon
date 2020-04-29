﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorScrollItem : ScrollItem
{
    public Text Label;

    public override void SetData(object obj)
    {
        base.SetData(obj);
        int floor = (int)_data;
        Label.text = floor + "樓";
    }
}