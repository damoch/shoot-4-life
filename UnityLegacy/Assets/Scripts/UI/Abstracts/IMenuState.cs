namespace Assets.Scripts.UI.Abstracts
{
    interface IMenuState
    {
        string StateID { get; }
        void SetStateActive(bool active);
    }
}
