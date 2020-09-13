using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriorityQueueElement : MonoBehaviour
{
    public Image Character;
    public Image BG;

    public void SetData(string name, int priority)
    {
        Character.overrideSprite = Resources.Load<Sprite>("Image/Character/Small/" + name);

        if (priority > 100)
        {
            BG.color = new Color32(145, 255, 255, 255);
        }
        else if (priority < 100)
        {
            BG.color = new Color32(50, 100, 200, 255);
        }
        else
        {
            BG.color = Color.white;
        }
    }
}
