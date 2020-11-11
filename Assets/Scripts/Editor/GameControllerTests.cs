using NUnit.Framework;
using Assets.Scripts.Game.Controllers.Implementations;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class GameControllerTests
    {
        [Test]
        public void NextTurnTest()
        {
            var ob = new GameObject();
            var controller = ob.AddComponent<GameController>();
            controller.Initialize();
            Assert.AreEqual(1, controller.GameState.TurnNumber);
            controller.NextTurn();
            Assert.AreEqual(2, controller.GameState.TurnNumber);
        }
    }
}
