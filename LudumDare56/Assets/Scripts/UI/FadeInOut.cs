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
        canvasGroup.blocksRaycasts = false;
    }

    public void Show(float dur = -1)
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        StartCoroutine(Fade(0, 1, dur));
    }

    public void Hide(float dur = -1)
    {
        StartCoroutine(Fade(1, 0, dur));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float dur=-1)
    {
        if (dur < 0)
        {
            dur = fadeDuration;
        }

        float elapsedTime = 0f;

        while (elapsedTime < dur)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / dur);
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // Ensure final alpha is set

        canvasGroup.interactable = (endAlpha > 0);
        canvasGroup.blocksRaycasts = (endAlpha > 0);
    }
}
