using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_6 : Plot
{
    private bool _start1 = false;
    private bool _start2 = false;
    private Timer _timer = new Timer();

    public void Check(Action callback) //檢查紫還剩幾條血
    {
        BattleCharacter character = GameObject.Find("Yukari").GetComponent<BattleCharacter>();
        if (!_start1 &&character.Info.HPQueue.Count == 1) //剩一條血時觸發
        {

            Start_1(callback);
            _start1 = true;
        }
        else if (!_start2 &&  character.Info.HPQueue.Count == 0)
        {
            Start_2(callback);
            _start2 = true;
            BattleController.Instance.ShowEndHandler -= Check;
        }
        else
        {
            callback();
        }
    }

    public void Start_1(Action callback) //剩下一條血時,召喚藍和橙
    {
        BattleUI.Instance.SetVisible(false);
        GameObject yukari = GameObject.Find("Yukari");
        Vector3 position = new Vector3(yukari.transform.position.x, yukari.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.DOMove(position, 0.5f).OnComplete(()=> 
        {
            ConversationUI.Open(6001, false, () =>
            {
                BattleCharacter character;

                TilePainter.Instance.Painting("Ground_1", 0, new Vector2Int(1, 5));
                BattleFieldManager.Instance.SetField(new Vector2(1, 5), 1);
                character = GameObject.Find("Ran").GetComponent<BattleCharacter>();
                character.SetActive(true);
                character.SetPosition(new Vector2(1, 5));
                character.transform.DOJump(character.transform.position, 1, 1, 0.5f);

                TilePainter.Instance.Painting("Ground_1", 0, new Vector2Int(-1, 5));
                BattleFieldManager.Instance.SetField(new Vector2(-1, 5), 1);
                character = GameObject.Find("Chen").GetComponent<BattleCharacter>();
                character.SetActive(true);
                character.SetPosition(new Vector2(-1, 5));
                character.transform.DOJump(character.transform.position, 1, 1, 0.5f);

                _timer.Start(1, ()=> 
                {
                    BattleUI.Instance.SetVisible(true);
                    callback();
                });
            });
        });
    }

    public void Start_2(Action callback) //剩下0條血的時候進行該戰鬥中最後一段對話
    {
        BattleUI.Instance.SetVisible(false);
        GameObject yukari = GameObject.Find("Yukari");
        Vector3 position = new Vector3(yukari.transform.position.x, yukari.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.DOMove(position, 0.5f).OnComplete(() =>
        {
            ConversationUI.Open(7001, false, () =>
            {
                BattleUI.Instance.SetVisible(true);
                callback();
            });
        });
    }
}
