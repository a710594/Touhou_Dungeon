using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Train : MonoBehaviour
{
    public SpriteRenderer Sprite;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x + Sprite.size.x / 2 + 0.5f, transform.position.y, transform.position.z);
        transform.DOMoveX(BattleFieldManager.Instance.MapBound.xMin - Sprite.size.x / 2 - 0.5f, 0.5f).SetEase(Ease.Linear).OnComplete(()=> 
        {
            Destroy(gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
