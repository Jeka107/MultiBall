using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSavedData
{
    public int mainNumber;
    public int highScore;

    public PlayerSavedData()
    {
        mainNumber = 0;
        highScore = 0;
    }

    public PlayerSavedData(int _mainNumber,int _score)
    {
        mainNumber = _mainNumber;
        highScore = _score;
    }
}
