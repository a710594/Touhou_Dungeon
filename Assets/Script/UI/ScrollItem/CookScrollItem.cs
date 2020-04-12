using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookScrollItem : ScrollItem
{
    public Text NameLabel;

    public override void SetData(object obj)
    {
        base.SetData(obj);
        CookData.RootObject cookData = (CookData.RootObject)obj;
        ItemData.RootObject itemData = ItemData.GetData(cookData.ResultID);
        NameLabel.text = itemData.Name;
    }
}
