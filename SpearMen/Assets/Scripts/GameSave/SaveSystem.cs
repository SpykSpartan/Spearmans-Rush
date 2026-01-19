using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveLocation playerLocation;
        public PlayerSaveStats playerStats;
    }

    public static string SaveFileName()
    {
        string saveFolder = Application.persistentDataPath;
        string saveFile = Path.Combine(saveFolder, "save.save");

        Debug.Log("Save file path: " + saveFile);

        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
            Debug.Log("Directory Created at: " + saveFolder);
        }
        else
        {
            Debug.Log("Directory already exists at: " + saveFolder);
        }

        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData()
    {
        gameManager.Instance.PlayerMovement.Save(ref _saveData.playerLocation);
        gameManager.Instance.PlayerStatics.Save(ref _saveData.playerStats);
    }

    public static void Load()
    {
        string path = SaveFileName();

        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found at: " + path);
            return;
        }

        string saveContent = File.ReadAllText(path);
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        gameManager.Instance.PlayerMovement.Load(_saveData.playerLocation);
        gameManager.Instance.PlayerStatics.Load(_saveData.playerStats);
    }
}
