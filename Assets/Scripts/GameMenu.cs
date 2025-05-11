using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class GameMenu : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI highScoreText2;
    public TextMeshProUGUI highScoreText3;


    public TextMeshProUGUI lastScore;

    void Start()
    {

        if (levelText != null)
        {
            levelText.text = "0";
        }

        if (highScoreText != null) 
        { 
        highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
        }
        if (highScoreText2 != null)
        {
            highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();
        }
        if (highScoreText3 != null)
        {
            highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();
        }
        if (lastScore != null)
        {
            lastScore.text = PlayerPrefs.GetInt("lastscore").ToString();
        }

    }
    public void PlayGame() 
    {
        if (Game.startingLevel==0)
        {
            Game.startingAtlevelZero = true;
        }
        else 
        {
            Game.startingAtlevelZero = false;
        }

        SceneManager.LoadScene("Level");
    }

    public void QuitGame() 
    {
        Application.Quit();
    }


    public void ChangeValue(float value) 
    {
        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void LaunchGameMenu() 
    {
        SceneManager.LoadScene("Menu");
    }
}

