using UnityEngine;

namespace Managers.Scenes
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Menu Elements")]
        [SerializeField] private CanvasGroup startMenu;
        [SerializeField] private CanvasGroup gameSelectionMenu;
        [SerializeField] private CanvasGroup optionsMenu;
        [SerializeField] private CanvasGroup characterSelectionMenu;
        [SerializeField] private CanvasGroup stageSelectionMenu;

        private void OnValidate()
        {
            if (startMenu == null || gameSelectionMenu == null || optionsMenu == null)
            {
                Debug.LogWarning($"[MainMenuManager] {gameObject.name} is missing UI references!", this);
            }
        }

        private void Start()
        {
            ShowStartMenu();
        
            // Clear the starting character
            GameManager.Instance.ClearStartingCharacter();
        }
    

        #region Menu Navigation Helpers
        public void ShowStartMenu()
        {
            startMenu.gameObject.SetActive(true);
            gameSelectionMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(false);
        }

        public void ShowOptions()
        {
            startMenu.gameObject.SetActive(false);
            gameSelectionMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(true);
        }

        public void ShowGameSelectionMenu()
        {
            startMenu.gameObject.SetActive(false);
            gameSelectionMenu.gameObject.SetActive(true);
            optionsMenu.gameObject.SetActive(false);
            
            characterSelectionMenu.gameObject.SetActive(true);
            stageSelectionMenu.gameObject.SetActive(false);
        }
        #endregion
    }
}