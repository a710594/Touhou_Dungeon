using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoopScrollView : MonoBehaviour
{
    public enum Type
    {
        Horizontal,
        Vertical,
    }

    public Image Mask;
    public Image Content;
    public GridLayoutGroup Grid;
    public ScrollRect ScrollView;
    public ScrollItem ScrollItem;
    [Range(1, 3)]
    public int PreparationAmount = 2;
    public Vector2 Spacing = new Vector2();
    public Type Direction;
    public bool ShowSelected = false;

    private bool _isInit = false;
    private int _currentIndex = 0;
    private int _gridAmount = 0; //Grid 的數量
    private int _gridElementAmount; //Grid 上物件的數量
    private float _itemLength; //舉例來說,垂直ScrollView的_itemLength是元件的高度
    private float _itemSubLength; //那他的_itemSubLength就是元件的寬度
    private float _upperBound;
    private float _lowerBound;
    private float _canvasScale;
    private float _isEven; //如果是偶數則為1,反之為0
    private ScrollItem _selectedItem = null;
    private List<List<ScrollItem>> _itemList = new List<List<ScrollItem>>();
    private List<GridLayoutGroup> _gridList = new List<GridLayoutGroup>();
    private List<ArrayList> _dataList = new List<ArrayList>();
    private Action<object> _onClickHandler;
    private Action<object> _onDownHandler;
    private Action<object> _onPressHandler;
    private Action<object> _onUpHandler;

    public void SetData(ArrayList list)
    {
        if (!_isInit)
        {
            Init();
        }

        if (list.Count == 0)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                for (int j = 0; j < _itemList[i].Count; j++)
                {
                    _itemList[i][j].gameObject.SetActive(false);
                }
            }
            return;
        }
        else
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                for (int j = 0; j < _itemList[i].Count; j++)
                {
                    _itemList[i][j].gameObject.SetActive(true);
                }
            }
        }

        _dataList.Clear();
        int count = 0;
        int dataGroupAmount = (int)Mathf.Ceil((float)list.Count / (float)_gridElementAmount);
        for (int i = 0; i < dataGroupAmount; i++)
        {
            ArrayList arrayList = new ArrayList();
            _dataList.Add(arrayList);
            for (int j=0; j<_gridElementAmount; j++)
            {
                if (count < list.Count)
                {
                    _dataList[i].Add(list[count]);
                    count++;
                }
            }
        }

        bool isShort; //內容物比ScrollView還小
        if (Direction == Type.Horizontal)
        {
            isShort = _itemLength * _dataList.Count + Spacing.x * (_dataList.Count - 1) < Mask.rectTransform.rect.width;
            if (isShort)
            {
                Content.rectTransform.sizeDelta = new Vector2(_itemLength * (_gridAmount - PreparationAmount) + Spacing.x * ((_gridAmount - PreparationAmount) - 1), _itemSubLength * _gridElementAmount + Spacing.y * (_gridElementAmount - 1));
            }
            else
            {
                Content.rectTransform.sizeDelta = new Vector2(_itemLength * _dataList.Count + Spacing.x * (_dataList.Count - 1), _itemSubLength * _gridElementAmount + Spacing.y * (_gridElementAmount - 1));
            }
            Content.transform.position = new Vector3(Mask.transform.position.x - Mask.rectTransform.rect.width / 2 * _canvasScale + Content.rectTransform.sizeDelta.x / 2 * _canvasScale, Content.transform.position.y);
            for (int i=0; i<_gridAmount; i++)
            {
                if (isShort)
                {
                    _gridList[i].transform.localPosition = new Vector3((((_gridAmount - PreparationAmount) / 2f) - i - 0.5f) * (_itemLength + Spacing.x) * -1f, (_gridElementAmount / 2 - _isEven * 0.5f) * (_itemSubLength + Spacing.y));
                }
                else
                {
                    _gridList[i].transform.localPosition = new Vector3(((_dataList.Count / 2f) - i - 0.5f) * (_itemLength + Spacing.x) * -1f, (_gridElementAmount / 2 - _isEven * 0.5f) * (_itemSubLength + Spacing.y));
                }
                for (int j=0; j<_gridElementAmount; j++)
                {
                    if (i < _dataList.Count && j < _dataList[i].Count)
                    {
                        _itemList[i][j].SetData(_dataList[i][j]);
                        _itemList[i][j].transform.SetParent(_gridList[i].transform);
                    }
                    else
                    {
                        _itemList[i][j].gameObject.SetActive(false);
                    }
                }
            }
            _lowerBound = _gridList[0].transform.position.x - _itemLength / 2;
        }
        else
        {
            isShort = _itemLength * _dataList.Count + Spacing.y * (_dataList.Count - 1) < Mask.rectTransform.rect.height;
            if (isShort)
            {
                Content.rectTransform.sizeDelta = new Vector2(_itemSubLength * _gridElementAmount + Spacing.x * (_gridElementAmount - 1), _itemLength * (_gridAmount - PreparationAmount) + Spacing.y * (_gridAmount - PreparationAmount - 1));
            }
            else
            {
                Content.rectTransform.sizeDelta = new Vector2(_itemSubLength * _gridElementAmount + Spacing.x * (_gridElementAmount - 1), _itemLength * _dataList.Count + Spacing.y * (_dataList.Count - 1));
            }
            Content.transform.position = new Vector3(Content.transform.position.x, Mask.transform.position.y + Mask.rectTransform.rect.height / 2 * _canvasScale - Content.rectTransform.sizeDelta.y / 2 * _canvasScale);
            for (int i = 0; i < _gridAmount; i++)
            {
                if (isShort)
                {
                    _gridList[i].transform.localPosition = new Vector3(-1 * (_gridElementAmount / 2 - _isEven * 0.5f) * (_itemSubLength + Spacing.x), (((_gridAmount - PreparationAmount) / 2f) - i - 0.5f) * (_itemLength + Spacing.y));
                }
                else
                {
                    _gridList[i].transform.localPosition = new Vector3(-1 * (_gridElementAmount / 2 - _isEven * 0.5f) * (_itemSubLength + Spacing.x), ((_dataList.Count / 2f) - i - 0.5f) * (_itemLength + Spacing.y));
                }
                for (int j = 0; j < _gridElementAmount; j++)
                {
                    if (i < _dataList.Count && j < _dataList[i].Count)
                    {
                        _itemList[i][j].SetData(_dataList[i][j]);
                        _itemList[i][j].transform.SetParent(_gridList[i].transform);
                    }
                    else
                    {
                        _itemList[i][j].gameObject.SetActive(false);
                    }
                }
            }
            _upperBound = _gridList[0].transform.position.y + _itemLength / 2;
        }

        if (_selectedItem != null)
        {
            _selectedItem.SetSelected(false);
        }

        _selectedItem = null;
        _onClickHandler = null;
        _onDownHandler = null;
        _onPressHandler = null;
        _onUpHandler = null;
    }

    public void SetFirstElement(object data) //在 SetData 之後使用
    {
        int index = 0;
        bool hasFound = false;
        Vector2 orignalPosition = new Vector2();
        for (int i=0; i<_dataList.Count; i++)
        {
            for (int j=0; j<_dataList[i].Count; j++)
            {
                if(Equals(data, _dataList[i][j]))
                {
                    index = i;
                    hasFound = true;
                    break;
                }
            }
            if (hasFound)
            {
                break;
            }
        }

        if (hasFound)
        {
            if (Direction == Type.Horizontal)
            {
                orignalPosition = Content.transform.localPosition;
                Content.transform.localPosition = new Vector2(orignalPosition.x - index * (_itemLength + Spacing.x), Content.transform.localPosition.y);
                if (Content.transform.localPosition.x < orignalPosition.x * -1)
                {
                    Content.transform.localPosition = new Vector2(orignalPosition.x * -1, Content.transform.localPosition.y);
                }

                if (index > _dataList.Count - _gridAmount)
                {
                    index = _dataList.Count - _gridAmount;

                    if (index < 0)
                    {
                        index = 0;
                    }
                }

                for (int i=0; i<_gridList.Count; i++)
                {
                    orignalPosition = _gridList[i].transform.localPosition;
                    _gridList[i].transform.localPosition = new Vector2(orignalPosition.x + index * (_itemLength + Spacing.x), orignalPosition.y);
                }
            }
            else
            {
                orignalPosition = Content.transform.localPosition;
                Content.transform.localPosition = new Vector2(Content.transform.localPosition.x, orignalPosition.y + index * (_itemLength + Spacing.y));
                if (Content.transform.localPosition.y > orignalPosition.y * -1)
                {
                    Content.transform.localPosition = new Vector2(Content.transform.localPosition.x, orignalPosition.y * -1);
                }

                if (index > _dataList.Count - _gridAmount)
                {
                    index = _dataList.Count - _gridAmount;

                    if (index < 0)
                    {
                        index = 0;
                    }
                }

                for (int i = 0; i < _gridList.Count; i++)
                {
                    orignalPosition = _gridList[i].transform.localPosition;
                    _gridList[i].transform.localPosition = new Vector2(orignalPosition.x, orignalPosition.y - index * (_itemLength + Spacing.y));
                }
            }

            for (int i = 0; i < _gridAmount; i++)
            {
                for (int j = 0; j < _gridElementAmount; j++)
                {
                    if ((index + i) < _dataList.Count &&  j < _dataList[index + i].Count)
                    {
                        _itemList[i][j].gameObject.SetActive(true);
                        _itemList[i][j].SetData(_dataList[index + i][j]);
                    }
                    else
                    {
                        _itemList[i][j].gameObject.SetActive(false);
                    }
                }
            }
            _currentIndex = _gridAmount + index;
        }
    }

    public void AddClickHandler(Action<object> callback)
    {
        _onClickHandler += callback;
    }

    public void RemoveClickHandler(Action<object> callback)
    {
        _onClickHandler -= callback;
    }

    public void AddDownHandler(Action<object> callback)
    {
        _onDownHandler += callback;
    }

    public void RemoveDownHandler(Action<object> callback)
    {
        _onDownHandler -= callback;
    }

    public void AddPressHandler(Action<object> callback)
    {
        _onPressHandler += callback;
    }

    public void RemovePressHandler(Action<object> callback)
    {
        _onPressHandler -= callback;
    }

    public void AddUpHandler(Action<object> callback)
    {
        _onUpHandler += callback;
    }

    public void RemoveUpHandler(Action<object> callback)
    {
        _onUpHandler -= callback;
    }

    public void RemoveSelectedItem() 
    {
        if (_selectedItem != null)
        {
            _selectedItem.SetSelected(false);
            _selectedItem = null;
        }
    }

    private void Init()
    {
        //int itemAmount = 0;

        if (Direction == Type.Horizontal)
        {
            _itemLength = ScrollItem.Background.rectTransform.rect.width;
            _itemSubLength = ScrollItem.Background.rectTransform.rect.height;
            _gridAmount = (int)Mathf.Ceil(Mask.rectTransform.rect.width / _itemLength) + PreparationAmount;
            _gridElementAmount = (int)Mathf.Floor(Mask.rectTransform.rect.height / (_itemSubLength + Spacing.y));
            //itemAmount = ((int)Mathf.Ceil(Mask.rectTransform.rect.width / _itemLength) + PreparationAmount) * _gridElementAmount;
            ScrollView.horizontal = true;
            ScrollView.vertical = false;
            Grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        }
        else
        {
            _itemLength = ScrollItem.Background.rectTransform.rect.height;
            _itemSubLength = ScrollItem.Background.rectTransform.rect.width;
            _gridAmount = (int)Mathf.Ceil(Mask.rectTransform.rect.height / _itemLength) + PreparationAmount;
            _gridElementAmount = (int)Mathf.Floor(Mask.rectTransform.rect.width / (_itemSubLength + Spacing.x));
            //itemAmount = ((int)Mathf.Ceil(Mask.rectTransform.rect.height / _itemLength) + PreparationAmount) * _gridElementAmount;
            ScrollView.horizontal = false;
            ScrollView.vertical = true;
            Grid.startAxis = GridLayoutGroup.Axis.Vertical;
        }
        _currentIndex = _gridAmount;
        _isEven = (_gridElementAmount + 1) % 2;

        Vector2 size = new Vector2(ScrollItem.Background.rectTransform.rect.width, ScrollItem.Background.rectTransform.rect.height);
        Grid.GetComponent<RectTransform>().sizeDelta = size;
        Grid.cellSize = size;
        Grid.spacing = Spacing;

        GameObject obj;
        for (int i = 0; i < _gridAmount; i++)
        {
            obj = Instantiate(Grid.gameObject, transform.position, transform.rotation);
            obj.transform.SetParent(Content.transform);
            obj.transform.localScale = Vector3.one;
            _gridList.Add(obj.GetComponent<GridLayoutGroup>());
            _itemList.Add(new List<ScrollItem>());
            for (int j=0; j<_gridElementAmount; j++)
            {
                obj = Instantiate(ScrollItem.gameObject, transform.position, transform.rotation);
                obj.transform.SetParent(Content.transform);
                obj.transform.localScale = Vector3.one;

                ScrollItem scrollItem = obj.GetComponent<ScrollItem>();
                _itemList[i].Add(scrollItem);
                scrollItem.OnClickHandler = OnClick;
                scrollItem.OnDownHandler = OnDown;
                scrollItem.OnPressHandler = OnPress;
                scrollItem.OnUpHandler = OnUp;
            }
        }
        _canvasScale = GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
        _isInit = true;
    }

    private void OnClick(object data, ScrollItem item)
    {
        if (_onClickHandler != null)
        {
            _onClickHandler(data);
        }

        if (ShowSelected)
        {
            if (_selectedItem != null)
            {
                _selectedItem.SetSelected(false);
            }
            item.SetSelected(true);
            _selectedItem = item;
        }
    }

    private void OnDown(object data)
    {
        if (_onDownHandler != null)
        {
            _onDownHandler(data);
        }
    }

    private void OnPress(object data)
    {
        if (_onPressHandler != null)
        {
            _onPressHandler(data);
        }
    }

    private void OnUp(object data)
    {
        if (_onUpHandler != null)
        {
            _onUpHandler(data);
        }
    }

    private GridLayoutGroup _tempGrid;
    private List<ScrollItem> _tempItem;
    void Update()
    {
        if (Direction == Type.Horizontal)
        {
            if (_currentIndex < _dataList.Count && _gridList[0].transform.position.x + _itemLength / 2 < _lowerBound)
            {
                _tempGrid = _gridList[0];
                _tempGrid.transform.localPosition = new Vector3(_tempGrid.transform.localPosition.x + _gridList.Count * (_itemLength + Spacing.x), _tempGrid.transform.localPosition.y);
                _gridList.Remove(_tempGrid);
                _gridList.Insert(_gridList.Count, _tempGrid);

                _tempItem = _itemList[0];
                _itemList.Remove(_tempItem);
                _itemList.Insert(_itemList.Count, _tempItem);
                for (int i=0; i<_gridElementAmount; i++)
                {
                    if (i < _dataList[_currentIndex].Count)
                    {
                        _itemList[_itemList.Count - 1][i].gameObject.SetActive(true);
                        _itemList[_itemList.Count - 1][i].SetData(_dataList[_currentIndex][i]);
                    }
                    else
                    {
                        _itemList[_itemList.Count - 1][i].gameObject.SetActive(false);
                    }
                }
                _currentIndex++;
                Debug.Log(_currentIndex);
            }

            if (_currentIndex > _gridList.Count && _gridList[0].transform.position.x - _itemLength / 2 > _lowerBound)
            {
                _tempGrid = _gridList[_gridList.Count - 1];
                _tempGrid.transform.localPosition = new Vector3(_tempGrid.transform.localPosition.x - _gridList.Count * (_itemLength + Spacing.x), _tempGrid.transform.localPosition.y);
                _gridList.Remove(_tempGrid);
                _gridList.Insert(0, _tempGrid);

                _tempItem = _itemList[_itemList.Count - 1];
                _itemList.Remove(_tempItem);
                _itemList.Insert(0, _tempItem);
                for (int i = 0; i < _gridElementAmount; i++)
                {
                    if (i < _dataList[_currentIndex - _itemList.Count - 1].Count)
                    {
                        _itemList[0][i].gameObject.SetActive(true);
                        _itemList[0][i].SetData(_dataList[_currentIndex - _itemList.Count - 1][i]);
                    }
                    else
                    {
                        _itemList[0][i].gameObject.SetActive(false);
                    }
                }
                _currentIndex--;
                Debug.Log(_currentIndex);
            }
        }
        else
        {
            if (_currentIndex < _dataList.Count && _gridList[0].transform.position.y - _itemLength / 2 > _upperBound)
            {
                _tempGrid = _gridList[0];
                _tempGrid.transform.localPosition = new Vector3(_tempGrid.transform.localPosition.x, _tempGrid.transform.localPosition.y - _gridList.Count * (_itemLength + Spacing.y));
                _gridList.Remove(_tempGrid);
                _gridList.Insert(_gridList.Count, _tempGrid);

                _tempItem = _itemList[0];
                _itemList.Remove(_tempItem);
                _itemList.Insert(_itemList.Count, _tempItem);
                for (int i = 0; i < _gridElementAmount; i++)
                {
                    if (i < _dataList[_currentIndex].Count)
                    {
                        _itemList[_itemList.Count - 1][i].gameObject.SetActive(true);
                        _itemList[_itemList.Count - 1][i].SetData(_dataList[_currentIndex][i]);
                    }
                    else
                    {
                        _itemList[_itemList.Count - 1][i].gameObject.SetActive(false);
                    }
                }
                _currentIndex++;
            }

            if (_currentIndex > _gridList.Count && _gridList[0].transform.position.y + _itemLength / 2 < _upperBound)
            {
                _tempGrid = _gridList[_gridList.Count - 1];
                _tempGrid.transform.localPosition = new Vector3(_tempGrid.transform.localPosition.x, _tempGrid.transform.localPosition.y + _gridList.Count * (_itemLength + Spacing.y));
                _gridList.Remove(_tempGrid);
                _gridList.Insert(0, _tempGrid);

                _tempItem = _itemList[_itemList.Count - 1];
                _itemList.Remove(_tempItem);
                _itemList.Insert(0, _tempItem);
                for (int i = 0; i < _gridElementAmount; i++)
                {
                    if (i < _dataList[_currentIndex - _itemList.Count - 1].Count)
                    {
                        _itemList[0][i].gameObject.SetActive(true);
                        _itemList[0][i].SetData(_dataList[_currentIndex - _itemList.Count - 1][i]);
                    }
                    else
                    {
                        _itemList[0][i].gameObject.SetActive(false);
                    }
                }
                _currentIndex--;
            }
        }
    }
}