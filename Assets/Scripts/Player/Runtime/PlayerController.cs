using System;
using System.Collections;
using UnityEngine;
using Player.Data;
using UI.Gameplay;
using UnityEngine.InputSystem;


namespace Player.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        public static event Action<float> OnPlayerHeal;
        
        public static PlayerController Instance { get; private set; }
        public static Transform PlayerTransform { get; private set; }
        
        // Stats
        [SerializeField] private PlayerStats _stats;
        public PlayerStats Stats => _stats;

        // Movement
        private float _speed; // cache value
        
        // Health regen
        private float _regen; // cache value
        private Coroutine _regenCoroutine;

        // Pickup range
        [SerializeField] private CircleCollider2D _pickUpCollider;
        
        // Components
        Rigidbody2D _rb;
        Animator _animator;
        SpriteRenderer _sr;
        Collider2D _collider;
        
        // Input
        private InputAction _moveAction;
        private Vector2 _moveInput;
        
        
        #region Initialization
        private void Awake()
        {
            #region Get components
            // Get the components for the rigid body and the animator
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _animator = gameObject.GetComponent<Animator>();
            _sr = gameObject.GetComponent<SpriteRenderer>();
            _collider = gameObject.GetComponent<Collider2D>();

            if (_pickUpCollider == null)
            {
                Debug.LogError("PlayerController has no PickUpRange collider.");
                enabled = false;
                return;
            }

            //Check that there are no null components
            if(_rb == null || _animator == null || _sr == null || _collider == null)
            {
                Debug.LogError("PlayerController has a null component. Missing dependency.");
                enabled = false;
                return;
            }
            #endregion
            
            // Singleton and transform
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            PlayerTransform = transform;

            // GameManager/PlayerController awake race solving 
            // If the game manager is not ready yet, wait for it to be ready before initializing
            if (GameManager.Instance != null)
            {
                Initialize();
            }
            else
            {
                GameManager.OnGameManagerReady += OnGameManagerReady;
            }
        }

        // To solve the race condition between Awake and GameManager initialization
        void OnGameManagerReady()
        {
            Initialize();
        }
        
        private void Start()
        {
            // Start heal coroutine
            _regenCoroutine = StartCoroutine(RegenCoroutine());
        }
        
        #endregion
        
        #region Event subscriptions
        private void OnEnable()
        {
            // Player stats
            PlayerStats.OnStatsChanged += UpdateCacheStats;
            PlayerStats.OnPlayerDeath += OnPlayerDeath;
            
            // Input system
            _moveAction = InputSystem.actions.FindAction("Move");
            _moveAction.performed += HandleMoveInput;
            _moveAction.canceled += HandleMoveInput;

            
        }

        private void OnDisable()
        {
            // GameManager/PlayerController awake race solving 
            GameManager.OnGameManagerReady -= OnGameManagerReady;
            
            // Player stats
            PlayerStats.OnStatsChanged -= UpdateCacheStats;
            PlayerStats.OnPlayerDeath -= OnPlayerDeath;
            
            // Input system
            _moveAction.performed -= HandleMoveInput;
            _moveAction.canceled -= HandleMoveInput;

        }
        #endregion
        
        #region Stats initialization and update

        // Reads the character data from the game manager and initializes the stats
        private void Initialize()
        {
            //Get the character data from the game manager
            CharacterData data = GameManager.Instance.GetStartingCharacter();
            if (data == null)
            {
                Debug.LogError("[PlayerController]: Character data not found!");
                return;
            }
            // Create the PlayerStats instance from the character data
            _stats = new PlayerStats(data);
            
            UpdateCacheStats();
            
            // Apply animator controller from character data
            _animator.runtimeAnimatorController = data.AnimatorController;
        }
        
        // Updates the stats cache, called when the stats change via event subscription
        private void UpdateCacheStats()
        {
            Debug.Log($"Updating stats cache for player {gameObject.name}");
            _speed = Stats.MoveSpeed;
            _regen = Stats.HealthRegen;
            
            // Update pickup range collider radius
            _pickUpCollider.radius = Stats.PickUpRange;
        }

        #endregion
        
        #region Health regen
        private IEnumerator RegenCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                ApplyHeal(_regen);
            }
        }
        
        public void ApplyHeal(float heal)
        {
            Stats.ApplyHeal(heal);
            OnPlayerHeal?.Invoke(heal);
        }
        #endregion
        
        #region OnPlayerDeath
        
        public void OnPlayerDeath()
        {
            // Stop regeneration
            if (_regenCoroutine != null)
            {
                StopCoroutine(_regenCoroutine);
                _regenCoroutine = null;
            }

            
            // Play death animation
            _animator.SetTrigger("Death");
            
            // Disable the rigid body
            _rb.simulated = false;
            _collider.enabled = false;
        }
        #endregion
        
        #region Cleanup
        void OnDestroy()
        {
            // Remove singleton 
            if (Instance == this)
            {
                Instance = null;
                PlayerTransform = null;
            }
        }

        public void DisableCombat()
        {
            _rb.simulated = false;
            _collider.enabled = false;
        }

        #endregion

        #region Movement

        // Read the move input

        private void HandleMoveInput(InputAction.CallbackContext context)
        {
            // This should already give a normalized vector
            _moveInput = context.ReadValue<Vector2>().normalized;
            
            // Flip the sprite based on the direction of the input
            if (_moveInput.x != 0)
            {
                _sr.flipX = _moveInput.x < 0;
            }
        }

        private void Move()
        {
            Vector2 velocity = _moveInput * _speed;
            _rb.linearVelocity = velocity;
            
            // Update animator state
            bool isMoving = _moveInput.sqrMagnitude > 0.5f; // safe margin
            _animator.SetBool("IsMoving", isMoving);
        }


        private void FixedUpdate()
        {
            Move();
        }

        #endregion
        
        public void PlayHurtAnimation()
        {
            Debug.Log("Play hurt animation");
            _animator.SetTrigger("Hurt");
        }
    }
}