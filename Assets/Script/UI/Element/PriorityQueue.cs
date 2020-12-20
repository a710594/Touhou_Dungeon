using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriorityQueue : MonoBehaviour
{
    public PriorityQueueElement PriorityQueueElement;

    private Dictionary<BattleCharacter, PriorityQueueElement> _imageDic = new Dictionary<BattleCharacter, PriorityQueueElement>();
    private Dictionary<BattleCharacter, bool> _activeDic = new Dictionary<BattleCharacter, bool>();

    public void Init(List<BattleCharacter> characterList)
    {
        foreach (KeyValuePair<BattleCharacter, PriorityQueueElement> item in _imageDic) 
        {
            if (!ReferenceEquals(item.Value, PriorityQueueElement)) //把第一個 Image 以外的刪掉
            {
                Destroy(item.Value.gameObject);
            }
        }
        _imageDic.Clear();
        _activeDic.Clear();

        PriorityQueueElement priorityQueueElement;
        for (int i = 0; i < characterList.Count; i++)
        {
            if (i > 0)
            {
                priorityQueueElement = Instantiate(PriorityQueueElement);
            }
            else
            {
                priorityQueueElement = PriorityQueueElement;
            }
            priorityQueueElement.transform.SetParent(PriorityQueueElement.transform.parent);
            priorityQueueElement.transform.localScale = Vector3.one;
            _imageDic.Add(characterList[i], priorityQueueElement);

            if (characterList[i].Info.JobData != null)
            {
                priorityQueueElement.SetData(characterList[i].Info.JobData.SmallImage);
            }
            else if (characterList[i].Info.EnemyData != null)
            {
                priorityQueueElement.SetData(characterList[i].Info.EnemyData.Image);
            }
            priorityQueueElement.gameObject.SetActive(true);
            _activeDic.Add(characterList[i], true);
        }
    }

    public void Scroll(BattleCharacter character)
    {
        foreach (KeyValuePair<BattleCharacter, PriorityQueueElement> item in _imageDic)
        {
            if (!_activeDic[item.Key])
            {
                continue;
            }
            if (ReferenceEquals(character, item.Key))
            {
                item.Value.gameObject.SetActive(false);
                _activeDic[item.Key] = false;
            }
        }
    }
}
