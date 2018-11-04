using Assets.Scripts.Actors;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Controllers
{
    public class PlayableActorController : MonoBehaviour
    {
        #region Private variables
        [SerializeField]
        private List<Actor> _playableActors;

        [SerializeField]
        private KeyCode _upKey;

        [SerializeField]
        private KeyCode _downKey;

        [SerializeField]
        private KeyCode _leftKey;

        [SerializeField]
        private KeyCode _rightKey;

        private Dictionary<KeyCode, Commands> _keyCodesToCommands;

        #endregion


        private void Start()
        {
            _keyCodesToCommands = new Dictionary<KeyCode, Commands>
            {
                { _upKey, Commands.Up },
                { _downKey, Commands.Down },
                { _leftKey, Commands.Left },
                { _rightKey, Commands.Right }
            };
        }

        private void FixedUpdate()
        {
            var selected = _playableActors.Where(x => x.IsSelected).ToList();

            if (selected.Count() > 0)
            {
                selected.ForEach(x => x.LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            }

            if (!Input.anyKey)
            {
                return;
            }
            for (int i = 0; i < _playableActors.Count(); ++i)
            {
                if (Input.GetKeyDown("" + (i + 1)))
                {
                    _playableActors[i].IsSelected = !_playableActors[i].IsSelected;
                }
            }

            var pressedCommand = _keyCodesToCommands.Keys.FirstOrDefault(x => Input.GetKey(x));

            if (pressedCommand != KeyCode.None)
            {
                _playableActors.Where(y => y.IsSelected).ToList().ForEach(x => x.GetCommand(_keyCodesToCommands[pressedCommand]));
            }
        }
    }
}

