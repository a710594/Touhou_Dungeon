using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriorityQueue : MonoBehaviour
{
    public Image Image;

    private int _index;
    private List<KeyValuePair<BattleCharacter, Image>> _imageList = new List<KeyValuePair<BattleCharacter, Image>>();

    public void Init(List<BattleCharacter> list)
    {
        _index = -1;
        for (int i=1; i<_imageList.Count; i++)
        {
            Destroy(_imageList[i].Value.gameObject);
        }
        _imageList.Clear();

        Image image;
        for (int i = 0; i < list.Count; i++)
        {
            if (i > 0)
            {
                image = Instantiate(Image);
            }
            else
            {
                image = Image;
            }
            image.transform.parent = Image.transform.parent;
            _imageList.Add(new KeyValuePair<BattleCharacter, Image>(list[i], image));

            if (list[i].Info.JobData != null)
            {
                image.overrideSprite = Resources.Load<Sprite>("Image/Character/Small/" + list[i].Info.JobData.SmallImage);
            }
            else if (list[i].Info.EnemyData != null)
            {
                image.overrideSprite = Resources.Load<Sprite>("Image/Character/Small/" + list[i].Info.EnemyData.Image);
            }
            _imageList[i].Value.gameObject.SetActive(true);
        }
    }

    public void Scroll(BattleCharacter character)
    {
        if (_index > -1) //第一個角色行動時不捲動
        {
            _imageList[_index].Value.gameObject.SetActive(false);
            if (character.Info.ActionCount == 0)
            {
                for (int i=_index; i<_imageList.Count; i++)
                {
                    if (GameObject.ReferenceEquals(character, _imageList[i].Key))
                    {
                        _imageList[i].Value.gameObject.SetActive(false);
                    }
                }
            }
        }
        _index++;
    }
}
