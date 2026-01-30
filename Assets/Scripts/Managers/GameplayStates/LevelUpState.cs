using Items;
using Managers.Scenes;
using UI.Gameplay;
using UnityEngine;

namespace Managers.StateMachine
{
    public class LevelUpState : MonoBehaviour, IState
    {
        [SerializeField] private LevelUpUI _levelUpUI;
        [SerializeField] private ItemDatabase _itemDatabase;
        [SerializeField] private GameplayManager _gameplayManager;
        
        private void OnEnable()
        {
            if (_levelUpUI != null)
            {
                _levelUpUI.OnChoiceProcessed += ResumeGame;
            }
            else
            {
                Debug.LogWarning("LevelUpUI reference missing in LevelUpState. This state might not work correctly if the UI is not assigned.", this);
            }
        }

        private void OnDisable()
        {
            if (_levelUpUI != null)
            {
                _levelUpUI.OnChoiceProcessed -= ResumeGame;
            }
        }

        public void Enter()
        {
            // Pause gameplay
            Time.timeScale = 0f;
            
            // Initialize ui
            GameManager.Instance.SwitchToUIActionMap();
            _levelUpUI.gameObject.SetActive(true);
            
            // Create level up choices
            var choices = _itemDatabase.GetLevelUpChoices();
            _levelUpUI.ShowChoices(choices);
        }

        public void Exit()
        {
            // Resume gameplay and close UI
            Time.timeScale = 1f;
            _levelUpUI.gameObject.SetActive(false);
            GameManager.Instance.SwitchToPlayerActionMap();
        }

        private void ResumeGame()
        {
            _gameplayManager.ChangeState(GameplayState.Active);
        }
    }
}