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
    private bool progressStarted=false;

    /////////////////////////////
    [Header("Scene Management")]
    [SerializeField] private float waitBeforeLoadScene;
    /////////////////////////////
    [Header("Labels Management")]
    [SerializeField] private GameObject pauseLabel;
    [SerializeField] private GameObject gameOverLabel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private bool gameOver=false;
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
        if (progressStarted)
        {
            time += Time.deltaTime*1.5f;
            timeText.text = ((int)time).ToString();
        }
        if (gameOver)
        {
            if (time >= 0)
            {
                time -= Time.deltaTime * 15;
                finalTimeText.text = ((int)time).ToString();

                FinalScore();
            }
            else
            {
                onNewScore?.Invoke((int)currentPoints);
                gameOver = false;
            }
        }
    }
    /////////////////////////////
    #region Game Management
    private void ProgressTextUpdate(int currentMultiply)
    {
        progressStarted = true;
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
    }
    private void GameOverLabelOn()
    {
        gameOverLabel.SetActive(true);
        onLabel?.Invoke(false);

        progressStarted = false;
        gameOver = true;
    }
    private void FinalScore()
    {
        currentPoints -= Time.deltaTime * timePointsMultiply;

        if (currentPoints <= 0)
            currentPoints = 0;
        
        finalScoreText.text = ((int)currentPoints).ToString();
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
