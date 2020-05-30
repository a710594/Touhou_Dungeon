using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScrollItem : ScrollItem
{
    public IconCard Card;

    public override void SetData(object obj)
    {
        KeyValuePair<int, int> pair = (KeyValuePair<int, int>)obj; //id, price
        base.SetData(pair.Key);

        int id = pair.Key;
        int price = pair.Value;

        Card.Init(id, price);
    }
}
