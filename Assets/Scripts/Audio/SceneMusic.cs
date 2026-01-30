using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] string _sceneMusic;
    [SerializeField] private bool loops = true;
    void Start()
    {
        SoundManager.Instance.PlayMusic(_sceneMusic,loops);
    }
}
