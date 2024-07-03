using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class PreGameView : MonoBehaviour
{
    [SerializeField]
    private Button _startGame;
    [SerializeField]
    private InputField _boardSizeXInputField;
    [SerializeField]
    private InputField _numberOfColorsInputField;
    [Inject]
    private IGameManager _gameManager;

    // Start is called before the first frame update
    private void Awake()
    {
        _startGame.onClick.AddListener(OnClickStartGame);
    }

    private void OnClickStartGame()
    {
        int xInputValue = 0;
        int yInputValue = 0;
        int.TryParse(_boardSizeXInputField.textComponent.text, out xInputValue);
        int.TryParse(_numberOfColorsInputField.textComponent.text, out yInputValue);
        if(xInputValue % 2 !=0 || xInputValue <4 || xInputValue > 8)
        {
            return;
        }
        if (yInputValue < 3 || yInputValue > 5)
        {
            return;
        }
        var gameSetting = new GameSetting();
        gameSetting.CurrentTimeSetting = 60;
        gameSetting.NumberOfItems = xInputValue;
        gameSetting.NumberOfColors = yInputValue;
        _gameManager?.SetGameSetting(gameSetting);
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
