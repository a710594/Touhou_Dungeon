using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriorityQueue : MonoBehaviour
{
    public Image Image;


    //private List<KeyValuePair<BattleCharacter, PriorityQueueElement>> _imageList = new List<KeyValuePair<BattleCharacter, PriorityQueueElement>>();
    private Dictionary<BattleCharacter, Image> _imageDic = new Dictionary<BattleCharacter, Image>();
    private Dictionary<BattleCharacter, bool> _activeDic = new Dictionary<BattleCharacter, bool>();

    public void Init(List<BattleCharacter> characterList)
    {
        //for (int i=1; i<_imageList.Count; i++)
        //{
        //    Destroy(_imageList[i].Value.gameObject);
        //}
        //_imageList.Clear();

        foreach (KeyValuePair<BattleCharacter, Image> item in _imageDic) 
        {
            if (!ReferenceEquals(item.Value, Image)) //把第一個 Image 以外的刪掉
            {
                Destroy(item.Value.gameObject);
            }
        }
        _imageDic.Clear();
        _activeDic.Clear();

        Image image;
        for (int i = 0; i < characterList.Count; i++)
        {
            if (i > 0)
            {
                image = Instantiate(Image);
            }
            else
            {
                image = Image;
            }
            image.transform.SetParent(Image.transform.parent);
            image.transform.localScale = Vector3.one;
            //_imageList.Add(new KeyValuePair<BattleCharacter, PriorityQueueElement>(characterList[i], image));
            _imageDic.Add(characterList[i], image);

            if (characterList[i].Info.JobData != null)
            {
                //image.SetData(characterList[i].Info.JobData.SmallImage);
                image.overrideSprite = Resources.Load<Sprite>("Image/Character/Small/" + characterList[i].Info.JobData.SmallImage);
            }
            else if (characterList[i].Info.EnemyData != null)
            {
                //image.SetData(characterList[i].Info.EnemyData.Image);
                image.overrideSprite = Resources.Load<Sprite>("Image/Character/Small/" + characterList[i].Info.EnemyData.Image);
            }
            //_imageList[i].Value.gameObject.SetActive(true);
            image.gameObject.SetActive(true);
            _activeDic.Add(characterList[i], true);
        }
    }

    public void Scroll(BattleCharacter character)
    {
        //for (int i = 0; i < _imageList.Count; i++)
        //{
        //    if (!_imageList[i].Value.isActiveAndEnabled)
        //    {
        //        continue;
        //    }
        //    if (GameObject.ReferenceEquals(character, _imageList[i].Key))
        //    {
        //        _imageList[i].Value.gameObject.SetActive(false);

        //        if (character.Info.ActionCount > 0 && character.LiveState == BattleCharacter.LiveStateEnum.Alive)
        //        {
        //            break;
        //        }
        //    }
        //}

        foreach (KeyValuePair<BattleCharacter, Image> item in _imageDic)
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
