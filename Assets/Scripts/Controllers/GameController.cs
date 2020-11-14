using Assets.Scripts.UI.Implementations;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {        
        [SerializeField]
        private MenuController _menuController;

        // private void Start()
        // {
        //     OpenMainMenu();
        // }

        private void OpenMainMenu()
        {
            _menuController.OpenInGameMenu();
        }

        private void Update()
        {
            if(Input.GetKey(KeyCode.Escape) && !_menuController.IsInGameMenuOpen)
            {
                _menuController.OpenInGameMenu();
            }
        }
    }
}