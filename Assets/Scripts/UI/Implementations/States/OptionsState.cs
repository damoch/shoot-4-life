using Assets.Scripts.UI.Abstracts;
using Assets.Scripts.UI.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Implementations.States
{
    public class OptionsState : StateBase
    {

        [SerializeField]
        private Toggle _fullscreenToggle;

        [SerializeField]
        private Button _applyButton;


        private Dictionary<OptionNames, string> _options;
        private readonly Dictionary<OptionNames, string> _defaultOptions = new Dictionary<OptionNames, string> { { OptionNames.Fullscreen, "false" } };


        private void OnEnable()
        {
            LoadOptions(false);
        }

        public void LoadOptions(bool setDefaults)
        {
            _applyButton.interactable = false;
            _options = new Dictionary<OptionNames, string>();
            foreach(var key in _defaultOptions.Keys)
            {
                var keyName = key.ToString();
                _options.Add(key, !setDefaults && PlayerPrefs.HasKey(keyName) ? PlayerPrefs.GetString(keyName) : _defaultOptions[key]);
            }
            SetupControls();
        }

        private void SetupControls()
        {
            var res = bool.Parse(_options[OptionNames.Fullscreen]);
            _fullscreenToggle.isOn = res;
        }

        public void ApplyOptions()
        {
            var fullScreen = bool.Parse(_options[OptionNames.Fullscreen]);
            Screen.fullScreen = fullScreen;
            PlayerPrefs.SetString(OptionNames.Fullscreen.ToString(), fullScreen.ToString());

            _applyButton.interactable = false;

        }

        public void SetFullscreen(bool value)
        {
            _options[OptionNames.Fullscreen] = _fullscreenToggle.isOn.ToString();
            OptionChanged();
        }

        private void OptionChanged()
        {
            _applyButton.interactable = true;
        }
    }
}
