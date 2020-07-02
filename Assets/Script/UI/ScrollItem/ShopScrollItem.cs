using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScrollItem : ScrollItem
{
    public IconCard Card;

    public override void SetData(object obj)
    {
        KeyValuePair<object, int> pair = (KeyValuePair<object, int>)obj; //id or Equip, price
        base.SetData(pair.Key);

        if (pair.Key is int)
        {
            Card.Init((int)pair.Key, pair.Value);
        }
        else if (pair.Key is Equip)
        {
            Card.Init(((Equip)pair.Key).ID, pair.Value);
        }
    }
}
