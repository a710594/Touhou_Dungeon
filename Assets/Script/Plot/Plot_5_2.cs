using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_5_2 : Plot
{
    private CheckState _start1 = CheckState.NotSatisfy;
    private CheckState _start2 = CheckState.NotSatisfy;
    private Timer _timer = new Timer();

    public Plot_5_2()
    {
        BattleCharacter character;
        character = GameObject.Find("Yukari").GetComponent<BattleCharacter>();
        character.GetDamageHandler += Check;
    }

    public override void Start(Action callback)
    {
        if (_start1 == CheckState.Satisfy)
        {
            Start_1(callback);
        }
        else if (_start2 == CheckState.Satisfy)
        {
            Start_2(callback);
        }
        else
        {
            callback();
        }
    }

    private void Check(BattleCharacter character) //檢查紫還剩幾條血
    {
        if (_start1 == CheckState.NotSatisfy &&character.Info.HPQueue.Count == 2 && character.Info.CurrentHP == 0) //剩一條血時觸發
        {
            _start1 = CheckState.Satisfy;
        }
        else if (_start2 == CheckState .NotSatisfy &&  character.Info.HPQueue.Count == 1 && character.Info.CurrentHP == 0)
        {
            _start2 = CheckState.Satisfy;
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
                    _start1 = CheckState.Completed;
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
                _start2 = CheckState.Completed;
                callback();
            });
        });
    }
}
