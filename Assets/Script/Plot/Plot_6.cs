using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_6 : Plot
{
    public void Check(int remain, Action callback) //檢查紫還剩幾條血
    {
        if (remain == 1) //剩一條血時觸發
        {

            Start(callback);
        }
    }

    public void Start(Action callback)
    {
        BattleUI.Instance.SetVisible(false);
        GameObject yukari = GameObject.Find("Yukari");
        Vector3 position = new Vector3(yukari.transform.position.x, yukari.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.DOMove(position, 1f).OnComplete(()=> 
        {
            ConversationUI.Open(6001, false, () =>
            {
                BattleCharacter character;

                TilePainter.Instance.Painting("Ground", 0, new Vector2Int(1, 5));
                character = GameObject.Find("Ran").GetComponent<BattleCharacter>();
                character.SetActive(true);
                character.transform.position = new Vector2(1, 5);

                TilePainter.Instance.Painting("Ground", 0, new Vector2Int(-1, 5));
                character = GameObject.Find("Chen").GetComponent<BattleCharacter>();
                character.SetActive(true);
                character.transform.position = new Vector2(-1, 5);

                BattleUI.Instance.SetVisible(true);
                callback();
            });
        });
    }
}
