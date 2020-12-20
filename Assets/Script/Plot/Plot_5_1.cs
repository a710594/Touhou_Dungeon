using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plot_5_1 : Plot
{
    public bool IsCompleted = false;
    private CheckState _allGapDead = CheckState.NotSatisfy;

    public Plot_5_1()
    {
        BattleCharacter character;
        for (int i = 1; i <= 2; i++)
        {
            character = GameObject.Find("Gap_" + i).GetComponent<BattleCharacter>();
            character.GetDamageHandler += Check;
        }
    }

    public override void Start(Action callback)
    {
        if (_allGapDead == CheckState.Satisfy)
        {
            TilePainter.Instance.Painting("Ground_1", 0, new Vector2Int(0, 4));
            BattleFieldManager.Instance.SetField(new Vector2(0, 4), 1);
            BattleCharacter character = GameObject.Find("Yukari").GetComponent<BattleCharacter>();
            BattleController.Instance.SetCharacerActive(character);
            character.SetPosition(new Vector2(0, 4));
            Vector3 cameraPosition = new Vector3(character.transform.position.x, character.transform.position.y, Camera.main.transform.position.z);
            Camera.main.transform.DOMove(cameraPosition, 1);
            character.Sprite.color = Color.clear;
            character.Sprite.DOColor(Color.white, 1).OnComplete(() =>
            {
                BattleUI.Instance.SetVisible(false);
                ConversationUI.Open(5001, false, () =>
                {
                    BattleUI.Instance.SetVisible(true);
                    _allGapDead = CheckState.Completed;
                    IsCompleted = true;
                    callback();
                });
            });
        }
        else
        {
            callback();
        }
    }


    private void Check(BattleCharacter character) //縫隙死掉時檢查,死掉兩個的時候觸發 Start
    {
        int count = 0;
        List<BattleCharacter> characterList = BattleController.Instance.CharacterList;
        for (int i=0; i< characterList.Count; i++) 
        {
            if (characterList[i].EnemyId == 8 && characterList[i].LiveState == BattleCharacter.LiveStateEnum.Dead)
            {
                count++;
            }
        }

        if (count == 2)
        {
            _allGapDead = CheckState.Satisfy;
        }
    }
}
