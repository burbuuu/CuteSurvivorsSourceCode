using System;
using System.Threading;
using Enemies.Runtime;
using UnityEngine;

namespace Items
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] AnimatorOverrideController _animatorOverrideController;
        
        private Animator _animator;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;
        
        [Header("Projectile Parameters (driven by the weapon)")]
        // Parameters passed by the weapon
        [SerializeField] private float _damage;
        [SerializeField]private float _speed;
        [SerializeField]private float _duration;
        [SerializeField]private Vector2 _direction;
        [SerializeField] private bool _isCirclingAroundPlayer;
        [SerializeField]private float _currentOrbitAngle;
        [SerializeField]private float _orbitRadius;
        [SerializeField]private string _weaponOwnerName;


        void Awake()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_animator == null || _collider == null ||  _spriteRenderer == null)
            {
                Debug.LogError($"Projectile '{gameObject.name}' has a missing component.", gameObject);
                enabled = false;
                return;
            }
            
            
            if (_animatorOverrideController == null)
            {
                Debug.LogWarning($"Projectile '{gameObject.name}' has no AnimatorOverrideController assigned.", gameObject);
                return;
            }
            _animator.runtimeAnimatorController = _animatorOverrideController;
        }
        
        public void Initialize(float damage, float speed, float size, float duration, Vector2 direction,
            string weaponOwnerName, bool rotatesWithDirection = false, bool isCirclingAroundPlayer = false)
        {
            _damage = damage;
            _speed = speed;
            _duration = duration;
            _direction = direction.normalized;
            _weaponOwnerName = weaponOwnerName;
            _isCirclingAroundPlayer = isCirclingAroundPlayer;
            
            transform.localScale = Vector3.one * size;
            
            if (_isCirclingAroundPlayer)
            {
                // Calculate the initial angle and radius from the direction vector passed
                // We assume the projectile is spawned at the player's position or with an offset
                // Here we use the direction to determine the starting angle.
                _currentOrbitAngle = Mathf.Atan2(direction.y, direction.x);
                _orbitRadius = 2f * size; // Default orbit radius proportional to size, can be adjusted
            }
            else if (rotatesWithDirection)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);

            }

            Destroy(gameObject, _duration);
        }
        

        private void FixedUpdate()
        {
            if (_isCirclingAroundPlayer)
            {
                if (Player.Runtime.PlayerController.Instance == null)
                {
                    Destroy(gameObject);
                    return;
                }

                // Update angle
                _currentOrbitAngle += _speed * Time.fixedDeltaTime;

                Vector2 playerPos = Player.Runtime.PlayerController.Instance.transform.position;
                Vector2 offset = new Vector2(
                    Mathf.Cos(_currentOrbitAngle),
                    Mathf.Sin(_currentOrbitAngle)
                ) * _orbitRadius;

                transform.position = playerPos + offset;
            }
            else
            {        
                transform.position += (Vector3)(_direction * (_speed * Time.fixedDeltaTime));
            }




        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy"))
                return;

            if (collision.gameObject.TryGetComponent(out EnemyController enemy))
            {
                float damage = _damage;
                damage *= UnityEngine.Random.Range(0.85f, 1.15f); // Randomize value by 15 percent
                enemy.TakeDamage(damage, _weaponOwnerName);
            }
        }
        
    }
}