using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneChanger : MonoBehaviour
{
    [Tooltip("List of scene names to randomly choose from")]
    public List<string> sceneNames = new List<string>();

    [Tooltip("Should the scene change happen immediately on contact?")]
    public bool changeImmediately = true;

    [Tooltip("Delay before scene change (if not immediate)")]
    public float delayBeforeChange = 1f;

    [Tooltip("Tag of the player object")]
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            if (changeImmediately)
            {
                ChangeToRandomScene();
            }
            else
            {
                Invoke("ChangeToRandomScene", delayBeforeChange);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            if (changeImmediately)
            {
                ChangeToRandomScene();
            }
            else
            {
                Invoke("ChangeToRandomScene", delayBeforeChange);
            }
        }
    }

    private void ChangeToRandomScene()
    {
        if (sceneNames.Count == 0)
        {
            Debug.LogWarning("No scenes assigned to SceneChanger!");
            return;
        }

        int randomIndex = Random.Range(0, sceneNames.Count);
        string sceneToLoad = sceneNames[randomIndex];

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Empty scene name in SceneChanger!");
        }
    }
}
