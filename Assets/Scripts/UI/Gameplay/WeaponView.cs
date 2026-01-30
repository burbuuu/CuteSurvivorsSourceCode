using Items;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class WeaponView : MonoBehaviour
    {
        [Header("Images")]
        [SerializeField] private Image iconGreyImage;
        [SerializeField] private Image iconFillImage;


        private Sprite icon;    // Runtime icon to use set when binding

        private Weapon weapon;

        private void Awake()
        {
            iconFillImage.enabled = false;
            iconGreyImage.enabled = false;
        }
        
        // Bind weapon to the view
        public void Bind(Weapon weapon)
        {
            this.weapon = weapon;
            SetIcon(weapon.WeaponItem.weaponData.icon);
            iconFillImage.enabled = true;
            iconGreyImage.enabled = true;
        }
        
        private void SetIcon(Sprite newIcon)
        {
            if (newIcon == null) return;

            icon = newIcon;
            iconGreyImage.sprite = icon;
            iconFillImage.sprite = icon;
        }
        
        // Update cooldown
        private void UpdateCooldown()
        {
            if (weapon ==null) return;
            iconFillImage.fillAmount = weapon.CooldownNormalized;
        }
        
        private void Update()
        {
            UpdateCooldown();
        }
    }
}