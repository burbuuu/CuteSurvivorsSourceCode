using System;
using Managers.Scenes;
using Player.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{

    [Serializable]
    public struct CharacterChoice
    {
        public CharacterData characterData;
        public Button button;
    }
    
    public class CharacterSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MainMenuManager _mainMenuManager;
        [SerializeField] private CanvasGroup _characterSelectionCanvas;
        [SerializeField] private CanvasGroup _stageSelectionCanvas;
        
        [Header("Navigation Buttons")]
        [SerializeField] private Button _stageSelectionButton;
        [SerializeField] private Button _returnButton;
        
        [Header("Selected Character")]
        [SerializeField] private Image _selectedCharacterIcon;
        [SerializeField] private TMP_Text _selectedCharacterNameText;
        [SerializeField] private CharacterChoice[] _characterChoices;

        [Header("State")]
        [SerializeField] private CharacterData _characterData;
        [SerializeField] private Sprite _selectedSprite;


        // Subscribe to all choice buttons
        private void Awake()
        {
            // Subscribe to all choice buttons
            foreach (var choice in _characterChoices)
            {
                if (choice.button == null || choice.characterData == null) continue;
                choice.button.onClick.AddListener(() => SelectCharacter(choice.characterData));
            }
            
            // Navigation buttons
            _returnButton.onClick.AddListener(ReturnToMainMenu);
            _stageSelectionButton.onClick.AddListener(GoToStageSelection);
        }

        void Start()
        {
            SelectDefault();
        }

        public void SelectCharacter(CharacterData _selectedData)
        {
            if (_selectedData == null)
            {
                Debug.LogWarning("[CharacterSelectionUI] Character selection is null.");
                return;
            }
            
            _characterData = _selectedData;
            _selectedSprite = _characterData.Miniature;
            _selectedCharacterIcon.sprite = _selectedSprite;
            _selectedCharacterIcon.SetNativeSize();
            _selectedCharacterNameText.text = _characterData.StatsId; // Name of the character
            
            // TODO : Display stats
        }

        private void SelectDefault()
        {
            // Select the first character of the array
            SelectCharacter(_characterChoices[0].characterData);
        }

        public void GoToStageSelection()
        {
            GameManager.Instance.SetStartingCharacter(_characterData.StatsId);
            _characterSelectionCanvas.gameObject.SetActive(false);
            _stageSelectionCanvas.gameObject.SetActive(true);
        }
        
        public void ReturnToMainMenu()
        {
            _mainMenuManager.ShowStartMenu();
        }
        
    }
}