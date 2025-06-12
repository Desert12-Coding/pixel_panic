using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;
    
    void Start()
    {
        // Show menu when scene starts
        startMenuCanvas.SetActive(true);
        // Pause game time if needed
        Time.timeScale = 0f;
    }
    
    public void OnStartButton()
    {
        startMenuCanvas.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        // Add any other startup logic
    }
    
    public void OnQuitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}