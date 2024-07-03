using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class GameOver : MonoBehaviour
{
    [Inject]
    private IGameManager _gameManager;
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _swaps;
    [SerializeField]
    private Button _playAgainButton;
    // Start is called before the first frame update

    void Start()
    {
        _scoreText.text = _gameManager.GetScore().ToString();
        _swaps.text = _gameManager.GetSwaps().ToString();
        _playAgainButton.onClick.AddListener(OnClickPlayAgain);
    }

    private void OnClickPlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}
