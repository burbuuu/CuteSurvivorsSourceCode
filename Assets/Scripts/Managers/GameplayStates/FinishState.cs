using System.Data.SqlTypes;
using System.Security;
using Enemies;
using Enemies.Runtime;
using Items;
using Managers.Scenes;
using Player.Runtime;
using UI.Gameplay;
using UnityEngine;

namespace Managers.StateMachine
{
    public class FinishState : MonoBehaviour, IState
    {
        [SerializeField] private GameplayManager _gameplayManager;
        
        [SerializeField] private CanvasGroup _finishCanvas;
        
        #region Implementation of IState
        public void Enter()
        {        
            // Switch to UI input action map
            GameManager.Instance.SwitchToUIActionMap();

            // Setup finish canvas
            _finishCanvas.gameObject.SetActive(true);
            _finishCanvas.alpha = 1f;
            _finishCanvas.blocksRaycasts = true;
            _finishCanvas.interactable = true;
            var finishUI = FindAnyObjectByType<FinishMenuUI>();
            finishUI.FetchFinishDataAndDisplayMenu(); // Get display data and display it
            
            // Stop spawner and stop enemies
            EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
            spawner.StopSpawning();
            EnemyManager.Instance.StopPathfinding();
            
            // Stop equiped weapons
            var inventory = FindFirstObjectByType<Inventory>();
            var equipedWeapons = inventory.EquippedWeapons;
            foreach (var weapon in equipedWeapons)
            {
                // Disable component
                weapon.enabled = false;
            }
            
            // Disable players combat
            PlayerController.Instance.DisableCombat();
            
            // Play gameover/win music
            SoundManager.Instance.StopMusic();
            if (_gameplayManager.IsGameOver)
            {
                SoundManager.Instance.PlayMusic("GameOver", false);
            }
            else
            {
                SoundManager.Instance.PlayMusic("Win", false);
            }
        }
        public void Exit()
        {
            
        }
        #endregion
    }
}