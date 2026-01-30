using UnityEngine;

namespace Enemies.Data
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string characterId;
        public string CharacterId => characterId;
        
        [Header("Visuals")]
        public AnimatorOverrideController AnimatorOverrideController;
        
        [Header("Stats")]
        public float Health;
        public float Damage;
        public float DamageCooldown;
        public float MoveSpeed;
    }
}