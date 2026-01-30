using System.Collections.Generic;
using Managers.StateMachine;
using Player.Runtime;
using Stages.Data;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Enemies.Data;

namespace Enemies
{

    
    public class EnemySpawner : MonoBehaviour
    {
        private struct EventSpawnInstance
        {
            public float Time;
            public EventSpawnsData Data;
        }
        
        private struct SpawnStamp
        {
            public int Minute;
            public float TimeInMinute;
            public string Source;
        }
        
        #region Definitions
        [Header("Debug")]
        [SerializeField] private bool _enableSpawnLogging = false;
        
        [Header("Spawn Settings")]
        [SerializeField] private float _spawnRadius = 15f;
        
        // Dependencies
        private StageData _stageData;
        private ActiveState _activeState;
        
        // State
        private bool _isSpawning;
        
        // Spawning data and control timers
        [SerializeField] private int _currentMinute; // Current wave index
        [SerializeField] private MinuteWaveData _currentWaveData;
        [SerializeField] private float _minuteTimer; // Timer in seconds, resets every minute
        
        
        // We use a Queue for event times because they are ordered chronologically
        private Queue<EventSpawnInstance> _eventSchedule = new();
        
        // Timer state for regular spawns
        [SerializeField] private Dictionary<RegularSpawnsData, float> _regularSpawnTimers = new();
        private Dictionary<RegularSpawnsData, float> _nextRandomDelay = new();
        [SerializeField] private Dictionary<RegularSpawnsData, int> _regularSpawnsRemaining = new();
        
        #endregion

        #region Public Initialization / Start / Stop API

        public void Initialize(StageData stageData, ActiveState activeState)
        {
            _stageData = stageData;
            _activeState = activeState;

            _currentMinute = -1;
            _currentWaveData = null;
            
            ResetRuntimeData();
        }

        private void ResetRuntimeData()
        {
            _eventSchedule.Clear();
            _regularSpawnTimers.Clear();
            _nextRandomDelay.Clear();
            _regularSpawnsRemaining.Clear();
        }

        public void StopSpawning() => _isSpawning = false;
        public void StartSpawning() => _isSpawning = (_stageData != null && _activeState != null);
        
        #endregion

        #region Spawn logic
        private void Update()
        {
            if (!_isSpawning) return;

            // Get the stage timer
            float time = _activeState.GameplayTimer;
            int minute = Mathf.FloorToInt(time / 60.0f);

            if (minute != _currentMinute) SetupMinute(minute);

            // Update the timer
            _minuteTimer = time % 60f;
            
            if (_currentWaveData == null) return;
            
            HandleRegularSpawns();
            HandleEventSpawns();
        }
        
        // This method is responsible for spawning enemies based on the current wave data
        private void HandleRegularSpawns()
        {
            foreach (var spawnData in _currentWaveData.regularSpawns)
            {
                if (_regularSpawnsRemaining[spawnData] <= 0) continue;

                _regularSpawnTimers[spawnData] += Time.deltaTime;

                if (_regularSpawnTimers[spawnData] >= _nextRandomDelay[spawnData])
                {
                    // Spawn logic
                    Vector2 spawnPos = GetRandomSpawnPosition();
                    SpawnEnemy(spawnData.enemyType, spawnPos, "Regular");
                    
                    // Decrement and Reset
                    _regularSpawnsRemaining[spawnData]--;
                    _regularSpawnTimers[spawnData] = 0f;
                    
                    // Set the next randomized delay
                    _nextRandomDelay[spawnData] = CalculateRandomInterval(spawnData);
                }
            }
        }

        private void HandleEventSpawns()
        {
            if (_eventSchedule.Count > 0 && _minuteTimer >= _eventSchedule.Peek().Time)
            {
                var instance = _eventSchedule.Dequeue();
                
                // For events, we spawn the full count at once
                
                // TODO: Implement spawn patterns instead of the randomposition
                for (int i = 0; i < instance.Data.spawnCount; i++)
                {
                    SpawnEnemy(instance.Data.enemyType, GetRandomSpawnPosition(), "Event");
                }
            }
        }
        
        private void SpawnEnemy(EnemyData enemyType, Vector2 position, string source)
        {
            var enemy = EnemyManager.Instance.Spawn(enemyType, position);
            if (enemy == null) return;

            if (!_enableSpawnLogging) return;

            SpawnStamp stamp = new SpawnStamp
            {
                Minute = _currentMinute,
                TimeInMinute = _minuteTimer,
                Source = source
            };

            Debug.Log(
                $"[EnemySpawn] " +
                $"Enemy={enemy.name} | " +
                $"Source={stamp.Source} | " +
                $"Minute={stamp.Minute} | " +
                $"t={stamp.TimeInMinute:F2}s"
            );
        }
        
        #endregion
        
        #region SetUp minute logic
        
        private void SetupMinute(int minute)
        {
            ResetRuntimeData();
            _currentMinute = minute;
            
            if (minute < 0 || minute >= _stageData.minuteWaves.Count)
            {
                _currentWaveData = null;
                return;
            }

            _currentWaveData = _stageData.minuteWaves[minute];

            // Setup Regular Spawns
            foreach (var reg in _currentWaveData.regularSpawns)
            {
                int totalToSpawn = Random.Range(reg.minSpawnsPerMinute, reg.maxSpawnsPerMinute + 1);
                _regularSpawnsRemaining[reg] = totalToSpawn;
                _regularSpawnTimers[reg] = 0f;
                // Initialize first random delay
                _nextRandomDelay[reg] = CalculateRandomInterval(reg);
            }

            SetUpEventSpawns();
        }
        private float CalculateRandomInterval(RegularSpawnsData data)
        {
            if (_regularSpawnsRemaining[data] <= 0) return 999f;

            // Calculate how much time is left in the minute
            float timeLeft = Mathf.Max(1f, 60f - _minuteTimer);
            // Average time per remaining enemy
            float avg = timeLeft / _regularSpawnsRemaining[data];
            
            // Randomize the interval (between 50% and 150% of the average)
            return Random.Range(avg * 0.5f, avg * 1.5f);
        }

        private void SetUpEventSpawns()
        {
            List<EventSpawnInstance> tempInstances = new List<EventSpawnInstance>();
            foreach (var eventData in _currentWaveData.eventSpawns)
            {
                int eventCount = Random.Range(eventData.minEventsPerMinute, eventData.maxEventsPerMinute + 1);
                for (int i = 0; i < eventCount; i++)
                {
                    tempInstances.Add(new EventSpawnInstance { Time = Random.Range(0f, 58f), Data = eventData });
                }
            }
            tempInstances.Sort((a, b) => a.Time.CompareTo(b.Time));
            foreach (var inst in tempInstances) _eventSchedule.Enqueue(inst);
        }

        private Vector2 GetRandomSpawnPosition()
        {
            if (PlayerController.Instance == null) return (Vector2)transform.position;

            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _spawnRadius;
            
            return (Vector2)PlayerController.Instance.transform.position + offset;
        }
    
        #endregion
    }
}