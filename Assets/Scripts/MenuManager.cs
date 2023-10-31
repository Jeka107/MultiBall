using System.Collections;
using System.Collections.Generic;
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

    /////////////////////////////
    [Header("Scene Management")]
    [SerializeField] private float waitBeforeLoadScene;
    /////////////////////////////
    [Header("UI Managment")]
    [SerializeField] private int selectedNum;
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private Transform numberButtons;

    private void Awake()
    {
        selectedNum = onGetLastSelectedNum.Invoke();
        highScore.text=onGetHighScore?.Invoke().ToString();

        BoldSelectedNumber(selectedNum);
    }

    #region Scene Management
    public void LoadNextScene()
    {
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
        int selectedNum = int.Parse(_selectedNum.text);

        BoldSelectedNumber(selectedNum);
        onSelectedNum?.Invoke(selectedNum);
        highScore.text = onGetHighScore?.Invoke().ToString();
    }
    private void BoldSelectedNumber(int selectedNum)
    {
        int i = 2;

        foreach(Transform transform in numberButtons)
        {
            if (selectedNum != i)
                transform.GetComponent<Image>().color = new Color32(255, 255, 255, 255);  
            else
                transform.GetComponent<Image>().color = new Color32(220, 220, 220, 255);
            i++;
        }
    }

    #endregion
}
