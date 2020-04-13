using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldEnemyGuard : FieldEnemy
{
    //守衛型的敵人不會移動,但是觸發戰鬥的範圍比較大

    public Action<Vector2> CheckPositionHandler;

    public int VisionDistance = 1;

    public override void Init(int battleGroupId, Vector2 position)
    {
        base.Init(battleGroupId, position);
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position + Vector3.up + Vector3.left));
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position + Vector3.up));
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position + Vector3.up + Vector3.right));
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position + Vector3.left));
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position));
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position + Vector3.right));
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position + Vector3.down + Vector3.left));
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position + Vector3.down));
        TilePainter.Instance.Painting("RedGrid", 1, Vector2Int.RoundToInt(transform.position + Vector3.down + Vector3.right));
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (OnPlayerEnterHandler != null)
            {
                OnPlayerEnterHandler(_battleGroupId);
            }

            if (CheckPositionHandler != null)
            {
                CheckPositionHandler(transform.position);
            }
        }
    }
}
