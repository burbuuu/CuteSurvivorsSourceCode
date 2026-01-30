using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

using Player.Data;



public class GameManager : MonoBehaviour
{
    #region Definitions

    // Singleton
    public static GameManager Instance { get; private set; } // Singleton instance
    public static event Action OnGameManagerReady; // This event signal that the instance is ready

    // Audio
    public SoundManager SoundManager { get; private set; } // Reference to SoundManager

    // Scene transition controller
    [SerializeField] private SceneFadeController fadeController;
    public bool IsTransitioning { get; private set; }

    [Header("Gameplay start data")]
    private CharacterData _startingCharacterData = null; 
    [SerializeField] private bool isDebugStartingCharacter = false;
    [SerializeField] private string debugStartingCharacterId;
    
    
    #endregion
    
    #region Initialization
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[GameManager] Singleton instance created.");
        DontDestroyOnLoad(gameObject);
        OnGameManagerReady?.Invoke();
    }
    #endregion

    #region Event subscription
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion
    
    #region Scene transition API
    public void TransitionToScene(string sceneName)
    {
        // Gard preventing multiple simultaneous transitions
        if (IsTransitioning)
        {
            Debug.LogWarning("[GameManager] Transition ignored. A scene transition is already active.");
            return;
        }
        
        // Defensive guard for the scene name
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[GameManager] Scene transition failed. " + $"Scene '{sceneName}' is not added to Build Settings.");
            return;
        }
        
        // Lock transition
        IsTransitioning = true;
        fadeController.TransitionToScene(sceneName); // This calls the Unity SceneManager
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsTransitioning = false;
        Debug.Log($"[GameManager] Transition complete. Now in scene: {scene.name}");
    }
    #endregion

    #region Gameplay start data

    public void SetStartingCharacter(string characterId)
    {
        CharacterDatabase database = FindFirstObjectByType<CharacterDatabase>();
        if (database == null)
        {
            Debug.LogError("[GameManager] CharacterDatabase not found.");
            _startingCharacterData = null;
            return;
        }

        _startingCharacterData = database.GetCharacterData(characterId);
    }

    // Returns the starting Character Data. If the character data is not set fallback to the default data
    // If its on editor and the debug option is active, then returns the debug character 
    public CharacterData GetStartingCharacter()
    {
        CharacterDatabase db = FindFirstObjectByType<CharacterDatabase>();
        if (db == null)
        {
            Debug.LogError("[GameManager] CharacterDatabase not found in scene!");
            return null;
        }

        // Check Editor Debug Override
#if UNITY_EDITOR
        if (isDebugStartingCharacter && !string.IsNullOrEmpty(debugStartingCharacterId))
        {
            Debug.Log($"[GameManager] Debug Mode: Loading character {debugStartingCharacterId}");
            return db.GetCharacterData(debugStartingCharacterId);
        }
#endif

        // Check for the assigned starting character
        if (_startingCharacterData != null)
        {
            return _startingCharacterData;
        }

        // Fallback to Default
        Debug.LogWarning("[GameManager] No character assigned. Falling back to default.");
        return db.GetDefaultCharacter();
    }
    

    public void ClearStartingCharacter()
    {
        _startingCharacterData = null;
    }

    #endregion


    #region Input methods
    public void SwitchToUIActionMap()
    {
        var actions = InputSystem.actions;
        if (actions == null)
        {
            Debug.LogError("[GameManager]: Project-wise InputActions no found.");
            return;
        }

        //Enable UI action map, disable Playe
        actions.FindActionMap("UI").Enable();
        actions.FindActionMap("Player").Disable();
    }

        public void SwitchToPlayerActionMap()
    {
        var actions = InputSystem.actions;
        if (actions == null)
        {
            Debug.LogError("[GameManager]: Project-wise InputActions no found.");
            return;
        }

        //Enable Player action map, disable UI 
        actions.FindActionMap("UI").Disable();
        actions.FindActionMap("Player").Enable();
    }

    #endregion
}
