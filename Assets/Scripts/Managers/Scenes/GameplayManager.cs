using System;
using System.Collections.Generic;
using Managers.StateMachine;
using Player.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers.Scenes
{
    public enum GameplayState
    {
        Active,
        Pause,
        LevelUp,
        Finish
    }

    public class GameplayManager : MonoBehaviour
    {
        // Public events
        public static event Action RunStart;
    
        // Game state machine
        private Dictionary<GameplayState, IState> _stateTable;
        private IState _currentState;
        [SerializeField] private bool _isGameOver = false;
        public bool IsGameOver => _isGameOver;
    
        // States on the scene
        [Header("State machine references")]
        [SerializeField] private ActiveState _activeState;
        [SerializeField] private PauseState _pauseState;
        [SerializeField] private LevelUpState _levelUpState;
        [SerializeField] private FinishState _finishState;
        
        // Input actions
        private InputAction _pause;
        
    
        #region Event subscriptions

        void OnEnable()
        {
            // Set pause input action
            _pause = InputSystem.actions.FindAction("Pause"); // Pause is present in all input maps
            if (_pause == null)
            {
                Debug.LogError("[GameplayManager]: Pause action not found.");
                return;
            }
            _pause.performed += HandlePause;

            // Level up events
            PlayerStats.OnPlayerLevelUp += HandlePlayerLevelUp;

            // Death events
            PlayerStats.OnPlayerDeath += OnPlayerDeath;
        }

        void OnDisable()
        {
            _pause.performed -= HandlePause;

            // Level up events
            PlayerStats.OnPlayerLevelUp -= HandlePlayerLevelUp;
        
            PlayerStats.OnPlayerDeath -= OnPlayerDeath;
        }
    
        #endregion

        private void Awake()
        {
            InitializeStates();
        
            // Notify systems that the run starts
            RunStart?.Invoke();
        }

        private void InitializeStates()
        {
            _stateTable = new Dictionary<GameplayState, IState>
            {
                { GameplayState.Active, _activeState },
                { GameplayState.Pause, _pauseState},
                { GameplayState.Finish, _finishState },
                { GameplayState.LevelUp, _levelUpState }
            }; 
        }
    
        public void ChangeState(GameplayState newState)
        {
            if (!_stateTable.ContainsKey(newState)) return;

            _currentState?.Exit();
            _currentState = _stateTable[newState];
            _currentState.Enter();
        
            Debug.Log($"[GameplayManager] Entered State: {newState}");
        }


        void Start()
        {
            _activeState.StartStage();
            ChangeState(GameplayState.Active);
        }

        public void HandlePause(InputAction.CallbackContext context)
        {
            if (_currentState as ActiveState == _activeState)
            {
                ChangeState(GameplayState.Pause);
            }
            else if (_currentState as PauseState == _pauseState)
            {
                ChangeState(GameplayState.Active);
            }
        }

        #region Gameplay

        private void OnPlayerDeath()
        {
            Debug.Log("[GameplayManager]: Player Death registered;");
            _isGameOver = true;
            ChangeState(GameplayState.Finish);
        }

        private void HandlePlayerLevelUp(int level)
        {
            Debug.Log($"[GameplayManager]: Level Up! New Level: {level}");
            ChangeState(GameplayState.LevelUp);
        }
        
        
    
        #endregion

        #region Public API for ending the game
        

        public void FinishGameOnPause()
        {
            _isGameOver = true;
            ChangeState(GameplayState.Finish);
        }

        public void FinishGameOnWin()
        {
            _isGameOver = false;
            ChangeState(GameplayState.Finish);
        }
        #endregion
        
        
        #region Debug
        [ContextMenu("Debug / Enter LevelUp State")]
        void DebugEnterLevelUp()
        {
            ChangeState(GameplayState.LevelUp);
        }

        [ContextMenu("Debug / Enter Active State")]
        void DebugEnterActive()
        {
            ChangeState(GameplayState.Active);
        }
    
        [ContextMenu("Debug / Enter Pause State")]
        void DebugEnterPause()
        {
            ChangeState(GameplayState.Pause);
        }
    
        [ContextMenu("Debug / Enter Finish State")]
        void DebugEnterFinish()
        {
            ChangeState(GameplayState.Finish);
        }
        #endregion


    
    }
}