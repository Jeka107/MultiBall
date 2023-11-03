using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsManager : MonoBehaviour
{
    [SerializeField] private GameObject mainBall;
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform ballsParent;
     

    private List<GameObject> balls = new List<GameObject>();
    private List<int> rows=new List<int>{ 1,2,3 };
    private List<int> colms = new List<int> { 1, 2, 3 };
    private List<int> exceptionsNumbers = new List<int>();
    private int mainNum;
    private Vector3 mainBallPosition;
    private GameObject currentBall;
    private Vector3 spawnPosition;
    private int currentMult = 2;

    private void Awake()
    {
        MainBall.onDestination += DestroyBalls;
    }
    private void OnDestroy()
    {
        MainBall.onDestination -= DestroyBalls;
    }
    private void Start()
    {
        mainNum = mainBall.GetComponent<MainBall>().GetMainNum();

        CreateBalls();
    }
    private void DestroyBalls()
    {
        foreach (GameObject ball in balls)
        {
            Destroy(ball);
        }
        balls.Clear();
        CreateBalls();
    }
    private void CreateBalls()
    {
        mainBallPosition = mainBall.transform.position;
        float randBall;

        for (int i = 0; i <= 2; i++)
        {
            int randRow = Random.Range(0, rows.Count - 1);

            if (rows.Count != 0)
            {
                for (int j = 0; j <= 2; j++)
                {
                    PositionBalls(rows[i], colms[j]);
                }
            }
        }

        randBall = Random.Range(0, balls.Count - 1);
        PutInNumber(balls[(int)randBall], true);

        for (int j=0;j<balls.Count;j++)
        {
            if (j != (int)randBall)
                PutInNumber(balls[j], false);
        }

        exceptionsNumbers.Clear();
    }
    private void PositionBalls(int rowNumber,int colmsNumber)
    {
        float randx=0;
        float randy=0;
        float randz;

        spawnPosition.z = mainBallPosition.z;

        switch (rowNumber)
        {
            case 1:
                randy= Random.Range(-1f,2f);
                break;
            case 2:
                randy = Random.Range(6f,10f);
                break;
            case 3:
                randy = Random.Range(14f, 20f);
                break;
        }
        switch(colmsNumber)
        {
            case 1:
                randx = Random.Range(Mathf.Round(mainBallPosition.x - 6f), Mathf.Round(mainBallPosition.x - 5f));
                break;
            case 2:
                randx = Random.Range(Mathf.Round(mainBallPosition.x - 2f), Mathf.Round(mainBallPosition.x + 2f));
                break;
            case 3:
                randx = Random.Range(Mathf.Round(mainBallPosition.x + 5f), Mathf.Round(mainBallPosition.x + 6f));
                break;
        }
        randz = Random.Range(10f, 20f);
        
        spawnPosition.y = randy + mainBallPosition.y;
        spawnPosition.x = randx;
        spawnPosition.z += (int)randz;

        currentBall =Instantiate(ball, spawnPosition, Quaternion.identity, ballsParent);
        balls.Add(currentBall);
    }

    private void PutInNumber(GameObject ball,bool correctNumber)
    {
        int randNum;

        if (correctNumber)
        {
            ball.GetComponent<Ball>().SetNumber(mainNum * currentMult);
            exceptionsNumbers.Add(mainNum * currentMult);
            currentMult++;
        }
        else
        {
            randNum = Random.Range((mainNum * (currentMult - 2))+1, mainNum * (currentMult + 4));

            for (int i = 0; i < exceptionsNumbers.Count; i++)
            {
                if (exceptionsNumbers[i] == randNum)
                {
                    PutInNumber(ball,false);
                    return;
                }
            }
            ball.GetComponent<Ball>().SetNumber(randNum);
            exceptionsNumbers.Add(randNum);
        }
    }
    
}
