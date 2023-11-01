using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainBall : MonoBehaviour
{
    //Move ball to destination.
    public delegate void OnDestination();
    public static event OnDestination onDestination;

    //GameOver.
    public delegate void OnGameOver();
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

    [SerializeField] private float moveSpeed;
    [SerializeField] private TextMeshProUGUI textMainNumber;
    [SerializeField] private float forceCollision;

    private Rigidbody rb;
    private bool clicked=false;
    private Transform transformClicked;
    private Vector3 distance;
    private int mainNum;
    private GameObject ballClicked;
    private int numClicked;
    private int currentMult=2;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //mainNum = int.Parse(textMainNumber.text);
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
        if (transformClicked && clicked)
        {
            transform.position = Vector3.MoveTowards(transform.position, transformClicked.position, moveSpeed * Time.deltaTime);

            if (transformClicked.position == transform.position)
                clicked = false;
        }
    }
    public int GetMainNum()
    {
        return mainNum;
    }
    private void MoveMain(Transform _transformClicked,GameObject ball)
    {
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
            Destroy(collision.gameObject);
            if (mainNum * currentMult == numClicked)
            {
                currentMult++;
                onPlayerProgress?.Invoke(currentMult);
                onDestination?.Invoke();
            }
            else
            {
                onGameOver?.Invoke();
            }
        }
    }
}
