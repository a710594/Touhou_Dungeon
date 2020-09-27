using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_5 : Plot
{
    private int _count = 0;

    public void Check() //縫隙死掉時檢查,死掉兩個的時候觸發 Start
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
        character.transform.position = new Vector2(0, 4);
        Vector3 cameraPosition = new Vector3(character.transform.position.x, character.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.DOMove(cameraPosition, 1);
        character.Sprite.color = Color.clear;
        character.Sprite.DOColor(Color.white, 1).OnComplete(()=> 
        {
            BattleUI.Instance.SetVisible(false);
            ConversationUI.Open(5001, false, () =>
            {
                BattleUI.Instance.SetVisible(true);
            });

            Plot_6 plot_6 = new Plot_6();
            character.OnHPDequeueHandler += plot_6.Check;
        });
    }
}
