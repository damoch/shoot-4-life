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
        private bool _isInGameMenuOpen;

        [SerializeField]
        protected List<StateBase> _states;

        [SerializeField]
        private StateBase _activeState;
        private float _normalTimeScale;

        private void Start()
        {
            if (!IsMainMenu)
            {
                CloseInGameMenu();
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

        public bool IsInGameMenuOpen { get => _isInGameMenuOpen; set => _isInGameMenuOpen = value; }

        public void GoToState(string stateId)
        {
            _activeState?.SetStateActive(false);
            _activeState = _states.FirstOrDefault(x => x.StateID == stateId);
            _activeState?.SetStateActive(true);
        }

        public void CloseInGameMenu()
        {
            _states.ForEach(x => x.SetStateActive(false));
            _activeState = null;
            Time.timeScale = 1;
            _isInGameMenuOpen = false;
        }

        public void OpenInGameMenu()
        {            
            _normalTimeScale = Time.timeScale;
            Time.timeScale = 0;
            GoToState("MainMenu");
            _isInGameMenuOpen = true;
        }

        public void ExitToMenuButtonClicked()
        {
            //temporary
            SceneManager.LoadScene("mainMenu");
        }
    }
}

