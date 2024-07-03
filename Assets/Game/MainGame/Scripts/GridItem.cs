using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GridItem : MonoBehaviour
{
    [SerializeField]
    private Button _gridButton;
    [SerializeField]
    private Image _selectImage;
    [SerializeField]
    private Outline _outline;
    private bool _isSelected = false;
    private Action<GridItem> _selectGridItem;
    private Color _defaultColor;
    [SerializeField]
    private List<GridItem> _items;
    private int _currentPosition;
    public bool IsReplacing = false;
    [SerializeField]
    private bool _isMatched = false;
    [SerializeField]
    private CanvasGroup _canvasGroup;

   

    private void Awake()
    {
        _gridButton.onClick.AddListener(OnClickGridItem);
    }

    private void OnClickGridItem()
    {
        SelectDeselectItem(!_isSelected);
        _selectGridItem?.Invoke(this);
    }

    public void SelectDeselectItem(bool selectItem)
    {
        _isSelected = selectItem;
        _outline.effectColor = _isSelected ? Color.yellow : Color.white;
    }

    public GridItem GetGridItemFromDirection(EnumGridDirection direction)
    {
        return _items[(int)direction];
    }

    public void SetSelectGridItem(Action<GridItem> selectGridItem)
    {
        _selectGridItem = selectGridItem;
    }

    public void SetColor(Color color)
    {
        _selectImage.color = color;
        _defaultColor = color;
    }

    internal Color GetColor()
    {
        return _defaultColor;
    }

    public void SetSideGridItems(List<GridItem> items)
    {
        _items = items;
    }

    public GridItem GetColorBeside(List<GridItem> _itemsMatched = null)
    {

        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] == null)
            {
                continue;
            }
            if (_items[i].GetIsMatched()){
                continue;
            }
            if (_itemsMatched != null)
            {
                if (_itemsMatched.Contains(_items[i]))
                {
                    continue;
                }
            }
           
            if (_items[i].GetColor() == _defaultColor)
            {
                return _items[i];
            }
        }
        return null;
    }

    public List<GridItem> GetAllSameColorBeside()
    {
        var colorBeside = new List<GridItem>();

        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] == null)
            {
                continue;
            }
            if (_items[i].GetIsMatched())
            {
                continue;
            }
            if (_items[i].GetColor() == _defaultColor)
            {
                colorBeside.Add(_items[i]);
            }
        }

        return colorBeside;
    }

    public void Match()
    {
        //gameObject.SetActive(false);
        FadeOut();
        _isMatched = true;
    }

    public void SetPosition(int i)
    {
        _currentPosition = i;
        //gameObject.name = _currentPosition.ToString();
    }

    public int GetPosition()
    {
        return _currentPosition;
    }

    public bool GetIsMatched()
    {
        return _isMatched;
    }

    public void FadeIn()
    {
        _canvasGroup.DOFade(0, 0.001f);
        _canvasGroup.DOFade(255, 0.4f);
        _isMatched = false;
    }

    public void FadeOut()
    {
        _canvasGroup.DOFade(255, 0.001f);
        _canvasGroup.DOFade(0, 0.8f);
       
    }

    public void SetIsMatched(bool matched)
    {
        _isMatched = matched;
    }

    public List<GridItem> GetSides()
    {
        return _items;
    }
}

