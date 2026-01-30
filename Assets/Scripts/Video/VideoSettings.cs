using UnityEngine;

namespace Video
{
    public class VideoSettings : MonoBehaviour
    {
        private Resolution[] resolutions;
        
        void Awake()
        {
            resolutions = new Resolution[]
            {
                // Define my valid resolutions
                new Resolution() { width = 1920, height = 1080 },
                new Resolution() { width = 1600, height = 900 },
                new Resolution() { width = 1280, height = 720 }
            };
        }

        void Start()
        {
            // Load fullscreen setting
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            Screen.fullScreen = fullscreen;
            
            // Load resolution
            int resIndex = PlayerPrefs.GetInt("Resolution", 0);
            SetResolution(resIndex);
            
            Resolution res = resolutions[resIndex];
            Screen.SetResolution(res.width, res.height, fullscreen);
        }
        
        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        }

        public void SetResolution(int index)
        {
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            
            PlayerPrefs.SetInt("Resolution", index);
        }
        
        
    }
}