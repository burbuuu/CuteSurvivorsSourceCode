using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class AccessoryView : MonoBehaviour
    {
        [Header("Images")]
        [SerializeField] private Image image;

        [Header("State")]
        [SerializeField] private Sprite icon;

        private bool _isEquipped;

        private void Awake()
        {
            SetIcon(icon);
            SetEquipped(false);

        }

        // Called when weapon is equipped
        public void SetEquipped(bool equipped)
        {
            _isEquipped = equipped;

            if (!_isEquipped)
            {
                image.enabled = false;
            }
            else
            {
                image.enabled = true;
            }
        }
        
        
        public void SetIcon(Sprite newIcon)
        {
            if (newIcon == null) return;

            icon = newIcon;
            image.sprite = icon;

        }
    }
}