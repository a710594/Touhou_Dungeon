using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriorityQueue : MonoBehaviour
{
    public PriorityQueueElement Element;

    private int _index;
    private List<KeyValuePair<BattleCharacter, PriorityQueueElement>> _imageList = new List<KeyValuePair<BattleCharacter, PriorityQueueElement>>();

    public void Init(List<BattleCharacter> characterList, List<int> priorityList)
    {
        _index = -1;
        for (int i=1; i<_imageList.Count; i++)
        {
            Destroy(_imageList[i].Value.gameObject);
        }
        _imageList.Clear();

        PriorityQueueElement element;
        for (int i = 0; i < characterList.Count; i++)
        {
            if (i > 0)
            {
                element = Instantiate(Element);
            }
            else
            {
                element = Element;
            }
            element.transform.SetParent(Element.transform.parent);
            element.transform.localScale = Vector3.one;
            _imageList.Add(new KeyValuePair<BattleCharacter, PriorityQueueElement>(characterList[i], element));

            if (characterList[i].Info.JobData != null)
            {
                element.SetData(characterList[i].Info.JobData.SmallImage, priorityList[i]);
            }
            else if (characterList[i].Info.EnemyData != null)
            {
                element.SetData(characterList[i].Info.EnemyData.Image, priorityList[i]);
            }
            _imageList[i].Value.gameObject.SetActive(true);
        }
    }

    public void Scroll(BattleCharacter character)
    {
        //if (_index > -1) //第一個角色行動時不捲動
        //{
        //    _imageList[_index].Value.gameObject.SetActive(false);
        //    if (character.Info.ActionCount == 0)
        //    {
        //        for (int i=_index; i<_imageList.Count; i++)
        //        {
        //            if (GameObject.ReferenceEquals(character, _imageList[i].Key))
        //            {
        //                _imageList[i].Value.gameObject.SetActive(false);
        //            }
        //        }
        //    }
        //}
        //_index++;

        for (int i = 0; i < _imageList.Count; i++)
        {
            if (!_imageList[i].Value.isActiveAndEnabled)
            {
                continue;
            }
            if (GameObject.ReferenceEquals(character, _imageList[i].Key))
            {
                _imageList[i].Value.gameObject.SetActive(false);

                if (character.Info.ActionCount > 0 && character.LiveState == BattleCharacter.LiveStateEnum.Alive)
                {
                    break;
                }
            }
        }
    }
}
