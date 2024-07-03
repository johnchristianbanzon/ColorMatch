using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using static UnityEditor.Progress;

public class GameView : MonoBehaviour
{
    [SerializeField]
    private GridItem _gridItem;
    [SerializeField]
    private Text _timeText;
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup;
    private GridItem _selectedGridItem;
    [SerializeField]
    private List<GridItem> _gridItemList = new List<GridItem>();
    [SerializeField]
    private List<Vector2> _gridPositions = new List<Vector2>();
    private List<Color> _colorsAvailable = new List<Color>() { Color.red, Color.blue, Color.green, Color.gray, Color.magenta};
    private int _conditionToMatch = 3;
    private int _playableSize;
    private int _playableColors;
    private float _timeLeft;
    private int _score;
    private int _swaps;
    [Inject]
    private IGameManager _gameManager;

    private void Start()
    {

        var setting = _gameManager.GetGameSetting();
        _playableSize = setting.NumberOfItems;
        _playableColors = setting.NumberOfColors;
        StartGame(setting.CurrentTimeSetting);


        _gridLayoutGroup.constraintCount = _playableSize;
        
        for (int i = 0; i < _playableSize * _playableSize; i++)
        {
            _gridItemList.Add(SpawnGridItem(i));
          
        }
        SetGridItemSide();
        StartCoroutine("StartLayoutGroupDelay");
        _gameManager.SetScore(0);
        _gameManager.SetSwaps(0);

    }

    public void StartGame(float time)
    {
        _timeLeft = time;
        StopCoroutine("StartGameTimer");
        StartCoroutine("StartGameTimer");
    }

    private IEnumerator StartGameTimer()
    {
        while(_timeLeft > 0)
        {
            _timeText.text = _timeLeft.ToString();
            yield return new WaitForSeconds(1);
            _timeLeft -= 1;
            
        }

        SceneManager.LoadScene(2);
    }

    private GridItem SpawnGridItem(int position)
    {
  
        var gridItem = Instantiate(_gridItem, _gridLayoutGroup.transform);
        gridItem.name = position.ToString();
        gridItem.SetSelectGridItem(SelectGridItem);
        gridItem.SetPosition(position);
        gridItem.SetColor(_colorsAvailable[UnityEngine.Random.Range(0, _playableColors)]);
   
        return gridItem;
    }

    private void SetGridItemSide()
    {
        
        for (int i = 0; i < _gridItemList.Count; i++)
        {
            var sideList = new List<GridItem>();
            var currentPos = _gridItemList[i].GetPosition();
            foreach (EnumGridDirection directions in Enum.GetValues(typeof(EnumGridDirection)))
            {
                sideList.Add(GetSidePosition(directions, currentPos));
            }
            _gridItemList[i].SetSideGridItems(sideList);
            _gridItemList[i].IsReplacing = false;
            _gridItemList[i].SetIsMatched(false);
        }
    }

    private GridItem GetSidePosition(EnumGridDirection direction,int position)
    {
        switch (direction)
        {
            case EnumGridDirection.North:
                if (position - _playableSize >= 0)
                {
                    return _gridItemList[position - _playableSize];
                }
                break;
            case EnumGridDirection.East:
                if (position < _gridItemList.Count - 1 && (((position + 1) % (_playableSize)) != 0 || position==0))
                {
                    return _gridItemList[position + 1];
                }
                break;
            case EnumGridDirection.South:
                if (position + _playableSize <= _gridItemList.Count - 1)
                {
                    return _gridItemList[position + _playableSize];
                }
                break;
            case EnumGridDirection.West:
                if (position > 0 && (((position) % (_playableSize)) != 0))
                {
                    return _gridItemList[position - 1];
                }
                break;
        }
        return null;
    }

    private IEnumerator StartLayoutGroupDelay()
    {
        yield return new WaitForSeconds(0.1f) ;
        _gridLayoutGroup.enabled = false;
        for (int i = 0; i < _gridItemList.Count; i++)
        {
            _gridPositions.Add(_gridItemList[i].transform.position);
        }
        StopCoroutine("CheckForConditionDelay");
        StartCoroutine("CheckForConditionDelay");
    }

    private void SelectGridItem(GridItem gridItem)
    {
        if (_selectedGridItem!=null)
        {
            SwapPlaces(_selectedGridItem, gridItem);
            _selectedGridItem = null;
        }
        else
        {
            _selectedGridItem = gridItem;
        }
        
        
    }

    public void SwapPlaces(GridItem fromItem, GridItem toItem, bool checkForEvents = true, bool isManualSwap = false)
    {
        var fromItemPos = fromItem.transform.position;
        var allSidePositions = new List<GridItem>();
        foreach (EnumGridDirection directions in Enum.GetValues(typeof(EnumGridDirection)))
        {
            allSidePositions.Add(GetSidePosition(directions, fromItem.GetPosition()));
        }
        //Check if tapped is beside
        if (isManualSwap)
        {
            if (allSidePositions.Contains(toItem) == false)
            {
                return;
            }
        }
     

        var fromItemPosIndex = fromItem.GetPosition();
        var fromItemName = fromItem.name;
        fromItem.transform.DOMove(toItem.transform.position, 0.4f).SetEase(Ease.OutBack);
        toItem.transform.DOMove(fromItemPos, 0.4f).SetEase(Ease.OutBack);
        _gridItemList[toItem.GetPosition()] = fromItem;
        _gridItemList[fromItemPosIndex] = toItem;
        fromItem.SetPosition(toItem.GetPosition());
        toItem.SetPosition(fromItemPosIndex);
        fromItem.name = toItem.name;
        toItem.name = fromItemName;
        fromItem.SelectDeselectItem(false);
        toItem.SelectDeselectItem(false);
        _swaps += 1;
        SetGridItemSide();
        _gameManager.SetSwaps(_swaps);
        if (checkForEvents)
        {
            StopCoroutine("CheckForConditionDelay");
            StartCoroutine("CheckForConditionDelay");
          
            //CheckForMatchCondition();
        }
     
    }

    private void CheckForMatchCondition()
    {
        var matchPotential = new List<GridItem>();
        var matchedList = new List<GridItem>();
        for (int i = 0; i < _gridItemList.Count; i++)
        {
            var colorsBeside = _gridItemList[i].GetAllSameColorBeside();
            matchPotential.Clear();
            matchPotential.Add(_gridItemList[i]);
            for (int j = 0; j < colorsBeside.Count; j++)
            {

                matchPotential.Add(colorsBeside[j]);
                if (matchPotential.Count >= _conditionToMatch)
                {
                    break;
                }

                var colorBeside = colorsBeside[j].GetColorBeside(matchPotential);
                if (colorBeside != null && matchPotential.Contains(colorBeside) ==false)
                {
                    matchPotential.Add(colorBeside);
                }
            }
             if (matchPotential.Count >= _conditionToMatch)
             {
                if (matchedList.Contains(_gridItemList[i])==false)
                {
                    matchedList.Add(_gridItemList[i]);
                }
                continue;
             }
        }

        if (matchedList.Count > 0)
        {
          
            for (int i = 0; i < matchedList.Count; i++)
            {
                matchedList[i].Match();
                _score += 10;
                _scoreText.text = _score.ToString();
                _gameManager.SetScore(_score);
            }
            StopCoroutine("StartFillMatchDelay");
            StartCoroutine("StartFillMatchDelay");
        }
        
        

    }

    private IEnumerator CheckForConditionDelay()
    {
        yield return new WaitForSeconds(1.5f);
        CheckForMatchCondition();
    }

    private IEnumerator StartFillMatchDelay()
    {
        yield return new WaitForSeconds(1f);
        FillMatched();
    }


    private void FillMatched()
    {

        var topGridHalfIndex = (_gridItemList.Count / 2) - 1;
        for (int i = topGridHalfIndex; i >= 0; i--)
        {
            FillEmpty(_gridItemList[i], EnumGridDirection.North); 
        }

        for (int i = topGridHalfIndex; i < _gridItemList.Count; i++)
        {
            FillEmpty(_gridItemList[i], EnumGridDirection.South);
        }
        SetGridItemSide();
    }
    private void FillEmpty(GridItem gridItem, EnumGridDirection direction)
    {
        if (gridItem.GetIsMatched() == false)
        {
            return;
        }
        var swapping = false;
        var nextInColumn = gridItem.GetGridItemFromDirection(direction);

        while (nextInColumn != null)
        {
            if (nextInColumn.GetIsMatched() == false && nextInColumn.IsReplacing==false)
            {
                var nextColumnPosition = nextInColumn.GetPosition();
                var nextColumnName = nextInColumn.name;
                nextInColumn.transform.DOMove(_gridPositions[gridItem.GetPosition()], 0.4f).SetEase(Ease.OutBack);
                _gridItemList[gridItem.GetPosition()] = nextInColumn;
                _gridItemList[nextColumnPosition] = gridItem;
                nextInColumn.name = gridItem.name;
                gridItem.name = nextColumnName;
               
                nextInColumn.SetPosition(gridItem.GetPosition());
                gridItem.SetPosition(nextColumnPosition);
                nextInColumn.IsReplacing = true;
                nextInColumn.SetIsMatched(true);
                //drops item 
                swapping = true;
                break;

            }
            //Loops until end of the row
            nextInColumn = nextInColumn.GetGridItemFromDirection(direction);
        }

        if (swapping)
        {
            return;
        }


        var itemGrid = SpawnGridItem(gridItem.GetPosition());

        var offset = direction == EnumGridDirection.North ? (_playableSize * 100) : -(_playableSize * 100);
        var fromPos = _gridPositions[gridItem.GetPosition()];
        itemGrid.transform.position =new Vector2(fromPos.x, fromPos.y + offset);
        itemGrid.transform.DOMove(fromPos, 1.2f);
        itemGrid.FadeIn();
        Destroy(gridItem.gameObject);
        _gridItemList[itemGrid.GetPosition()] = itemGrid;
        StopCoroutine("CheckForConditionDelay");
        StartCoroutine("CheckForConditionDelay");
    }
}
