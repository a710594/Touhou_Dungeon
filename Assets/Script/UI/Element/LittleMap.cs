using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleMap : MonoBehaviour
{
    public float Scale = 1;
    public Image Map;
    public GameObject Player;
    public GameObject Start;
    public GameObject Goal;

    private Vector2Int _playerPosition = new Vector2Int();
    private Vector2Int _goalPosition = new Vector2Int();
    private Texture2D _texture2d;
    private BoundsInt _mapBound;
    private List<Vector2Int> _mapList;

    public void Init(Vector2Int playerPosition, Vector2Int startPosition, Vector2Int goalPosition, BoundsInt mapBound, List<Vector2Int> mapList)
    {
        _mapBound = mapBound;
        _mapList = mapList;
        _playerPosition = playerPosition;
        _goalPosition = goalPosition;

        Sprite sprite;

        _texture2d = new Texture2D(mapBound.size.x + 3, mapBound.size.y + 3, TextureFormat.ARGB32, false);
        _texture2d.filterMode = FilterMode.Point;
        sprite = Sprite.Create(_texture2d, new Rect(0, 0, _texture2d.width, _texture2d.height), Vector2.zero);
        Map.sprite = sprite;
        Map.rectTransform.sizeDelta = new Vector2(_texture2d.width * Scale, _texture2d.height * Scale);

        for (int i = 0; i < _texture2d.width; i++)
        {
            for (int j = 0; j < _texture2d.height; j++)
            {
                _texture2d.SetPixel(i, j, Color.clear);
            }
        }

        _texture2d.Apply();

        Start.transform.localPosition = new Vector2((startPosition.x - _mapBound.center.x) * Scale, (startPosition.y - _mapBound.center.y) * Scale);
        Goal.transform.localPosition = new Vector2((goalPosition.x - _mapBound.center.x) * Scale, (goalPosition.y - _mapBound.center.y) * Scale);

        if (playerPosition == goalPosition)
        {
            Goal.SetActive(true);
        }
        else
        {
            Goal.SetActive(false);
        }
    }

    public void Refresh(Vector2Int playerPosition, List<Vector2Int> exploredList, List<Vector2Int> wallList)
    {
        _texture2d.SetPixel(_playerPosition.x - _mapBound.xMin + 1, _playerPosition.y - _mapBound.yMin + 1, Color.white);

        Vector2Int texturePos;

        for (int i=0; i< exploredList.Count; i++)
        {
            texturePos = new Vector2Int(exploredList[i].x - _mapBound.xMin + 1, exploredList[i].y - _mapBound.yMin + 1);
            _texture2d.SetPixel(texturePos.x, texturePos.y, Color.white);

            if (exploredList[i] == _goalPosition)
            {
                Goal.SetActive(true);
            }
        }

        for (int i = 0; i < wallList.Count; i++)
        {
            texturePos = new Vector2Int(wallList[i].x - _mapBound.xMin + 1, wallList[i].y - _mapBound.yMin + 1);
            _texture2d.SetPixel(texturePos.x, texturePos.y, Color.black);
        }

        _playerPosition = playerPosition;
        //_texture2d.SetPixel(_playerPosition.x - _mapBound.xMin + 1, _playerPosition.y - _mapBound.yMin + 1, Color.red);
        _texture2d.Apply();

        float positionX = (_mapBound.center.x - _playerPosition.x) * Scale;
        float positionY = (_mapBound.center.y - _playerPosition.y) * Scale;
        Map.transform.localPosition = new Vector2(positionX, positionY);

        Player.transform.localPosition = new Vector2((_playerPosition.x - _mapBound.center.x) * Scale, (_playerPosition.y - _mapBound.center.y) * Scale);
    }
}
