using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot_5 : Plot
{
    private int _count = 0;

    public void Check(BattleCharacter character) //縫隙死掉時檢查,死掉兩個的時候觸發 Start
    {
        _count++;
        if (_count == 2)
        {
            Start();
        }
    }

    public override void Start()
    {
        TilePainter.Instance.Painting("Ground", 0, new Vector2Int(0, 4));
        BattleCharacter character = GameObject.Find("Yukari").GetComponent<BattleCharacter>();
        character.SetActive(true);
        character.transform.position = new Vector2(0, 5);

    }
}
