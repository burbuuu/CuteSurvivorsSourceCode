using System.Collections.Generic;
using Managers.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Video;

namespace UI.Menu
{
    public class OptionsMainMenuUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MainMenuManager _mainMenu;
        [SerializeField] private VideoSettings _videoSettings;
        
        [Header("ScrollBars and buttons")]
        // Audio sliders
        [SerializeField] private Slider _master;
        [SerializeField] private Slider _music;
        [SerializeField] private Slider _sfx;
        
        // Video settings
        [SerializeField] private Toggle _fullscreen;
        [SerializeField] private TMP_Dropdown _resolution;
        
        // Return button
        [SerializeField] private Button _return;

        private void Awake()
        {
            // Get audio settings
            _master.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            _music.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            _sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

            // Video settings
            // Define dropdown options
            _resolution.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("1920 x 1080"),
                new TMP_Dropdown.OptionData("1600 x 900"),
                new TMP_Dropdown.OptionData("1280 x 720")
            };
            _resolution.AddOptions(options);
            // Load values
            _fullscreen.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            _resolution.value = PlayerPrefs.GetInt("ResolutionIndex", 0);
            _resolution.RefreshShownValue();

            // Add Listeners
            _master.onValueChanged.AddListener(OnMasterVolume);
            _music.onValueChanged.AddListener(OnMusicVolume);
            _sfx.onValueChanged.AddListener(OnSFXVolume);

            _fullscreen.onValueChanged.AddListener(_videoSettings.SetFullscreen);
            _resolution.onValueChanged.AddListener(_videoSettings.SetResolution);

            _return.onClick.AddListener(OnReturnButton);
        }

        private void OnMasterVolume(float vol)
        {
            SoundManager.Instance.SetMasterVolume(vol);
        }

        private void OnMusicVolume(float vol)
        {
            SoundManager.Instance.SetMusicVolume(vol);
        } 

        private void OnSFXVolume(float vol)
        {
            SoundManager.Instance.SetSFXVolume(vol);
        }

        private void OnReturnButton()
        {
            _mainMenu.ShowStartMenu();
        }
        
        
    }
}