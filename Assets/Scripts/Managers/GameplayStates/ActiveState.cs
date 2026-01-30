using System;
using System.ComponentModel;
using Enemies;
using Managers.Scenes;
using Stages.Data;
using UI.Gameplay;
using UnityEngine;

namespace Managers.StateMachine
{
    public class ActiveState : MonoBehaviour, IState
    {
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private StageData _stageData;
        public StageData StageData => _stageData;
        
        // Gameplay timer
        private float _gameplayTimer;
        private float _timeLimit;
        public float GameplayTimer => _gameplayTimer;
        private bool _tickTimer; // Used on update. (Update is not part of the state machine)

        // UI
        [SerializeField] private CanvasGroup gameplayUI;
        
        
        void Awake()
        {
            _tickTimer = false;

            if (_stageData == null)
            {
                Debug.LogError("[ActiveState]: StageData not attached to GameplayManager!");
                enabled = false;
                return;
            }
            _enemySpawner = FindFirstObjectByType<EnemySpawner>();
            if (_enemySpawner == null)
            {
                Debug.LogError("[ActiveState]: EnemySpawner not found in scene!");
                enabled = false;
                return;
            }
        }

        public void StartStage()
        {
            Debug.Log("[ActiveState]: Starting stage!");
            _gameplayTimer = 0;
            _timeLimit = _stageData.StageDurationMinutes * 60; // Convert minutes to seconds
            
            // Initialize the spawner
            _enemySpawner.Initialize(_stageData, this);
            _enemySpawner.StartSpawning();
        }
        
        
        #region Implementation of IState
        public void Enter()
        {
            _tickTimer = true;
            _enemySpawner.StartSpawning();

            // Switch to player's input action map
            GameManager.Instance.SwitchToPlayerActionMap();

            // Enable and hide gameplay canvas
            gameplayUI.gameObject.SetActive(true);
            gameplayUI.alpha = 1f;
            gameplayUI.interactable = false;
            gameplayUI.blocksRaycasts = false;
            
            gameplayUI.GetComponent<GameplayUI>().InitializeUI();
        }
        public void Exit()
        {
            _tickTimer = false;
            _enemySpawner.StopSpawning();
        }
        #endregion

        
        private void Update()
        {
            if (!_tickTimer) return;
            
            _gameplayTimer += Time.deltaTime;

            if (_gameplayTimer >= _timeLimit)
            {
                var gameplayManager = gameObject.GetComponentInParent<GameplayManager>();
                if (gameplayManager == null)
                {
                    Debug.LogError("[ActiveState]: GameplayManager not found in parent.");
                    return;
                }
                gameplayManager.ChangeState(GameplayState.Finish);
            }
        }
    }
}