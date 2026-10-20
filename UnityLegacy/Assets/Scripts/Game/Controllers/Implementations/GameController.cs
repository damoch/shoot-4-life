using Assets.Scripts.Game.Controllers.Abstractions;
using Assets.Scripts.Game.State.Abstracts;
using Assets.Scripts.Game.State.Implementations;
using UnityEngine;

namespace Assets.Scripts.Game.Controllers.Implementations
{ 
    public class GameController : MonoBehaviour, IGameController
    {
        public IGameState GameState { get;  private set;}

        public void NextTurn()
        {
            GameState.TurnNumber++;
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            GameState = new GameState();
            Debug.Log(GameState.GetAboutString());
        }
    }
}
