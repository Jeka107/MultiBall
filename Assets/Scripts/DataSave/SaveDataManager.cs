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
    public static List<PlayerSavedData> newGame=new List<PlayerSavedData>(9);

    private string saveFolderName = "Saved_Data_JSON";
    private string saveGamePlayFileName = "save_json1.sav";
    private string saveSettingsFileName = "settings_json1.sav";

    private static bool created = false;
    private string password = "Jeka";
    private bool encrypt = true;
    private int currentMainNum=2;
    private int highScore=0;

    private bool soundEffectStatus;
    private void Awake()
    {
        soundEffectStatus = LoadDataSettingsData();

        if (!created)
        {
            if (LoadDataPlayerData() == null)
            {
                CreateList();
                SaveData();
            }
            else
            {
                newGame = LoadDataPlayerData();
            }

            SaveSettingsData();

            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            newGame = LoadDataPlayerData();
        }

        AudioController.onLastSoundEffect += GetSoundEffectStatus;

        MenuManager.onGetLastSelectedNum += GetLastSelectedNumber;
        MenuManager.onGetHighScore += GetHighScore;
        MenuManager.onSelectedNum += UpdateMainNumber;
        MenuManager.onLastSoundEffect += GetSoundEffectStatus;
        MenuManager.onSetSoundEffect += SetSoundStatus;

        GameManager.onGetHighScore += GetHighScore;
        GameManager.onNewScore += UpdateScoreList;
        GameManager.onLastSoundEffectStatus += GetSoundEffectStatus;
        GameManager.onSetSoundEffect += SetSoundStatus;
        
        MainBall.onGetmainNumber += GetMainNumber; 
    }
    private void OnDestroy()
    {
        AudioController.onLastSoundEffect -= GetSoundEffectStatus;

        MenuManager.onGetLastSelectedNum -= GetLastSelectedNumber;
        MenuManager.onGetHighScore -= GetHighScore;
        MenuManager.onSelectedNum -= UpdateMainNumber;
        MenuManager.onLastSoundEffect -= GetSoundEffectStatus;
        MenuManager.onSetSoundEffect -= SetSoundStatus;

        GameManager.onGetHighScore -= GetHighScore;
        GameManager.onNewScore -= UpdateScoreList;
        GameManager.onLastSoundEffectStatus -= GetSoundEffectStatus;
        GameManager.onSetSoundEffect -= SetSoundStatus;

        MainBall.onGetmainNumber -= GetMainNumber;     
    }
    #region PlayerDataUse$Update
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
    #endregion
    //
    #region PlayerDataSaveLoad
    public void SaveData()
    {
        string filePath = Application.persistentDataPath + "/" + saveFolderName + "/" + saveGamePlayFileName;
        
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
        //onSaveToCloud?.Invoke(true);
        sw.Write(dataInJSON);

        sw.Close();
        fs.Close();
    }
    
    public List<PlayerSavedData> LoadDataPlayerData()
    {
        string filePath = Application.persistentDataPath + "/" + saveFolderName + "/" + saveGamePlayFileName;

        string dataToLoad = "";

        if (System.IO.File.Exists(filePath))
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);

            StreamReader sr = new StreamReader(fs);

            dataToLoad = sr.ReadToEnd();
            //onSaveToCloud?.Invoke(false);

            if (dataToLoad != null)
            {
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
        return null;
    }
    #endregion

    #region SettingsLoad
    private bool GetSoundEffectStatus()
    {
        return soundEffectStatus;
    }
    private void SetSoundStatus(bool _soundEffectStatus)
    {
        soundEffectStatus = _soundEffectStatus;
        SaveSettingsData();
    }
    private void SaveSettingsData()
    {
        PlayerSavedSettings dataToSave = new PlayerSavedSettings(soundEffectStatus);

        string filePath = Application.persistentDataPath + "/" + saveFolderName + "/" + saveSettingsFileName;
        
        if (!System.IO.File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        string dataInJSON = JsonUtility.ToJson(dataToSave, true);

        FileStream fs = new FileStream(filePath, FileMode.Create);

        StreamWriter sw = new StreamWriter(fs);

        sw.Write(dataInJSON);

        sw.Close();
        fs.Close();
    }
    private bool LoadDataSettingsData()
    {
        string filePath = Application.persistentDataPath + "/" + saveFolderName + "/" + saveSettingsFileName;

        string dataToLoad = "";

        if (System.IO.File.Exists(filePath))
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);

            StreamReader sr = new StreamReader(fs);

            dataToLoad = sr.ReadToEnd();

            PlayerSavedSettings loadedData =
                JsonUtility.FromJson<PlayerSavedSettings>(dataToLoad);

            soundEffectStatus = loadedData.soundEffectStatus;

            sr.Close();
            fs.Close();

            return soundEffectStatus;
        }
        else
            return true;
    }
    #endregion

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
