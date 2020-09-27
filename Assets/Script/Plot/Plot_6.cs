using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_6 : Plot
{
    private Timer _timer = new Timer();

    public void Check(int remain) //檢查紫還剩幾條血
    {
        if (remain == 1) //剩一條血時觸發
        {

            Start_1();
        }
        else
        {
            Start_2();
        }
    }

    public void Start_1() //剩下一條血時,召喚藍和橙
    {
        BattleUI.Instance.SetVisible(false);
        GameObject yukari = GameObject.Find("Yukari");
        Vector3 position = new Vector3(yukari.transform.position.x, yukari.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.DOMove(position, 0.5f).OnComplete(()=> 
        {
            ConversationUI.Open(6001, false, () =>
            {
                BattleCharacter character;

                TilePainter.Instance.Painting("Ground", 0, new Vector2Int(1, 5));
                character = GameObject.Find("Ran").GetComponent<BattleCharacter>();
                character.SetActive(true);
                character.transform.position = new Vector2(1, 5);
                character.transform.DOJump(character.transform.position, 1, 1, 0.5f);

                TilePainter.Instance.Painting("Ground", 0, new Vector2Int(-1, 5));
                character = GameObject.Find("Chen").GetComponent<BattleCharacter>();
                character.SetActive(true);
                character.transform.position = new Vector2(-1, 5);
                character.transform.DOJump(character.transform.position, 1, 1, 0.5f);

                _timer.Start(1, ()=> 
                {
                    BattleUI.Instance.SetVisible(true);
                });
            });
        });
    }

    public void Start_2() //剩下0條血的時候進行該戰鬥中最後一段對話
    {
        BattleUI.Instance.SetVisible(false);
        GameObject yukari = GameObject.Find("Yukari");
        Vector3 position = new Vector3(yukari.transform.position.x, yukari.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.DOMove(position, 0.5f).OnComplete(() =>
        {
            ConversationUI.Open(7001, false, () =>
            {
                BattleUI.Instance.SetVisible(true);
            });
        });
    }
}
