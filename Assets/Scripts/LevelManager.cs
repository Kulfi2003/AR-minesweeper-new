using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.XR.ARFoundation;

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

        if (easyScore != null) SetMainMenuUI();
    }


    [SerializeField] TextMeshProUGUI easyScore, mediumScore, hardScore;
    void SetMainMenuUI()
    {

        if (PlayerPrefs.HasKey("easyScore")) easyScore.text = "HighScore : " + ConvertToMinutesAndSeconds(PlayerPrefs.GetFloat("easyScore"));
        else easyScore.text = "HighScore : NA";

        if (PlayerPrefs.HasKey("mediumScore")) mediumScore.text = "HighScore : " + ConvertToMinutesAndSeconds(PlayerPrefs.GetFloat("mediumScore"));
        else mediumScore.text = "HighScore : NA";

        if (PlayerPrefs.HasKey("hardScore")) hardScore.text = "HighScore : " + ConvertToMinutesAndSeconds(PlayerPrefs.GetFloat("hardScore"));
        else hardScore.text = "HighScore : NA";
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

        //Destroy all trackables
        ARPlaneManager arPlaneManager = FindObjectOfType<ARPlaneManager>();
        if (arPlaneManager != null)
        {
            foreach (var plane in arPlaneManager.trackables)
            {
                Destroy(plane.gameObject);
            }
        }

        SceneManager.LoadScene(level);
    }

    string ConvertToMinutesAndSeconds(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);  // Get the total minutes
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);  // Get the remaining seconds

        // Format the string to be "mins:secs" and pad seconds with a leading zero if needed
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }

    public void LoadLevel(int difficulty)
    {
        PlayerPrefs.SetInt("difficulty", difficulty); 
        FadeIn(1);
        StartCoroutine(LoadLevelWithDelay(1f, 1));
    }

    public void GameManagerUI(bool turnOff)
    {
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = !turnOff;
    }
}
