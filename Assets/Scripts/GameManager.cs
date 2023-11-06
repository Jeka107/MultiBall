using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    /////////////////////////////
    //when label is on touch disable.
    public delegate void OnLabel(bool touchStatus);
    public static event OnLabel onLabel;

    public delegate void OnNewScore(int newScore);
    public static event OnNewScore onNewScore;


    /////////////////////////////
    [Header("Game Management")]
    [SerializeField] private int points;
    [SerializeField] private int timePointsMultiply;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI multiplicationNumberText;

    private float currentPoints;
    private float time;
    private bool timerOn=false;

    /////////////////////////////
    [Header("Scene Management")]
    [SerializeField] private float waitBeforeLoadScene;
    /////////////////////////////
    [Header("Labels Management")]
    [SerializeField] private GameObject pauseLabel;
    [SerializeField] private GameObject gameOverLabel;

    [SerializeField] private int pointsDecrease; //decrease points after x second.
    [SerializeField] private int onSecondDo;    //on second x decrease points.
    [SerializeField] private float waitForDecreaseTime;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    /////////////////////////////

    private void Awake()
    {
        pauseLabel.SetActive(false);
        gameOverLabel.SetActive(false);

        MainBall.onGameOver += GameOverLabelOn;
        MainBall.onPlayerProgress += ProgressTextUpdate;
    }
    private void OnDestroy()
    {
        MainBall.onGameOver -= GameOverLabelOn;
        MainBall.onPlayerProgress -= ProgressTextUpdate;
    }
    private void Update()
    {
        if (timerOn)
        {
            time += Time.deltaTime;
            DisplayTime(time);
        }
    }
    /////////////////////////////
    #region Game Management
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void ProgressTextUpdate(int currentMultiply)
    {
        timerOn = true;
        currentPoints += points;
        scoreText.text = currentPoints.ToString();
        multiplicationNumberText.text =currentMultiply.ToString();
    }
    #endregion
    /////////////////////////////
    #region Label Management
    public void PauseLabelOn()
    {
        pauseLabel.SetActive(true);
        onLabel?.Invoke(false);
        timerOn = false;
    }
    public void PauseLabelOff()
    {
        pauseLabel.SetActive(false);
        onLabel?.Invoke(true);

        if(time!=0)
            timerOn = true;
    }
    private void GameOverLabelOn()
    {
        gameOverLabel.SetActive(true);
        onLabel?.Invoke(false);
        StartCoroutine(GameOverLabelTimeScoreDisplay());
        onNewScore?.Invoke((int)currentPoints);

        timerOn = false;
    }
    IEnumerator GameOverLabelTimeScoreDisplay()
    {
        while (time > 1)
        {
            time -= 1;

            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);

            if (minutes==0)
            {
                if (seconds % onSecondDo == 0 && seconds!=0)
                    currentPoints -= pointsDecrease;
            }
            else
            {
                if (seconds % onSecondDo == 0)
                    currentPoints -= pointsDecrease;
            }

            if (currentPoints <= 0)
                currentPoints = 0;

            finalTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            finalScoreText.text = (Mathf.FloorToInt(currentPoints)).ToString();

            yield return new WaitForSeconds(waitForDecreaseTime);
        }
    }
    #endregion
    /////////////////////////////
    #region Scene Management
    public void RestartScene()
    {
        StartCoroutine(Restart());
    }
    IEnumerator Restart()
    {
        yield return new WaitForSeconds(waitBeforeLoadScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void LoadMenuScene()
    {
        StartCoroutine(LoadMenu());
    }
    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(waitBeforeLoadScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);

    }
    #endregion
}
