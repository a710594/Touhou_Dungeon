using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleMap : MonoBehaviour
{
    public float Scale = 1;
    public Image Map;

    private Vector2Int _characterPosition = new Vector2Int();
    private Texture2D _texture2d;
    private BoundsInt _mapBound;
    private List<Vector2Int> _mapList;

    public void Init(Vector2Int characterPosition, BoundsInt mapBound, List<Vector2Int> mapList)
    {
        _mapBound = mapBound;
        _mapList = mapList;

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

        _characterPosition = characterPosition;
        _texture2d.Apply();
    }

    public void Refresh(Vector2Int characterPosition, List<Vector2Int> exploredList, List<Vector2Int> wallList)
    {
        _texture2d.SetPixel(_characterPosition.x - _mapBound.xMin + 1, _characterPosition.y - _mapBound.yMin + 1, Color.white);

        Vector2Int texturePos;

        for (int i=0; i< exploredList.Count; i++)
        {
            texturePos = new Vector2Int(exploredList[i].x - _mapBound.xMin + 1, exploredList[i].y - _mapBound.yMin + 1);
            _texture2d.SetPixel(texturePos.x, texturePos.y, Color.white);
        }

        for (int i = 0; i < wallList.Count; i++)
        {
            texturePos = new Vector2Int(wallList[i].x - _mapBound.xMin + 1, wallList[i].y - _mapBound.yMin + 1);
            _texture2d.SetPixel(texturePos.x, texturePos.y, Color.black);
        }

        _characterPosition = characterPosition;
        _texture2d.SetPixel(_characterPosition.x - _mapBound.xMin + 1, _characterPosition.y - _mapBound.yMin + 1, Color.red);
        _texture2d.Apply();

        float positionX = (_mapBound.center.x - _characterPosition.x) * Scale;
        float positionY = (_mapBound.center.y - _characterPosition.y) * Scale;
        Map.transform.localPosition = new Vector2(positionX, positionY);
    }
}
