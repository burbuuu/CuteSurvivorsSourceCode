using Managers.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class FinishMenuUI : MonoBehaviour
    {
        private GameplayManager _gameplayManager = null;
        [Header("References")]
        [SerializeField] private TMP_Text _gameOverText;
        [SerializeField] private Button _returnToMainMenuButton;
        
        private bool gameOver;
        
        
        private void Awake()
        {
            _gameplayManager = FindFirstObjectByType<GameplayManager>();
            if (_gameplayManager == null)
            {
                Debug.LogError("[FinishMenuUI] GameplayManager not found.");
            }

            if (_returnToMainMenuButton == null)
            {
                Debug.LogError("[FinishMenuUI] ReturnToMainMenuButton not found.");
            }
            _returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
            
        }

        public void FetchFinishDataAndDisplayMenu()
        {
            gameOver = _gameplayManager.IsGameOver;

            if (gameOver)
            {
                _gameOverText.text = "Game Over!";
            }
            else
            {
                _gameOverText.text = "Stage Clear!";
            }
        }

        private void ReturnToMainMenu()
        {
            GameManager.Instance.TransitionToScene("MainMenu");
        }
        
        
    }
}
