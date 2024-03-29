using System.Collections;
using UnityEngine;
using TMPro;

public class MainBall : MonoBehaviour
{
    //Move ball to destination.
    public delegate void OnDestination();
    public static event OnDestination onDestination;
    public delegate void OnAirSweepSound();
    public static event OnAirSweepSound onAirSweepSound;
    public delegate void OnCreateNewBalls();
    public static event OnCreateNewBalls onCreateNewBalls;

    //GameOver.
    public delegate void OnGameOver(bool state);
    public static event OnGameOver onGameOver;

    //Text canvas update.
    public delegate void OnPlayerProgress(int multiply);
    public static event OnPlayerProgress onPlayerProgress;

    //Get main number.
    public delegate int OnGetmainNumber();
    public static event OnGetmainNumber onGetmainNumber;

    //Move background
    public delegate void OnMoveBackground(Vector3 distance);
    public static event OnMoveBackground onMoveBackground;

    //Sound Effect
    public delegate void OnMainBallMergedSE();
    public static event OnMainBallMergedSE onMainBallMergedSE;

    [SerializeField] private float moveSpeed;
    [SerializeField] private TextMeshProUGUI textMainNumber;
    [SerializeField] private float forceCollision;

    private bool clicked=false;
    private Transform transformClicked;
    private Vector3 distance;
    private int mainNum;
    private GameObject ballClicked;
    private int numClicked;
    private int currentMult=2;

    private void Awake()
    {
        mainNum = onGetmainNumber.Invoke();
        textMainNumber.text = mainNum.ToString();

        PlayerTouch.onBallPressed += MoveMain;
    }
    private void OnDestroy()
    {
        PlayerTouch.onBallPressed -= MoveMain;
    }
    private void Update()
    {
        if (clicked)
        {
            transform.position = Vector3.MoveTowards(transform.position, transformClicked.position, moveSpeed * Time.deltaTime);

            if (transformClicked.position == transform.position)
            {
                clicked = false;
            }
        }
    }
    IEnumerator WaitBeforeCreateNewBalls()
    {
        onDestination?.Invoke();
        yield return new WaitForSeconds(0.05f);
        onCreateNewBalls?.Invoke();
    }
    public int GetMainNum()
    {
        return mainNum;
    }
    private void MoveMain(Transform _transformClicked,GameObject ball)
    {
        onAirSweepSound?.Invoke();

        transformClicked = _transformClicked;
        distance = transformClicked.position - transform.position;
        onMoveBackground?.Invoke(distance);

        clicked = true;

        ballClicked = ball;
        numClicked = int.Parse(ballClicked.GetComponent<Ball>().GetNumber().text);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball" && collision.gameObject == ballClicked)
        {
            if (mainNum * currentMult == numClicked)
            {
                collision.gameObject.SetActive(false);
                onMainBallMergedSE?.Invoke();//Sound Effect
                currentMult++;

                onPlayerProgress?.Invoke(currentMult);
                StartCoroutine(WaitBeforeCreateNewBalls());
            }
            else
            {
                collision.gameObject.SetActive(false);
                onGameOver?.Invoke(false);
            }
        }
    }
}
