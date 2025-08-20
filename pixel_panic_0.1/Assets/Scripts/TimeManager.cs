using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [Header("UI Elements")]
    [Tooltip("The TextMeshPro UI object that displays the timer.")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [Tooltip("Check this to make the timer start automatically when the object is created.")]
    public bool startOnAwake = true;
    [Tooltip("The TimeManager will DESTROY itself when loading any scene in this list.")]
    public List<string> destroyOnScenes; // Add scenes like "MainMenu" here

    private float elapsedTime;
    private bool isTimerRunning;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (startOnAwake)
        {
            StartTimer();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (destroyOnScenes.Contains(scene.name))
        {
            Destroy(this.gameObject);
        }
        else
        {
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    // --- Public Timer Controls ---

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime % 60F);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
    }

    // --- THE MISSING FUNCTION ---
    // This function allows other scripts (like the LeaderboardManager)
    // to safely get the current timer value.
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
