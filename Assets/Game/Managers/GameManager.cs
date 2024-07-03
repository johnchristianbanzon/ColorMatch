public class GameManager : IGameManager
{
    private GameSetting _gameSetting;
    private int _swaps;
    private int _score;

    public void SetGameSetting(GameSetting gameSetting)
    {
        _gameSetting = gameSetting;
    }

    public GameSetting GetGameSetting()
    {
        return _gameSetting;
    }

    public void SetScore(int score)
    {
        _score = score;
    }

    public void SetSwaps(int swaps)
    {
        _swaps = swaps;
    }

    public int GetScore()
    {
        return _score;
    }

    public int GetSwaps()
    {
        return _swaps;
    }
}