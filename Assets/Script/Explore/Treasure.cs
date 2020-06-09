using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure
{
    public string TileName;
    public List<int> ItemList = new List<int>();

    public Treasure() { }

    public Treasure(int id)
    {
        TreasureData.RootObject data = TreasureData.GetData(id);
        TileName = data.Image;
        ItemList = data.GetItemList();
    }
}
