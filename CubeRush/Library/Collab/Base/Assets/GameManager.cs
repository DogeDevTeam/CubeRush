using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public SpawnScript Spawn;
    public PlayerScript Player;
    private int Score;
    private int HighScore;
    public bool CanCreateNext = true;
    public int ObstacleN = 1;
    public float ObstacleSpeed = 3f;

    [Header("UI")]
    public Text ScoreText;
    public Text HighScoreText;
    public Button PlayAgainButton;

    void Start()
    {
        // Read highscore
        if (PlayerPrefs.HasKey("HighScore"))
        {
            HighScore = PlayerPrefs.GetInt("HighScore");
            HighScoreText.text = "HIGH SCORE\n" + HighScore.ToString();
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", 0);
        }

        Score = 0;
        Spawn.CreateObstacle();  // Spawn first obstacle when game run
    }

    void Update()
    {
        ScoreText.text = Score.ToString();
    }

    public void NewObstacleSet()
    {
        if (CanCreateNext)
        {
            ChangeDifficult();

            CanCreateNext = false;
            Player.ClearShape();

            Score++;
            Spawn.CreateObstacle();
        }
    }

    public void ChangeDifficult()
    {
        if (Score == 10)
        {
            ObstacleSpeed = 5f;
        }

        if (ObstacleN != 8 && Score == ObstacleN * 10)
        {
            ObstacleN++;
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Prototype1");
    }

    public void GameOver()
    {
        PlayAgainButton.gameObject.SetActive(true);

        // Set new highscore
        if (Score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", Score);
        }
    }
}
