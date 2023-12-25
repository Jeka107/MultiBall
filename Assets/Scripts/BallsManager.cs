using System.Collections.Generic;
using UnityEngine;

public class BallsManager : MonoBehaviour
{
    public delegate void OnMaxMultiplier (bool state);
    public static event OnMaxMultiplier onMaxMultiplier;
    public delegate void OnSecondChance();
    public static event OnSecondChance onSecondChance;


    [SerializeField] private GameObject mainBall;
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform ballsParent;
    [SerializeField] private List<Material> materials = new List<Material>();
    [SerializeField] private int maxMultiplier;

    [SerializeField] private List<GameObject> balls = new List<GameObject>();
    private List<int> rows=new List<int>{ 1,2,3 };
    private List<int> colms = new List<int> { 1, 2, 3 };
    private List<int> exceptionsNumbers = new List<int>();
    private List<int> materialNums;

    private int mainNum;
    private Vector3 mainBallPosition;
    //private GameObject currentBall;
    private int currentBall=0;
    private Vector3 spawnPosition;
    private int currentMult = 2;
    private int round=0;
    private Vector3 lastMainBallPos;

    private void Awake()
    {
        MainBall.onDestination += DestroyBalls;
        RewardedAdsButton.onSecondSchance += SecondChance;

        CreateListOfBalls();
    }
    private void OnDestroy()
    {
        MainBall.onDestination -= DestroyBalls;
        RewardedAdsButton.onSecondSchance -= SecondChance;
    }
    private void Start()
    {
        mainNum = mainBall.GetComponent<MainBall>().GetMainNum();

        CreateBalls();
    }
    private void CreateListOfBalls()
    {
        for (int i = 0; i < 9; i++)
        {
            balls.Add(Instantiate(ball, Vector3.zero, Quaternion.identity, ballsParent));
        }
    }
    private void CreateMaterialNums()
    {
        materialNums = new List<int>();

        for (int i=0;i<materials.Count;i++)
        {
            materialNums.Add(i);
        }
    }
    private void DestroyBalls()
    {
        foreach (GameObject ball in balls)
        {
            currentBall = 0;
            ball.SetActive(false);
        }

        if (round != maxMultiplier)
        {
            CreateBalls();
            round++;
        }
        else
            onMaxMultiplier?.Invoke(true);
    }
    private void CreateBalls()
    {
        float randBall;

        lastMainBallPos = mainBall.transform.position;

        CreateMaterialNums();
        mainBallPosition = mainBall.transform.position;

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
        SetActiveBalls();
    }
    private void SetActiveBalls()
    {
        foreach (GameObject ball in balls)
        {
            ball.SetActive(true);
        }
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
                randy = Random.Range(14f, 18f);
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

        balls[currentBall].transform.position = spawnPosition;
        currentBall++;
    }

    private void PutInNumber(GameObject ball,bool correctNumber)
    {
        int randNum=0;
        int randMaterial = Random.Range(0, materialNums.Count);
        Ball currentBall = ball.GetComponent<Ball>();

        if (correctNumber)
        {
            currentBall.SetNumber(mainNum * currentMult);
            currentBall.SetTimerOn();
            currentBall.SetMaterial(materials[materialNums[randMaterial]]);
            materialNums.RemoveAt(randMaterial);
            exceptionsNumbers.Add(mainNum * currentMult);
            currentMult++;
        }
        else
        {
            if(mainNum<4)
                randNum = Random.Range((mainNum * (currentMult - 2))+1, mainNum * (currentMult + 3));
            else
                randNum = Random.Range((mainNum * (currentMult - 2))+1, mainNum * (currentMult + 1));

            
            for (int i = 0; i < exceptionsNumbers.Count; i++)
            {
                if (exceptionsNumbers[i] == randNum)
                {
                    PutInNumber(ball,false);
                    return;
                }
            }
            currentBall.SetNumber(randNum);
            currentBall.SetMaterial(materials[materialNums[randMaterial]]);
            materialNums.RemoveAt(randMaterial);
            exceptionsNumbers.Add(randNum);
        }
    }
    private void SecondChance()
    {
        mainBall.transform.position = new Vector3(lastMainBallPos.x, lastMainBallPos.y, lastMainBallPos.z);
        onSecondChance?.Invoke();
    }
    
}
