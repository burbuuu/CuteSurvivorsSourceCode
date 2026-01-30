using LevelUpSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class LevelUpChoiceView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Button button;

        private ILevelUpChoice _choice;
        private LevelUpUI _parent;

        public void Setup(ILevelUpChoice choice, LevelUpUI parent)
        {
            Debug.Log($"Choice setup: {choice} | ItemData: {choice?.ItemData}");
            _choice = choice;
            _parent = parent;
            
            icon.sprite = choice.ItemData.icon;
            itemName.text = choice.ItemData.itemName;

            description.text = choice.ItemData.GetDescriptionForLevel(choice.PreviewLevel);
            
            button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            Debug.Log($"Clicked choice: {_choice}");
            _parent.OnChoiceSelected(_choice);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClicked);
        }
    }
}