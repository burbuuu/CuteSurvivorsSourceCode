using System.Collections.Generic;
using Enemies.Data;
using Enemies.Runtime;
using Player.Runtime;
using Stages.Data;
using UnityEngine;

namespace Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }
        
        // Enemy pool
        [SerializeField] private EnemyController prefab;
        [SerializeField] private int maxEnemies = 300;
        private EnemyController[] _enemies;
        private Stack<EnemyController> _pool;
        
        // Enemy queue for pathfinding
        private List<EnemyController> _activeEnemies;
        private int _lastEnemyUpdated = 0;
        [SerializeField] private int updatesPerFrame = 5; // Number of enemies that will update per frame
        
        private bool isPathfindingEnabled = true;
        
        #region Initialization
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            
            // Create and preallocate _pool and _activeEnemies
            _pool = new Stack<EnemyController>(maxEnemies);
            _activeEnemies = new List<EnemyController>(maxEnemies);
            
            // Instantiate enemy pool
            _enemies = new EnemyController[maxEnemies];
            for (int i = 0; i < maxEnemies; i++)
            {
                EnemyController enemy = Instantiate(prefab, transform);
                enemy.gameObject.SetActive(false);
                _pool.Push(enemy);
            }
        }
        #endregion
        
        #region Spawning

        // Spawns an enemy at the given position
        public EnemyController Spawn(EnemyData data, Vector2 spawnPosition)
        {
            if (_pool.Count == 0)
            {
                Debug.Log("[EnemyManager]: Failed to spawn enemy, no more enemies available in the pool.");
                return null;
            }
            
            // Take enemy from pool
            EnemyController enemy = _pool.Pop();
            _activeEnemies.Add(enemy);
            
            enemy.gameObject.SetActive(true);
            enemy.OnSpawn(data, spawnPosition);
            return enemy;
        }
        
        public void Despawn(EnemyController enemy)
        {
            int index = _activeEnemies.IndexOf(enemy);
            if (index < 0) return; // Enemy already despawned or not in active list

            if (index <= _lastEnemyUpdated) _lastEnemyUpdated--;

            enemy.OnDespawn();
            _activeEnemies.RemoveAt(index);
            enemy.gameObject.SetActive(false);
            _pool.Push(enemy);
        }
        #endregion

        
        void Update()
        {
            if (!isPathfindingEnabled) return;
            
            if (_activeEnemies.Count == 0) return;
            for (int i = 0; i < updatesPerFrame; i++)
            {
                // Safety: wrap around if we reach the end of the list
                if (_lastEnemyUpdated >= _activeEnemies.Count)
                {
                    _lastEnemyUpdated = 0;
                }

                _activeEnemies[_lastEnemyUpdated].UpdatePathfinding();
        
                _lastEnemyUpdated++;
        
                // Stop if we've looped through everyone in a single frame
                if (_lastEnemyUpdated >= _activeEnemies.Count && i < updatesPerFrame - 1)
                    break;
            }
        }

        #region Get target enemies

        public List<Vector2> GetClosestEnemiesPosition(int numEnemies)
        {
            Vector2 playerPosition = PlayerController.PlayerTransform.position;
            List<Vector2> positions = new List<Vector2>();

            if (_activeEnemies.Count == 0) return positions;

            // Sort active enemies by distance to player
            List<EnemyController> sortedEnemies = new List<EnemyController>(_activeEnemies);
            sortedEnemies.Sort((a, b) =>
            {
                float distA = Vector2.Distance(playerPosition, a.transform.position);
                float distB = Vector2.Distance(playerPosition, b.transform.position);
                return distA.CompareTo(distB);
            });

            int count = Mathf.Min(numEnemies, sortedEnemies.Count);
            for (int i = 0; i < count; i++)
            {
                positions.Add(sortedEnemies[i].transform.position);
            }

            return positions;
        }

        public List<Vector2> GetRandomEnemiesPosition(int numEnemies, float radius)
        {
            Vector2 playerPosition = PlayerController.PlayerTransform.position;
            List<Vector2> positions = new List<Vector2>();

            if (_activeEnemies.Count == 0) return positions;

            // Copy active enemies to a temporary list to pick from
            List<EnemyController> candidates = new List<EnemyController>(_activeEnemies);
            float radiusSq = radius * radius;

            while (positions.Count < numEnemies && candidates.Count > 0)
            {
                int randomIndex = Random.Range(0, candidates.Count);
                EnemyController enemy = candidates[randomIndex];
                
                // Use square distance for performance
                if (Vector2.SqrMagnitude((Vector2)enemy.transform.position - playerPosition) <= radiusSq)
                {
                    positions.Add(enemy.transform.position);
                }
                
                // Remove to avoid picking the same enemy twice (either because it's added or because it's out of range)
                candidates.RemoveAt(randomIndex);
            }

            return positions;
        }

        #endregion
        
        #region Stop enemies

        public void StopPathfinding()
        {
            isPathfindingEnabled = false;
        }
        
        public void StartPathfinding()
        {
            isPathfindingEnabled = true;
        }
        
        #endregion
    }
}