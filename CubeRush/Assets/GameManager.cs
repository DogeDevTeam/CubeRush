using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects")]
    public SpawnScript Spawn;
    public PlayerScript Player;
    public Camera Cam;
    private int Score;
    private int HighScore;
    private float TargetProgress;
    private int ScoreToNextLevel;

    [Header("Obstacle start values")]
    public int ObstacleGridSize;
    public bool CanCreateNext = true;
    public int ObstacleN = 1;
    public float ObstacleSpeed = 3f;
    public int NextLevelValue = 25;

    [Header("UI")]
    public Text ScoreText;
    public Text HighScoreText;
    public GameObject LosePanel;
    public Text SummaryScore;
    public Slider NextLevelSlider;

    void Start()
    {
        NextLevelSlider.value = 0;

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
        NextLevelSlider.value = (float)Score / (float)NextLevelValue;
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
        // Change game parameters
        if (ObstacleGridSize != 11 && Score > 0 && Score == NextLevelValue)
        {
            // Change value to next level
            NextLevelValue += 100;
            
            // Increase grid size
            ObstacleGridSize += 2;

            // Change camera field of view
            Cam.fieldOfView += 10;

            // Increase player limits
            Player.topLimit += 0.5f;
            Player.bottomLimit -= 0.5f;
            Player.leftLimit -= 0.5f;
            Player.rightLimit += 0.5f;
        }

        // Increase speed mechanics
        if (Score == 10)
        {
            ObstacleSpeed = 4f;
        }
        
        if (Score == 30)
        {
            ObstacleSpeed = 4.5f;
        }

        // Increase level mechanics
        if (ObstacleN != 10 && Score == ObstacleN * 25)
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
        LosePanel.SetActive(true);
        SummaryScore.text = Score.ToString();

        // Set new highscore
        if (Score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", Score);
        }
    }

    public void Pasue()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
}
