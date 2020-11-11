using Assets.Scripts.UI.Enums;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Runtime
{
    class GameStartupManager
    {
        [RuntimeInitializeOnLoadMethod]
        private static void StartupOptionsLoader()
        {
            if (!EditorApplication.isPlaying)
            {
                Application.logMessageReceived += HandleConsole;
            }

            if (PlayerPrefs.HasKey(OptionNames.Fullscreen.ToString()))
            {
                var fs = PlayerPrefs.GetInt(OptionNames.Fullscreen.ToString()) == 1;
                Screen.fullScreen = fs;
            }
        }

        private static void HandleConsole(string condition, string stackTrace, LogType type)
        {
            //TODO: logging
        }
    }
}
