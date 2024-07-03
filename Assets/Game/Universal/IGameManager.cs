
public interface IGameManager 
{
    public void SetGameSetting(GameSetting gameSetting);
    public GameSetting GetGameSetting();
    public void SetScore(int score);
    public void SetSwaps(int swaps);
    public int GetScore();
    public int GetSwaps();
}
