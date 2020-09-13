using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public AnchorValueBar AnchorValueBar;

    private void Start()
    {
        //for (int i=0; i<10; i++)
        //{
        //    for (int j=0; j<10; j++)
        //    {
        //        TilePainter.Instance.Painting("Ground", 0, new Vector2Int(i, j));
        //    }
        //}
        TilePainter.Instance.Fill("Mist", 0, 0, 0, 10, 10);
        Debug.Log(TilePainter.Instance.TileMap[0].cellBounds);
    }
}
