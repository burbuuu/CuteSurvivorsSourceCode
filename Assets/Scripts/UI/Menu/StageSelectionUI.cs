using System;
using Managers.Scenes;
using Player.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Menu
{
        
    [Serializable]
    public struct StageChoice
    {
        public string sceneName;
        public string displayName;
        public Sprite icon;
        public Button button;
        public string timeLimit;
    }
    
    public class StageSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup _characterSelectionCanvas;
        [SerializeField] private CanvasGroup _stageSelectionCanvas;
        
        [Header("Navigation Buttons")]
        [SerializeField] private Button _startPlay;
        [SerializeField] private Button _returnButton;
        
        [Header("Selected Stage UI")]
        [SerializeField] private TMP_Text _selectedStageSceneName;
        [SerializeField] private TMP_Text _timeLimitTMPText;
        
        [Header("Stage Choices")]
        [SerializeField] private StageChoice[] _stageChoices;

        [Header("State")]
        [SerializeField] private string _selectedSceneName;
        [SerializeField] private string _selectedDisplayName;
        [SerializeField] private string _timeLimit;



        // Subscribe to all choice buttons
        private void Awake()
        {
            foreach (var choice in _stageChoices)
            {
                if (choice.button == null || string.IsNullOrEmpty(choice.sceneName))
                    continue;

                choice.button.onClick.AddListener(
                    () => SelectStage(choice)
                );
            }

            _returnButton.onClick.AddListener(ReturnToCharacterSelection);
            _startPlay.onClick.AddListener(StartGame);
        }

        void Start()
        {
            SelectDefault();
        }

        public void SelectStage(StageChoice choice)
        {
            _selectedSceneName = choice.sceneName;
            _selectedDisplayName = choice.displayName;
            _timeLimit = choice.timeLimit;
            
            if (_selectedStageSceneName != null)
                _selectedStageSceneName.text = _selectedDisplayName;
            
            if (_timeLimitTMPText != null)
                _timeLimitTMPText.text = _timeLimit;
        }

        private void SelectDefault()
        {
            if (_stageChoices.Length == 0) return;
            SelectStage(_stageChoices[0]);
        }

        public void StartGame()
        {
            if (string.IsNullOrEmpty(_selectedSceneName))
            {
                Debug.LogWarning("[StageSelectionUI] No stage selected.");
                return;
            }
            
            GameManager.Instance.TransitionToScene(_selectedSceneName);
        }
        
        public void ReturnToCharacterSelection()
        {
            _characterSelectionCanvas.gameObject.SetActive(true);
            _stageSelectionCanvas.gameObject.SetActive(false);
        }
        
    }
    
}