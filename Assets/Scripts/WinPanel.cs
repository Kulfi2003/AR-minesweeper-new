using UnityEngine;
using TMPro;

public class WinPanel : MonoBehaviour
{
    
    [SerializeField] GameManager gameManager;

    [SerializeField] TextMeshProUGUI text;
    bool highScore = false;
    string textPrint = "";

    // Start is called before the first frame update
    void Start()
    {
        switch (PlayerPrefs.GetInt("difficulty"))
        {
            case 0:
                if (!PlayerPrefs.HasKey("easyScore"))
                {
                    PlayerPrefs.SetFloat("easyScore", gameManager.timePassed);
                    highScore = true;
                }
                else if (gameManager.timePassed < PlayerPrefs.GetFloat("easyScore"))
                {
                    PlayerPrefs.SetFloat("easyScore", gameManager.timePassed);
                    highScore = true;
                }

                if (highScore)
                {
                    textPrint += "New Personal Best! \n Easy difficulty \n Time taken : " + ConvertToMinutesAndSeconds(gameManager.timePassed);
                }
                else
                {
                    textPrint += "Easy difficulty \n Time taken : " + ConvertToMinutesAndSeconds(gameManager.timePassed);
                }
                break;

            case 1:
                if (!PlayerPrefs.HasKey("mediumScore"))
                {
                    PlayerPrefs.SetFloat("mediumScore", gameManager.timePassed);
                    highScore = true;
                }
                else if (gameManager.timePassed < PlayerPrefs.GetFloat("mediumScore"))
                {
                    PlayerPrefs.SetFloat("mediumScore", gameManager.timePassed);
                    highScore = true;
                }

                if (highScore)
                {
                    textPrint += "New Personal Best! \n Medium difficulty \n Time taken : " + ConvertToMinutesAndSeconds(gameManager.timePassed);
                }
                else
                {
                    textPrint += "Medium difficulty \n Time taken : " + ConvertToMinutesAndSeconds(gameManager.timePassed);
                }
                break;

            case 2:
                if (!PlayerPrefs.HasKey("hardScore"))
                {
                    PlayerPrefs.SetFloat("hardScore", gameManager.timePassed);
                    highScore = true;
                }
                else if (gameManager.timePassed < PlayerPrefs.GetFloat("hardScore"))
                {
                    PlayerPrefs.SetFloat("hardScore", gameManager.timePassed);
                    highScore = true;
                }

                if (highScore)
                {
                    textPrint += "New Personal Best! \n Hard difficulty \n Time taken : " + ConvertToMinutesAndSeconds(gameManager.timePassed);
                }
                else
                {
                    textPrint += "Hard difficulty \n Time taken : " + ConvertToMinutesAndSeconds(gameManager.timePassed);
                }
                break;

            default:
                textPrint += "Time taken : " + ConvertToMinutesAndSeconds(gameManager.timePassed);
            break;
        }

        text.text = textPrint;
    }

    string ConvertToMinutesAndSeconds(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);  // Get the total minutes
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);  // Get the remaining seconds

        // Format the string to be "mins:secs" and pad seconds with a leading zero if needed
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }

}
