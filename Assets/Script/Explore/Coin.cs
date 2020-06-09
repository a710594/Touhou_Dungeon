using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    public SpriteRenderer Sprite;

    private bool _isTrigger = false;

    Timer _timer = new Timer();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isTrigger && collision.tag == "Player")
        {
            _isTrigger = true;
            Sprite.transform.DOLocalJump(Sprite.transform.localPosition, 1, 1, 1).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                Destroy(gameObject);
            });
            Sprite.transform.DOScale(Vector3.zero, 1).SetEase(Ease.OutCubic);
        }
    }
}
