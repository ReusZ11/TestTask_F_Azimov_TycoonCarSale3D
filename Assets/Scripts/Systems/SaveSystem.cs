using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

public class SaveSystem : BaseSystem
{
    [SerializeField] private float autoSaveInterval = 300f; 
    private float lastSaveTime;

    public override void Initialize()
    {
        base.Initialize();
        lastSaveTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - lastSaveTime > autoSaveInterval)
        {
            SaveGame();
            lastSaveTime = Time.time;
        }
    }

    public void SaveGame()
    {
        GameData gameData = new GameData();

        gameData.playerData = GameManager.Instance.GetComponent<ResourceSystem>().GetPlayerData();
        gameData.inventory = GameManager.Instance.GetComponent<InventorySystem>().GetInventoryData();
        gameData.parking = GameManager.Instance.GetComponent<BuildSystem>().GetParkingData();
        gameData.lastSaveTime = DateTime.Now.ToString();

        string json = JsonUtility.ToJson(gameData, true);
        string path = Application.persistentDataPath + "/savegame.json";

        try
        {
            File.WriteAllText(path, json);
            Debug.Log("Game saved successfully to: " + path);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.json";

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                GameData gameData = JsonUtility.FromJson<GameData>(json);

                GameManager.Instance.GetComponent<ResourceSystem>().SetPlayerData(gameData.playerData);
                GameManager.Instance.GetComponent<InventorySystem>().SetInventoryData(gameData.inventory);
                GameManager.Instance.GetComponent<BuildSystem>().SetParkingData(gameData.parking);

                Debug.Log("Game loaded successfully from: " + path);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
            }
        }
        else
        {
            Debug.Log("No save file found, starting new game");
        }
    }
    public bool HasSaveData()
    {
        string path = Application.persistentDataPath + "/savegame.json";
        return File.Exists(path);
    }


    [System.Serializable]
    private class GameData
    {
        public PlayerData playerData;
        public InventoryData inventory;
        public BuildingData parking;
        public List<EmployeeData> employees;
        public string lastSaveTime;
    }


}