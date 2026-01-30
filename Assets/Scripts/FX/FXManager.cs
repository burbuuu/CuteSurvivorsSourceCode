using UnityEngine;

namespace FX
{
    public class FXManager : MonoBehaviour
    {
        public static FXManager Instance { get; private set; }
        
        [Header("Blood FX")]
        [SerializeField] private ParticleSystem bloodFXPrefab;

        [Header("Damage Text")]
        [SerializeField] private DamageText damageTextPrefab;
        [SerializeField] private Canvas uiCanvas;
        
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void PlayHitFX(Vector2 worldPosition, float damage)
        {
            PlayBlood(worldPosition);
            PlayDamageText(worldPosition, Mathf.RoundToInt(damage));
        }

        #region Blood

        private void PlayBlood(Vector2 worldPosition)
        {
            if (bloodFXPrefab == null) return;

            ParticleSystem fx = Instantiate(
                bloodFXPrefab,
                worldPosition,
                Quaternion.identity
            );

            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }

        #endregion

        #region Damage Text

        private void PlayDamageText(Vector2 worldPosition, int damage)
        {
            if (damageTextPrefab == null || uiCanvas == null) return;

            DamageText text = Instantiate(
                damageTextPrefab,
                uiCanvas.transform
            );

            text.Init(damage, worldPosition);
        }

        #endregion
    }
}