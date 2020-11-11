using Assets.Scripts.UI.Abstracts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Assets.Scripts.UI.Implementations
{
    public class MenuController : MonoBehaviour, IMenuController
    {
        public bool IsMainMenu { get { return SceneManager.GetActiveScene().name == "mainMenu"; } }

        [SerializeField]
        protected List<StateBase> _states;

        [SerializeField]
        private StateBase _activeState;

        private void Start()
        {
            if (!IsMainMenu)
            {
                _states.ForEach(x => x.SetStateActive(false));
            }
        }

        public StateBase ActiveState
        {
            get
            {
                return _activeState;
            }
            set
            {
                _activeState = value;
            }
        }

        public void GoToState(string stateId)
        {
            _activeState?.SetStateActive(false);
            _activeState = _states.FirstOrDefault(x => x.StateID == stateId);
            _activeState?.SetStateActive(true);
        }
    }
}

