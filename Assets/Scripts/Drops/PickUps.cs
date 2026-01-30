using Player.Runtime;
using UnityEngine;

namespace Drops
{
    [System.Serializable]
    public struct PickupProbability
    {
        // Pickup to spawn
        public GameObject prefab;
        public float probability; // Per cent
    }
    
    public enum PickupType
    {
        Health,
        Experience
    }

    public class PickUps : MonoBehaviour
    {
        [SerializeField] private int amount;
        [SerializeField] private PickupType type;
        [SerializeField] private float moveSpeed = 3f;
        
        // Logic: When the pick-up enters on the magnet range, it starts moving towards the player
        private bool _isMovingTowardsPlayer = false;
        
        private Rigidbody2D _rb;


        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
            {
                Debug.LogError($"PickUp '{gameObject.name}' has a missing Rigidbody2D component.", gameObject);
                enabled = false;
                return;
            }
        }

        void FixedUpdate()
        {
            if (_isMovingTowardsPlayer) MoveTowardsPlayer();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PickUpRange"))
            {
                _isMovingTowardsPlayer = true;
            }
            else if(other.CompareTag("Player"))
            {
                PickUp();
                Destroy(gameObject);
            }

        }

        void MoveTowardsPlayer()
        {
            var dir = (PlayerController.Instance.transform.position - transform.position).normalized;
            _rb.linearVelocity = dir * moveSpeed;
        }
        
        private void PickUp()
        {
            switch (type)
            {
                case PickupType.Health:
                    PlayerController.Instance.ApplyHeal(amount);
                    // TODO play sound
                    // TODO apply some effect
                    break;
                case PickupType.Experience:
                    PlayerController.Instance.Stats.AddExperience(amount);
                    break;
            }
        }

    }
}