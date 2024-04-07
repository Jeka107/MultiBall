using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Enums;

public class GameManager : MonoBehaviour
{
    /////////////////////////////
    //when label is on,touch disable.
    public delegate void OnLabel(bool touchStatus);
    public static event OnLabel onLabel;

    public delegate int OnGetHighScore();
    public static event OnGetHighScore onGetHighScore;

    public delegate void OnNewScore(int newScore);
    public static event OnNewScore onNewScore;

    //Slider
    public delegate void OnSliderprogress();
    public static event OnSliderprogress onSliderprogress;
    public delegate void OnSliderReset();
    public static event OnSliderReset onSliderReset;
    //

    //SoundEffect
    public delegate bool OnLastSoundEffectStatus();
    public static event OnLastSoundEffectStatus onLastSoundEffectStatus;
    public delegate void OnSetSoundEffect(bool soundEffectStatus);
    public static event OnSetSoundEffect onSetSoundEffect;
    public delegate void OnTextAppearsSoundEffect();
    public static event OnTextAppearsSoundEffect onTextAppearsSoundEffect;
    public delegate void OnNewHighScoreSE();
    public static event OnNewHighScoreSE onNewHighScoreSE;
    public delegate void OnProgressSE();
    public static event OnProgressSE onProgressSE;
    //

    public delegate void OnClick();
    public static event OnClick onClick;

    public delegate void OnAdOn();
    public static event OnAdOn onAdOn;


    /////////////////////////////
    [Header("Game Management")]
    [SerializeField] private int points;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI multiplicationNumberText;

    private float currentPoints;
    private int progressPoints;
    private float time;
    private float previousTime=0;
    private bool timerOn=false;

    [Space]
    [Header("Bonus Points Management")]
    [SerializeField] private List<int> TimeBonuss;
    [SerializeField] private TextMeshProUGUI scoreMultiText;
    [SerializeField] private int bonusPoint;
    [SerializeField] private GameObject progress;
    [SerializeField] private int maxNumHitBall;

    private int currentTimeBonus;
    private TextMeshProUGUI progressText;
    private Animator progressAnimator;
    private int numHitBall = 0;
    private int currentProgressState = 0;

    /////////////////////////////
    [Space]
    [Header("Scene Management")]
    [SerializeField] private float waitBeforeLoadScene;
    /////////////////////////////
    [Space]
    [Header("Labels Management")]
    [SerializeField] private GameObject pauseLabel;
    [SerializeField] private GameObject gameStateLabel;
    [SerializeField] private GameObject rewardBtn;
    /////////////////////////////
    [Space]
    [Header("Sound Management")]
    [SerializeField] private GameObject soundEffectOn;
    [SerializeField] private GameObject soundEffectOff;

    private bool soundEffectStatus;

    [Space]
    [Header("GameState Management")]
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private float textSpeedAppears;
    [SerializeField] private GameObject finalScore;
    [SerializeField] private GameObject finalScoreNum;
    [SerializeField] private GameObject bestScore;
    [SerializeField] private GameObject bestScoreNum;
    [SerializeField] private GameObject newText;
    [SerializeField] private GameObject rewardedAdsButton;

    private TextMeshProUGUI finalScoreNumText;
    private TextMeshProUGUI bestScoreNumText;
    private int currentBestScore;

    private float minutes;
    private float seconds;
    private float previousSeconds;
    /////////////////////////////

    private void Awake()
    {
        progressText = progress.GetComponent<TextMeshProUGUI>();
        progressAnimator = progress.GetComponent<Animator>();
        currentTimeBonus = TimeBonuss[0];

        //labels off.
        progress.SetActive(false);
        pauseLabel.SetActive(false);
        gameStateLabel.SetActive(false);
        //Texts off
        finalScore.SetActive(false);
        finalScoreNum.SetActive(false);
        bestScore.SetActive(false);
        bestScoreNum.SetActive(false);
        newText.SetActive(false);
        //Game over label.
        finalScoreNumText = finalScoreNum.GetComponent<TextMeshProUGUI>();
        bestScoreNumText = bestScoreNum.GetComponent<TextMeshProUGUI>();
        
        MainBall.onGameOver += GameStateLabelOn;
        MainBall.onPlayerProgress += ProgressTextUpdate;
        BallsManager.onMaxMultiplier += GameStateLabelOn;
        BallsManager.onSecondChance += GameStateLabelOff;
        BallsManager.onSecondChance += ResetPointsBaseTime;
        soundEffectStatus = onLastSoundEffectStatus.Invoke();

        SoundEffectStatus();
    }
    private void OnDestroy()
    {
        MainBall.onGameOver -= GameStateLabelOn;
        MainBall.onPlayerProgress -= ProgressTextUpdate;
        BallsManager.onMaxMultiplier -= GameStateLabelOn;
        BallsManager.onSecondChance -= GameStateLabelOff;
        BallsManager.onSecondChance -= ResetPointsBaseTime;
    }
    private void Update()
    {
        if (timerOn)
        {
            time += Time.deltaTime;
            seconds = Mathf.FloorToInt(time % 60);

            //Bonus
            if (!(previousSeconds + currentTimeBonus >= seconds))
            {
                numHitBall = 0;
                currentProgressState = 0;
                PointsBaseTime(0);
                onSliderReset?.Invoke();
            }
        }
    }
    /////////////////////////////
    #region Game Management
    private void ProgressTextUpdate(int currentMultiply)
    {
        if (previousSeconds + currentTimeBonus >= seconds)
        {
            if (currentProgressState < 5)
            {
                numHitBall++;
                onSliderprogress?.Invoke();
                
                if (numHitBall == maxNumHitBall)
                {
                    onProgressSE?.Invoke();
                    currentProgressState++;
                    numHitBall = 0;

                    if(currentProgressState!=5)
                        onSliderReset?.Invoke(); 
                }
            }
        }
        else
        {
            onSliderReset?.Invoke();
            numHitBall = 0;
            currentProgressState = 0;
        }
        PointsBaseTime(currentProgressState);

        timerOn = true;
        currentPoints += progressPoints;
        scoreText.text = currentPoints.ToString();
        multiplicationNumberText.text =currentMultiply.ToString();

        previousTime = time;
        previousSeconds = Mathf.FloorToInt(previousTime % 60);
    }
    private void PointsBaseTime(int progressState)
    {
        scoreMultiText.text = "X";

        switch (progressState)
        {
            case 0:
                progress.SetActive(false);
                currentTimeBonus = TimeBonuss[0];
                progressPoints = points;
                scoreMultiText.text = "";
                break;
            case 1:
                progress.SetActive(true);
                currentTimeBonus = TimeBonuss[1];
                progressText.text = PROGRESS_TEXT.GoodJob + "!";
                progressPoints = points * 2* bonusPoint;
                scoreMultiText.text += 2 * bonusPoint;
                break;
            case 2:                
                currentTimeBonus = TimeBonuss[2];
                progressAnimator.SetTrigger("Default");
                progressText.text = PROGRESS_TEXT.Great + "!";
                progressPoints = points * 3* bonusPoint;
                scoreMultiText.text += 3 * bonusPoint;
                break;
            case 3:
                currentTimeBonus = TimeBonuss[3];
                progressAnimator.SetTrigger("Default");
                progressText.text = PROGRESS_TEXT.Wow + "!";
                progressPoints = points * 4* bonusPoint;
                scoreMultiText.text += 4 * bonusPoint;
                break;
            case 4:
                currentTimeBonus = TimeBonuss[4];
                progressAnimator.SetTrigger("Default");
                progressText.text = PROGRESS_TEXT.Excellent + "!";
                progressPoints = points * 5*bonusPoint;
                scoreMultiText.text += 5 * bonusPoint;
                break;
            case 5:
                currentTimeBonus = TimeBonuss[5];
                progressAnimator.SetTrigger("Default");
                progressText.text = PROGRESS_TEXT.Insane + "!";
                progressPoints = points * 6*bonusPoint;
                scoreMultiText.text += 6 * bonusPoint;
                break;
        }
    }
    private void ResetPointsBaseTime()
    {
        numHitBall = 0;
        currentProgressState = 0;
        PointsBaseTime(0);
        onSliderReset?.Invoke();
    }
    #endregion
    /////////////////////////////
    #region Label Management
    public void PauseLabelOn()
    {
        SoundEffectClick();
        pauseLabel.SetActive(true);
        onLabel?.Invoke(false);
        timerOn = false;
    }
    public void PauseLabelOff()
    {
        SoundEffectClick();
        pauseLabel.SetActive(false);
        onLabel?.Invoke(true);

        if(time!=0)
            timerOn = true;
    }
    private void GameStateLabelOn(bool state)
    {
        gameStateLabel.SetActive(true);

        if (state)
        {
            gameStateText.text = "You Win";
            rewardBtn.SetActive(false);
        }
        else
        {
            gameStateText.text = "Game Over";
            rewardBtn.SetActive(true);
        }
        onLabel?.Invoke(false);
        currentBestScore = onGetHighScore.Invoke();
        onNewScore?.Invoke((int)currentPoints);
        timerOn = false;

        StartCoroutine(ShowFinalScore());
    }
    IEnumerator ShowFinalScore()
    {
        yield return new WaitForSeconds(textSpeedAppears);
        onTextAppearsSoundEffect?.Invoke();
        finalScore.SetActive(true);

        yield return new WaitForSeconds(textSpeedAppears);
        onTextAppearsSoundEffect?.Invoke();
        finalScoreNum.SetActive(true);
        finalScoreNumText.text = scoreText.text;

        yield return new WaitForSeconds(textSpeedAppears);
        onTextAppearsSoundEffect?.Invoke();
        bestScore.SetActive(true);

        yield return new WaitForSeconds(textSpeedAppears);
        onTextAppearsSoundEffect?.Invoke();
        bestScoreNum.SetActive(true);
        bestScoreNumText.text = currentBestScore.ToString();

        
        if (currentBestScore< currentPoints)
        {
            Animator bestScoreAnimator = bestScoreNum.GetComponent<Animator>();

            yield return new WaitForSeconds(textSpeedAppears);
            //onTextAppearsSoundEffect?.Invoke();
            onNewHighScoreSE?.Invoke();
            bestScoreNumText.text = scoreText.text;
            bestScoreAnimator.SetTrigger("NewScore");
            yield return new WaitForSeconds(textSpeedAppears);
            //onNewHighScoreSE?.Invoke();
            newText.SetActive(true);
        }
    }
    private void GameStateLabelOff()
    {
        GameStateLabelReset();
        rewardedAdsButton.SetActive(false);
        gameStateLabel.SetActive(false);
        onLabel?.Invoke(true);
        timerOn = true;
    }
    private void GameStateLabelReset()
    {
        gameStateLabel.SetActive(false);
        finalScore.SetActive(false);
        finalScoreNum.SetActive(false);
        bestScore.SetActive(false);
        bestScoreNum.SetActive(false);
        newText.SetActive(false);
    }
    #endregion
    /////////////////////////////
    #region Sound Management
    private void SoundEffectClick()
    {
        onClick?.Invoke();
    }
    private void SoundEffectStatus()
    {
        if (soundEffectStatus)
        {
            soundEffectOn.SetActive(true);
        }
        else
        {
            soundEffectOff.SetActive(true);
        }
    }
    public void SoundEffectButton()
    {
        if (soundEffectStatus)
        {
            soundEffectStatus = false;
            soundEffectOn.SetActive(false);
            soundEffectOff.SetActive(true);

            onSetSoundEffect?.Invoke(soundEffectStatus);
        }
        else
        { 
            soundEffectStatus = true;
            soundEffectOff.SetActive(false);
            soundEffectOn.SetActive(true);

            onSetSoundEffect?.Invoke(soundEffectStatus);
            onClick?.Invoke();
        }
    }
    #endregion

    #region Scene Management
    public void RestartScene()
    {
        onAdOn?.Invoke();
        StartCoroutine(Restart());
    }
    IEnumerator Restart()
    {
        yield return new WaitForSeconds(waitBeforeLoadScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void LoadMenuScene()
    {
        onClick?.Invoke();
        StartCoroutine(LoadMenu());
    }
    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(waitBeforeLoadScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);

    }
    #endregion
}
