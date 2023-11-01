using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper?.Items;
    }

    public static string ToJson<T>(List<T> list)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = list;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(List<T> list, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = list;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}

public class SaveDataManager : MonoBehaviour
{
    private static List<PlayerSavedData> newGame=new List<PlayerSavedData>(9);

    private string saveFolderName = "Saved_Data_JSON";
    private string saveFileName = "save_json1.sav";

    private static bool created = false;
    private string password = "Jeka";
    private bool encrypt = true;
    private int currentMainNum;
    private int highScore=0;

    private void Awake()
    {
        if (!created)
        {
            if (LoadData() == null)
            {
                CreateList();
                SaveData();
            }
            else
            {
                newGame = LoadData();
            }

            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            newGame = LoadData();
            Debug.Log(1);
        }

        MenuManager.onGetLastSelectedNum += GetLastSelectedNumber;
        MenuManager.onGetHighScore += GetHighScore;
        MenuManager.onSelectedNum += UpdateMainNumber;
        MainBall.onGetmainNumber += GetMainNumber;
        GameManager.onNewScore += UpdateScoreList;
    }
    private void OnDestroy()
    {
        MenuManager.onGetLastSelectedNum -= GetLastSelectedNumber;
        MenuManager.onGetHighScore -= GetHighScore;
        MenuManager.onSelectedNum -= UpdateMainNumber;
        MainBall.onGetmainNumber -= GetMainNumber;
        GameManager.onNewScore -= UpdateScoreList;
    }
    private void CreateList()
    {
        for(int i=0;i<9;i++)
        {
            newGame.Add(null);
        }
    }

    //Update//
    public void UpdateMainNumber(int mainNum)
    {
        currentMainNum = mainNum;
        highScore= newGame[currentMainNum - 1].highScore;

        newGame[0] = new PlayerSavedData(currentMainNum, highScore);

        SaveData();
    }
    public void UpdateScoreList(int score)
    {
        if (score > newGame[currentMainNum - 1].highScore)
            newGame[currentMainNum - 1] = new PlayerSavedData(currentMainNum, score);
        newGame[0] = newGame[currentMainNum - 1];

        SaveData();
    }

    //Get//
    public int GetLastSelectedNumber()
    {
        if(newGame!=null)
            currentMainNum= newGame[0].mainNumber;

        return currentMainNum;
    }
    public int GetMainNumber()
    {
        return currentMainNum;
    }
    public int GetHighScore()
    {
        highScore = newGame[0].highScore;

        return highScore;
    }
    //
    public void SaveData()
    {
        string filePath = Application.persistentDataPath + "/" + saveFolderName + "/" + saveFileName;

        if (!System.IO.File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        string dataInJSON = JsonHelper.ToJson(newGame, true);
        FileStream fs = new FileStream(filePath, FileMode.Create);

        StreamWriter sw = new StreamWriter(fs);

        if (encrypt)
        {
            dataInJSON = EncryptDecrypt(dataInJSON);
        }

        sw.Write(dataInJSON);

        sw.Close();
        fs.Close();
    }
    public List<PlayerSavedData> LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + saveFolderName + "/" + saveFileName;

        string dataToLoad = "";

        if (System.IO.File.Exists(filePath))
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);

            StreamReader sr = new StreamReader(fs);

            dataToLoad = sr.ReadToEnd();

            if (encrypt)
            {
                dataToLoad = EncryptDecrypt(dataToLoad);
            }

            List<PlayerSavedData> loadedData = JsonHelper.FromJson<PlayerSavedData>(dataToLoad);

            sr.Close();
            fs.Close();

            return loadedData;
        }
        else
            return null;

    }
    private string EncryptDecrypt(string data)
    {
        string newData = "";

        for (int i = 0; i < data.Length; i++)
        {
            newData += (char)(data[i] ^ password[i % password.Length]);
        }

        return newData;
    }

}
