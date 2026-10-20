namespace Assets.Scripts.UI.Abstracts
{
    public interface IMenuController
    {
        StateBase ActiveState { get; set; }
        bool IsMainMenu { get; }

        void GoToState(string stateId);
    }
}