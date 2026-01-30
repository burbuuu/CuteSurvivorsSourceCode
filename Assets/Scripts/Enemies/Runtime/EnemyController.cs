using System;
using System.Collections;
using System.Collections.Generic;
using Drops;
using Enemies.Data;
using FX;
using Player.Runtime;
using UI.Gameplay;
using UnityEngine;

namespace Enemies.Runtime
{
    public class EnemyController : MonoBehaviour
    {
        #region Declarations

        // Public events
        public static event Action<string> OnEnemyKilled;
        public static event Action<string, float> OnEnemyDamageTaken;
        
        // Enemy data
        private EnemyData _data;

        // Runtime data
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _lastHitTime; // Last time the enemy hit the player

        private bool _isActive;
        
        // Components
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        
        
        [Header("Pick-ups")]
        // Pick-ups: General to all enemies for now, do it on the inspector
        [SerializeField] List<PickupProbability> _pickupProbabilities = new List<PickupProbability>();
        
        #endregion

        #region Initialization
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_rigidbody == null || _animator == null || _spriteRenderer == null)
            {
                Debug.LogWarning("[EnemyController]: A Component is missing.");
                enabled = false;
                return;
            }
        }
        #endregion

        #region Spawning and destruction
        // This method spawns an enemy position, reading its data and applying the spawn position 
        public void OnSpawn(EnemyData data, Vector2 spawnPosition)
        {
            if (data == null)
            {
                Debug.LogError("[EnemyController]: Enemy data on spawn is null.");
                return;
            }
            this._data = data;
            transform.position = spawnPosition;
            
            if (data.AnimatorOverrideController != null)
            {
                _animator.runtimeAnimatorController = data.AnimatorOverrideController;
            }
            _currentHealth = data.Health;
            _isActive = true;
        }

        public void OnDespawn()
        {
            _isActive = false;
        }
        
        private void OnEnemyDeath()
        {
            //  Spawn pickup
            SpawnDrops();
            
            _isActive = false;
            _animator.SetTrigger("Death");
            StopMovement();
            
            StartCoroutine(DespawnAfterDeath());
        }

        private IEnumerator DespawnAfterDeath()
        {
            yield return new WaitForSeconds(
                _animator.GetCurrentAnimatorStateInfo(0).length
            );

            EnemyManager.Instance.Despawn(this);
        }
        
        #endregion
        
        #region Combat
        private bool CanDealDamage()
        {
            if (!_isActive) return false;
            return Time.time >= _lastHitTime + _data.DamageCooldown;
        }

        private void DealDamage()
        {
            PlayerController.Instance.Stats.ApplyDamage(_data.Damage);
            _lastHitTime = Time.time;
        }

        public void TakeDamage(float damage, string damageSource)
        {
            if (!_isActive) return;

            if (damage < 0)
            {
                Debug.LogWarning("[EnemyController]: Trying to take negative damage.");
                return;
            }

            _currentHealth -= damage;
            OnEnemyDamageTaken?.Invoke(damageSource, damage);
            
            _animator.SetTrigger("Hurt");
            
            // FX
            FXManager.Instance.PlayHitFX(transform.position, damage);
            
            if (_currentHealth <= 0)
            {
                OnEnemyDeath();
                OnEnemyKilled?.Invoke(damageSource);
            }
        }
        
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!CanDealDamage()) return;
            if (!collision.collider.CompareTag("Player")) return;
            if (PlayerController.Instance == null) return;

            DealDamage();
        }
        #endregion
        
        #region Movement

        public void UpdatePathfinding()
        {
            if(!_isActive) return;
            
            Transform playerTransform = PlayerController.PlayerTransform;
            if (playerTransform == null)
            {
                Debug.LogError("[Enemy controller]: PlayerTransform is null.");
                return;
            }
            
            Vector2 moveDirection = (playerTransform.position - transform.position).normalized;
            _spriteRenderer.flipX = moveDirection.x < 0; // Update facing direction
            _rigidbody.linearVelocity = moveDirection * _data.MoveSpeed;
        }
        
        public void StopMovement()
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        #endregion

        #region Enemy drops

        private void SpawnDrops()
        {
            foreach (var spawn in _pickupProbabilities)
            {
                if (spawn.prefab == null) continue;
                if (spawn.probability <= 0f) continue;

                float roll = UnityEngine.Random.value * 100f;
                if (roll <= spawn.probability)
                {
                    Vector2 offset = UnityEngine.Random.insideUnitCircle * 0.3f;
                    Instantiate(
                        spawn.prefab,
                        (Vector2)transform.position + offset,
                        Quaternion.identity
                    );
                }
            }

        }

        #endregion
        
    }
}