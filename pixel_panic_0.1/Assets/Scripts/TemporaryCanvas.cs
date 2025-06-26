using UnityEngine;
using System.Collections;

public class TemporaryCanvas : MonoBehaviour
{
    [Tooltip("Drag your Canvas object here")]
    public Canvas targetCanvas;
    
    [Tooltip("How long the canvas stays fully visible (seconds)")]
    public float displayTime = 2f;
    
    [Tooltip("Duration of fade in/out effects (seconds)")]
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (targetCanvas != null)
        {
            // Get or add CanvasGroup component
            canvasGroup = targetCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = targetCanvas.gameObject.AddComponent<CanvasGroup>();
            }
            
            StartCoroutine(ShowAndHide());
        }
        else
        {
            Debug.LogError("Target Canvas is not assigned!");
        }
    }

    IEnumerator ShowAndHide()
    {
        // Initialize
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Fade in
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
        
        // Fully visible
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        // Wait display time
        yield return new WaitForSeconds(displayTime);
        
        // Fade out
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        
        // Fully hidden
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}