using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScrollItem : ScrollItem
{
    public IconCard Card;

    public override void SetData(object obj)
    {
        if (obj is KeyValuePair<int, int>)
        {
            KeyValuePair<int, int> pair = (KeyValuePair<int, int>)obj; //id, price
            base.SetData(pair.Key);

            Card.Init(pair.Key, pair.Value);
        }
        else if(obj is KeyValuePair<Item, int>)
        {
            KeyValuePair<Item, int> pair = (KeyValuePair<Item, int>)obj; //Item, price
            base.SetData(pair.Key);

            Card.Init(pair.Key.ID, pair.Value);
        }
    }
}
