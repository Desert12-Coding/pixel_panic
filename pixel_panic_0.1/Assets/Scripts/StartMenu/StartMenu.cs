using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StartMenu : MonoBehaviour
{
    // List of scene names to choose from
    [SerializeField] private List<string> sceneNames = new List<string>();

    public void OnPlayButton()
    {
        // Hide the menu
        gameObject.SetActive(false);
        
        // Make sure we have scenes to choose from
        if (sceneNames.Count == 0)
        {
            Debug.LogError("No scenes assigned in the RandomSceneLoader!");
            return;
        }

        // Select a random scene
        int randomIndex = Random.Range(0, sceneNames.Count);
        string sceneToLoad = sceneNames[randomIndex];

        // Load the scene
        SceneManager.LoadScene(sceneToLoad);
        
        // Resume time in case it was paused
        Time.timeScale = 1f;
    }

    public void OnQuitButton ()
    {
        Application.Quit ();
    }

    public void OnOptionsButton()
    {
        SceneManager.LoadScene(1);
    }
}