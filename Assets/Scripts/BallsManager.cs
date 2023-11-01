using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsManager : MonoBehaviour
{
    [SerializeField] private GameObject mainBall;
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform ballsParent;
    [SerializeField] private int mainNum;

    private List<GameObject> balls = new List<GameObject>();
    private List<int> rows=new List<int>{ 1,2,3,4,5,6 };
    private List<int> exceptionsNumbers = new List<int>();
    private Vector3 mainBallPosition;
    private GameObject currentBall;
    private Vector3 spawnPosition;
    private int currentMult = 2;
    private bool correctNumber = false;

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

        for (int i = 0; i <= 5; i++)
        {
            int randRow = Random.Range(0, rows.Count - 1);

            if (rows.Count != 0)
            {
                PositionBalls(rows[randRow]);
                rows.RemoveAt(randRow);
            }
        }
        rows = new List<int> { 1, 2, 3, 4, 5, 6 };
        exceptionsNumbers.Clear();
        correctNumber = false;
    }
    private void PositionBalls(int rowNumber)
    {
        float randx = Random.Range(Mathf.Round(mainBallPosition.x - 6f), Mathf.Round(mainBallPosition.x + 6f));
        
        spawnPosition.x = randx;
        spawnPosition.z = mainBallPosition.z;

        switch (rowNumber)
        {
            case 1:
                spawnPosition.y = 0.5f+ mainBallPosition.y;
                spawnPosition.z += 10;
                break;
            case 2:
                spawnPosition.y = 4.5f + mainBallPosition.y;
                spawnPosition.z += 15;
                break;
            case 3:
                spawnPosition.y = 8f + mainBallPosition.y;
                spawnPosition.z += 20;
                break;
            case 4:
                spawnPosition.y = 11.5f + mainBallPosition.y;
                spawnPosition.z += 20;
                break;
            case 5:
                spawnPosition.y = 15f + mainBallPosition.y;
                spawnPosition.z += 15;
                break;
            case 6:
                spawnPosition.y = 18f + mainBallPosition.y;
                spawnPosition.z += 10;
                break;
        }

        currentBall=Instantiate(ball, spawnPosition, Quaternion.identity, ballsParent);
        balls.Add(currentBall);
        PutInNumber(currentBall);
    }
    private void PutInNumber(GameObject ball)
    {
        if (!correctNumber)
        {
            ball.GetComponent<Ball>().SetNumber(mainNum * currentMult);
            exceptionsNumbers.Add(mainNum * currentMult);
            currentMult++;
            correctNumber = true;
        }
        else
        {
            int randNum = Random.Range(mainNum * (currentMult-2), mainNum * (currentMult + 2));

            for (int i = 0; i < exceptionsNumbers.Count; i++)
            {
                if (exceptionsNumbers[i] == randNum)
                {
                    PutInNumber(ball);
                    return;
                }
            }

            ball.GetComponent<Ball>().SetNumber(randNum);
            exceptionsNumbers.Add(randNum);
        }
    }
    
}
