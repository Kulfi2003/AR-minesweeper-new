using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    #region defining the white screen and it's fade in and fade out functions
    public Image whiteScreen;

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeImage(0, 1, duration, true));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeImage(1, 0, duration, false));
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration, bool screenActive)
    {
        whiteScreen.gameObject.SetActive(true);

        Color color = whiteScreen.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            whiteScreen.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        whiteScreen.gameObject.SetActive(screenActive);

        // Ensure the final alpha is set
        whiteScreen.color = new Color(color.r, color.g, color.b, endAlpha);
    }
    #endregion

    private void Start()
    {
        if (whiteScreen != null)
        {
            FadeOut(1);
        }
    }

    public void GoToHome()
    {
        FadeIn(1);
        StartCoroutine(LoadLevelWithDelay(1f, 0));
    }

    public void ResetLevel()
    {
        FadeIn(1);
        StartCoroutine(LoadLevelWithDelay(1f, 1));
    }

    private IEnumerator LoadLevelWithDelay(float delay, int level)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(level);
    }
}
