public class GameSetting 
{
    public int NumberOfItems;
    public int NumberOfColors;
    public float CurrentTimeSetting = 60;

    public void Set(int numberOfItems, int numberOfColors, float time)
    {
        NumberOfItems = numberOfItems;
        NumberOfColors = numberOfColors;
        CurrentTimeSetting = time;
    }


}