using Managers.Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MainMenuManager _mainMenuManager;
        
        [Header("Buttons")]
        [SerializeField] private Button _configureRun;
        [SerializeField] private Button _options;
        [SerializeField] private Button _credits;
        [SerializeField] private Button _quit;

        private void Awake()
        {
            _configureRun.onClick.AddListener(OnConfigureRunButton);
            _options.onClick.AddListener(OnOptionsButton);
            _credits.onClick.AddListener(OnCreditsButton);
            _quit.onClick.AddListener(OnQuitButton);
        }

        private void OnConfigureRunButton()
        {
            _mainMenuManager.ShowGameSelectionMenu();
        }
        
        private void OnOptionsButton()
        {
            _mainMenuManager.ShowOptions();
        }

        private void OnCreditsButton()
        {
            GameManager.Instance.TransitionToScene( "Credits");
        }

        private void OnQuitButton()
        {
            Application.Quit();
        }
        
        
    }
}