using UnityEngine;
using NUnit.Framework;
using Assets.Scripts.UI.Implementations;
using Assets.Scripts.UI.Abstracts;
using System.Linq;

namespace Assets.Scripts.Editor
{
    public class MainMenuTests
    {

        [Test]
        public void MenuControllerCreationTest()
        {
            var gObject = new GameObject();
            gObject.AddComponent<MockMenuController>();
            Assert.That(gObject.GetComponent<MockMenuController>() != null);
        }

        [Test]
        public void GoToStateTest()
        {
            var gObject = new GameObject();
            var contr = gObject.AddComponent<MockMenuController>();

            var stateObject = new GameObject();
            var stat = stateObject.AddComponent<MockState>();

            var id = "test0";
            stat.SetID(id);

            contr.LoadStates();
            contr.GoToState(id);
            Assert.That(contr.ActiveState == stat);
        }
    }

    class MockState : StateBase
    {
        internal void SetID(string id)
        {
            _stateID = id;
        }
    }

    class MockMenuController : MenuController
    {
        internal void LoadStates()
        {
            _states = FindObjectsOfType<StateBase>().ToList();
        }
    }
}
