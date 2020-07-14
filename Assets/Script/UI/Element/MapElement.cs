using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapElement : MonoBehaviour
{
    public float Scale = 10;
    public Image Map;
    public GameObject Player;
    public GameObject Start;
    public GameObject Goal;

    private Vector2Int _playerPosition = new Vector2Int();
    private Vector2Int _goalPosition = new Vector2Int();

    public void Init(Vector2 startPosition, Vector2 goalPosition, Sprite sprite, Texture2D texture2d)
    {
        Map.sprite = sprite;
        Map.rectTransform.sizeDelta = new Vector2(texture2d.width * Scale, texture2d.height * Scale);

        Start.transform.localPosition = startPosition;
        Goal.transform.localPosition = goalPosition;
    }

    public void Refresh(Vector2 playerPosition, Vector2 mapPosition)
    {
        Player.transform.localPosition = playerPosition;
        Map.transform.localPosition = mapPosition;
    }

    public void SetStartVisible(bool isVisible)
    {
        Start.SetActive(isVisible);
    }

    public void SetGoalVisible(bool isVisible)
    {
        Goal.SetActive(isVisible);
    }
}
