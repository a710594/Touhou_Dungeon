using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriorityQueueElement : MonoBehaviour
{
    public Image Character;
    public Image BG;

    public void SetData(string name)
    {
        Character.overrideSprite = Resources.Load<Sprite>("Image/Character/Small/" + name);
    }
}
