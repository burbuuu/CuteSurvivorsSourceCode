using System.Collections;
using Items.Data;
using Player.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace Items
{
    [System.Serializable]
    public class WeaponItem : Item
    {
        public WeaponData weaponData;
        public event System.Action OnWeaponLevelUp;
        public WeaponItem(WeaponData data) : base(data)
        {
            weaponData = data;
        }

        protected override void HandleItemLevelUp()
        {
            base.HandleItemLevelUp();
            OnWeaponLevelUp?.Invoke();
        }
    }

    /// <summary>
    /// This enum defines the weapon projectile shooting patter
    /// </summary>
    public enum ShootPatternType
    {
        TargetClosestAndSpread,
        TargetNClosest,
        RandomTargetAndSpread,
        RandomNTargets,
        CircleAroundPlayer
    }

    /// <summary>
    /// Weapon: Reads the data from its WeaponItem, which defines the base stats of the weapon and then applies
    /// the bonifications of the player stats. 
    /// This class also calls periodically UseWeapon(), which shoots the projectile.
    /// For modifying the weapon prefab, you should modify its ScriptableObject. 
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        #region Definitions
        [SerializeField] private WeaponItem _weaponItem; // Weapon item instance, contains weapon data
        public WeaponItem WeaponItem => _weaponItem;
        
        #endregion
        
        #region Runtime values, editor visualization
        [Header("Runtime values")]
        [SerializeField] private float runtimeDamage;
        [SerializeField] private float runtimeSize;
        [SerializeField] private float runtimeCooldown;
        [SerializeField] private float runtimeProjectileSpeed;
        [SerializeField] private float runtimeDuration;
        [SerializeField] private int runtimeProjectileCount;

        [SerializeField] private float useTimer;
        #endregion
        
        #region Public getters
        // Public getters for runtime stats if needed by other systems
        public float RuntimeDamage => runtimeDamage;
        public float RuntimeSize => runtimeSize;
        public float RuntimeCooldown => runtimeCooldown;
        public float RuntimeProjectileSpeed => runtimeProjectileSpeed;
        public float RuntimeDuration => runtimeDuration;
        public int RuntimeProjectileCount => runtimeProjectileCount;
        public float CooldownNormalized // Used by the weapon view GUI
        {
            get
            {
                if (runtimeCooldown <= 0f) return 0f;
                return 1f - (useTimer / runtimeCooldown);
            }
        }
        #endregion
        
        #region Event subscription
        protected virtual void OnEnable()
        {
            if (_weaponItem == null)
            {
                Debug.LogWarning($"WeaponItem is missing on {gameObject.name}. If this is a prefab, this is normal. If it's in the scene, please assign it.", this);
                return;
            }

            PlayerStats.OnStatsChanged -= RecalculateWeaponStats;
            PlayerStats.OnStatsChanged += RecalculateWeaponStats;
            
            
            _weaponItem.OnWeaponLevelUp -= OnWeaponLeveledUp;
            _weaponItem.OnWeaponLevelUp += OnWeaponLeveledUp;
        }
        
        protected virtual void OnDisable()
        {
            PlayerStats.OnStatsChanged -= RecalculateWeaponStats;
            
            if (_weaponItem != null)
            {
                _weaponItem.OnWeaponLevelUp -= OnWeaponLeveledUp;
            }
        }
        #endregion
        
        #region Stats and Level Up 
        
        private void OnWeaponLeveledUp()
        {
            RecalculateWeaponStats();
            useTimer = 0f; // fire immediately on next Update
            UseWeapon();
        }
        
        /// <summary>
        /// Calculates the runtime stats based on the base stats of the weapon
        /// and the bonifications of the player stats.
        /// </summary>
        private void RecalculateWeaponStats()
        {
            if (_weaponItem == null || _weaponItem.weaponData == null) return;
            
            // Refresh base stats in case the level changed
            var baseStats = _weaponItem.weaponData.GetWeaponBaseStatsForLevel(_weaponItem.Level);
            if (baseStats == null) return;

            var newPlayerStats = PlayerController.Instance.Stats;
            
            // Calculated by applying the bonification. Some bons are multipliers and others are additive.
            runtimeCooldown = baseStats.BaseCooldown / newPlayerStats.CooldownBonus; // In this case reduces it
            runtimeDamage = baseStats.BaseDamage * newPlayerStats.DamageBonus;
            runtimeSize = baseStats.BaseSize * newPlayerStats.AreaBonus;
            runtimeProjectileSpeed = baseStats.BaseProjectileSpeed * newPlayerStats.ProjectileSpeedBonus;
            runtimeDuration = baseStats.BaseDuration * newPlayerStats.DurationBonus;
            runtimeProjectileCount = baseStats.BaseProjectileCount + newPlayerStats.ProjectileCountBonus;
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            if (_weaponItem != null && _weaponItem.Level <= 0)
            {
                _weaponItem.Level = 1;
            }
        }
        private void Start()
        {
            StartCoroutine(InitializeAfterPlayerControllerStart());
            useTimer = 0.1f;
        }
        
        private IEnumerator InitializeAfterPlayerControllerStart()
        {
            yield return null;
            RecalculateWeaponStats();
            useTimer = runtimeCooldown;
        }
        #endregion

        private void Update()
        {
            // Update timer and use the weapon
            useTimer -= Time.deltaTime;
            if (useTimer <= 0)
            {
                UseWeapon();
                useTimer = runtimeCooldown;
            }
        }

        #region Use Weapon
        
        private void UseWeapon()
        {
            if (_weaponItem == null || _weaponItem.weaponData == null) return;

            switch (_weaponItem.weaponData.ShootPattern)
            {
                case ShootPatternType.TargetClosestAndSpread:
                    TargetClosestAndSpread();
                    break;
                case ShootPatternType.TargetNClosest:
                    TargetNClosest();
                    break;
                case ShootPatternType.RandomTargetAndSpread:
                    RandomTargetAndSpread();
                    break;
                case ShootPatternType.RandomNTargets:
                    RandomNTargets();
                    break;
                case ShootPatternType.CircleAroundPlayer:
                    CircleAroundPlayer();
                    break;
            }
        }

        // Shoots from player. Lock on a unique target and spread the projectiles in that direction
        private void TargetClosestAndSpread()
        {
            if (_weaponItem?.weaponData?.ProjectilePrefab == null) return;
            
            var targets = Enemies.EnemyManager.Instance.GetClosestEnemiesPosition(1);
            Vector2 direction;
            
            if (targets.Count > 0)
            {
                direction = (targets[0] - (Vector2)transform.position).normalized;
            }
            else
            {
                // If no enemies found, target a random direction
                direction = Random.insideUnitCircle.normalized;
            }

            SpawnSpread(direction);
        }

        // Shoot from player to n the closest enemies
        private void TargetNClosest()
        {
            if (_weaponItem?.weaponData?.ProjectilePrefab == null) return;

            var targets = Enemies.EnemyManager.Instance.GetClosestEnemiesPosition(runtimeProjectileCount);
            var playerPos = PlayerController.Instance.transform.position;
            // Spawn at found targets
            foreach (var targetPos in targets)
            {
                Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
                SpawnProjectile(dir, playerPos,alignDirection:true);
            }

            // If less targets than projectile count, shoot the rest in random directions from player
            int remaining = runtimeProjectileCount - targets.Count;
            for (int i = 0; i < remaining; i++)
            {
 
                var randDir = Random.insideUnitCircle;
                SpawnProjectile(randDir,alignDirection:true);
            }
        }

        // Spawns on an enemy and moves in a random direction
        private void RandomTargetAndSpread()
        {
            if (_weaponItem?.weaponData?.ProjectilePrefab == null) return;

            var target = Enemies.EnemyManager.Instance.GetRandomEnemiesPosition(1, 12f);
            Vector2 spawnPosition;

            if (target.Count > 0)
            {
                spawnPosition = target[0];
            }
            else
            {
                spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 12f;
            }

            float chainRadius = 0.8f;
            Vector2 direction = Random.insideUnitCircle.normalized;

            for (int i = 0; i < runtimeProjectileCount; i++)
            {
                SpawnProjectile(direction, spawnPosition);

                // change direction for next step
                direction = (direction + Random.insideUnitCircle * 0.4f).normalized;
                spawnPosition += direction * chainRadius;
            }
        }


        // Spawns on random targets and dont move
        private void RandomNTargets()
        {
            if (_weaponItem?.weaponData?.ProjectilePrefab == null) return;

            var targets = Enemies.EnemyManager.Instance.GetRandomEnemiesPosition(runtimeProjectileCount, 10f);
            
            // Spawn at found targets
            foreach (var targetPos in targets)
            {
                SpawnProjectile(Vector2.zero, targetPos, alignDirection:false);
            }

            // If less targets than projectile count, shoot the rest in random directions from player
            int remaining = runtimeProjectileCount - targets.Count;
            for (int i = 0; i < remaining; i++)
            {
                var randPos = Random.insideUnitCircle * 15f;
                SpawnProjectile(Vector2.zero, randPos, alignDirection:false);
            }
        }

        private void CircleAroundPlayer()
        {
            if (_weaponItem?.weaponData?.ProjectilePrefab == null)
            {
                Debug.LogWarning(
                    $"[{nameof(SpawnProjectile)}] Weapon '{name}' tried to spawn a projectile, but ProjectilePrefab is null.", 
                    gameObject
                );
                return;
            }

            float angleStep = 360f / runtimeProjectileCount;
            float randomOffset = Random.Range(0f, 360f); // 👈 random start angle
            for (int i = 0; i < runtimeProjectileCount; i++)
            {
                float angle = randomOffset + i * angleStep;
                Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                SpawnProjectile(direction, isCirclingAroundPlayer: true);
            }
        }

        private void SpawnSpread(Vector2 centerDirection)
        {
            if (runtimeProjectileCount <= 1)
            {
                SpawnProjectile(centerDirection);
                return;
            }

            float spreadAngle = 10f; // Angle between projectiles
            float startAngle = -spreadAngle * (runtimeProjectileCount - 1) / 2f;
            float baseAngle = Mathf.Atan2(centerDirection.y, centerDirection.x) * Mathf.Rad2Deg;

            for (int i = 0; i < runtimeProjectileCount; i++)
            {
                float currentAngle = (baseAngle + startAngle + i * spreadAngle) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
                SpawnProjectile(direction);
            }
        }

        private void SpawnProjectile(Vector2 direction, Vector2? spawnPosition = null, bool alignDirection = false,bool isCirclingAroundPlayer = false)
        {
            if (_weaponItem?.weaponData?.ProjectilePrefab == null)
            {
                Debug.LogWarning($"[{nameof(SpawnProjectile)}] Weapon '{name}' tried to spawn a projectile, but ProjectilePrefab is null.", gameObject);
                return;
            }
            Vector2 position = spawnPosition ?? (Vector2)transform.position;
            GameObject go = Instantiate(_weaponItem.weaponData.ProjectilePrefab, position, Quaternion.identity);
            if (go.TryGetComponent(out Projectile projectile))
            {
                projectile.Initialize(runtimeDamage, runtimeProjectileSpeed, runtimeSize, runtimeDuration, direction, 
                    _weaponItem.weaponData.itemName, isCirclingAroundPlayer: isCirclingAroundPlayer, rotatesWithDirection:alignDirection);
            }
        }

        #endregion
    }
}