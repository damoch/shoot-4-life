namespace Assets.Scripts.Game.State.Abstracts
{
    public interface IGameState
    {
        int TurnNumber { get; set; }
        string GetAboutString();
    }
}
