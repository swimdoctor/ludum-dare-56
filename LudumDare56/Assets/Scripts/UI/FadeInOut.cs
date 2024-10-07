using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInOut : MonoBehaviour
{
    public float fadeDuration = 0.6f; // Duration of the fade effect
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0; // Start hidden
        canvasGroup.interactable = false;
    }

    public void Show()
    {
        canvasGroup.interactable = true;
        StartCoroutine(Fade(0, 1));
    }

    public void Hide()
    {
        StartCoroutine(Fade(1, 0));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // Ensure final alpha is set

        canvasGroup.interactable = (endAlpha > 0);
    }
}
