using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public delegate void OnSelectedNum(int selectedNum);
    public static event OnSelectedNum onSelectedNum;

    public delegate int OnGetHighScore();
    public static event OnGetHighScore onGetHighScore;

    public delegate int OnGetLastSelectedNum();
    public static event OnGetLastSelectedNum onGetLastSelectedNum;

    public delegate bool OnLastSoundEffect();
    public static event OnLastSoundEffect onLastSoundEffect;

    public delegate void OnSetSoundEffect(bool soundEffectStatus);
    public static event OnSetSoundEffect onSetSoundEffect;

    public delegate void OnClick();
    public static event OnClick onClick;

    /////////////////////////////
    [Space]
    [Header("Scene Management")]
    [SerializeField] private float waitBeforeLoadScene;
    /////////////////////////////
    [Space]
    [Header("UI Managment")]
    [SerializeField] private int selectedNum;
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private Transform numberButtons;
    [Space]
    [Header("Sound Management")]
    [SerializeField] private GameObject soundEffectOn;
    [SerializeField] private GameObject soundEffectOff;

    private bool soundEffectStatus;

    private void Awake()
    {
        selectedNum = onGetLastSelectedNum.Invoke();
        highScore.text=onGetHighScore?.Invoke().ToString();
        soundEffectStatus = onLastSoundEffect.Invoke();

        if (selectedNum == 0)
        {
            selectedNum = 2;
            onSelectedNum?.Invoke(selectedNum);
        }

        BoldSelectedNumber(selectedNum);
        SoundEffectStatus();
    }

    #region Scene Management
    public void LoadNextScene()
    {
        SoundEffectClick();
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        yield return new WaitForSeconds(waitBeforeLoadScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion

    #region Button Management
    public void SelectMainNumber(TextMeshProUGUI _selectedNum)
    {
        SoundEffectClick();
        selectedNum = int.Parse(_selectedNum.text);

        BoldSelectedNumber(selectedNum);
        onSelectedNum?.Invoke(selectedNum);

        highScore.text = onGetHighScore?.Invoke().ToString();
    }
    private void BoldSelectedNumber(int selectedNum)
    {
        
        int i = 2;

        foreach (Transform transform in numberButtons)
        {
            if (selectedNum != i)
                transform.GetComponent<Image>().color = new Color32(48, 46, 185, 255);
            else
                transform.GetComponent<Image>().color = new Color32(207, 83, 87, 255);
            i++;
        }      
    }
    #endregion

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
}
