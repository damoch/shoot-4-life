using Assets.Scripts.UI.Abstracts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.Implementations.States
{
    public class MainMenuState : StateBase
    {
        public void NewGameButtonClicked()
        {
            //temporary
            SceneManager.LoadScene("test");
        }

        public void ExitGameButtonClicked()
        {
            Application.Quit();
        }
    }
}
