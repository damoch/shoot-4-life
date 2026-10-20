namespace Assets.Scripts.IO.Abstractions
{
    public interface ISaveLoadManager
    {
        void SaveGame();
        void LoadGame(string name);
    }
}
