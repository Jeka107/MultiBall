using UnityEngine;

public class AudioController : MonoBehaviour
{
    public delegate bool OnLastSoundEffect();
    public static event OnLastSoundEffect onLastSoundEffect;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip ballMerged;
    [SerializeField] private AudioClip airSweep;
    [SerializeField] private AudioClip textAppears;
    [SerializeField] private AudioClip newHighScore;
    [SerializeField] private AudioClip progress;



    private bool soundEffectStatus;
    private static bool created = false;

    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        MenuManager.onSetSoundEffect += SetSoundEffectStatus;
        GameManager.onSetSoundEffect += SetSoundEffectStatus;

        MenuManager.onClick += ClickButton;
        GameManager.onClick += ClickButton;

        GameManager.onTextAppearsSoundEffect += TextAppears;
        GameManager.onNewHighScoreSE += NewHighScore;
        GameManager.onProgressSE += ProgressText;
        MainBall.onAirSweepSound += AirSweep;
        MainBall.onMainBallMergedSE += BallMerged;
    }
    private void Start()
    {
        soundEffectStatus = onLastSoundEffect.Invoke();
    }
    private void OnDestroy()
    {
        MenuManager.onSetSoundEffect -= SetSoundEffectStatus;
        GameManager.onSetSoundEffect -= SetSoundEffectStatus;

        MenuManager.onClick -= ClickButton;
        GameManager.onClick -= ClickButton;

        GameManager.onTextAppearsSoundEffect -= TextAppears;
        GameManager.onNewHighScoreSE -= NewHighScore;
        GameManager.onProgressSE -= ProgressText;
        MainBall.onAirSweepSound -= AirSweep;
        MainBall.onMainBallMergedSE -= BallMerged;
    }
    private void SetSoundEffectStatus(bool _soundEffectStatus)
    {
        soundEffectStatus = _soundEffectStatus;
    }
    private void ClickButton()
    {
        if (soundEffectStatus)
        {
            audioSource.volume = 0.6f;
            audioSource.PlayOneShot(click);
        }
    }
    private void BallMerged()
    {
        if (soundEffectStatus)
        {
            audioSource.priority = 128;
            audioSource.volume = 1f;
            audioSource.PlayOneShot(ballMerged);
        }
    }
    private void AirSweep()
    {
        if (soundEffectStatus)
        {
            audioSource.priority = 128;
            audioSource.volume = 1f;
            audioSource.PlayOneShot(airSweep);
        }
    }
    private void TextAppears()
    {
        if (soundEffectStatus)
        {
            audioSource.volume = 1f;
            audioSource.PlayOneShot(textAppears);
        }
    }
    private void NewHighScore()
    {
        if (soundEffectStatus)
        {
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(newHighScore);
        }
    }
    private void ProgressText()
    {
        if (soundEffectStatus)
        {
            audioSource.priority = 130;
            audioSource.volume = 1f;

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(progress);
            }
            else
            {
                audioSource.Stop();
                audioSource.PlayOneShot(progress);
            }
        }
    }
}
