using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using ATDungeon.Weapons;
using ATDungeon.Items;
using ATDungeon.Classes;
using ATDungeon.Player;

namespace ATDungeon.Persistence
{
    public class PersistenceManager : MonoBehaviour
    {
        public static PersistenceManager Instance;
        private Player.Movement.PlayerView playerView;

        private string settingsDataPath;
        private string scoreDataPath;
        private string statisticsDataPath;

        //Saved During Gameplay
        private GameObject[] playerWeapons;
        private GameObject[] playerItems;
        private ClassManager.PlayerClass playerClass;
        private int playerCoins;
        private int playerTotalCoins;
        private int enemiesKilled;
        private int bossesKilled;
        private int floorsCleared;
        private float timeSurvived;

        //Saved For Game Reload
        private float playerSens;
        private ScoreData highestScore;
        private StatisticsData statistics;


        [Serializable]
        private struct SettingsData
        {
            public float savedSens;
        }

        [Serializable]
        public struct ScoreData
        {
            public string soChosenClassName;
            public int soPlayerScore;
            public int soFloorsCleared;
            public int soEnemiesKilled;
            public int soBossesKilled;
        }

        [Serializable]
        public struct StatisticsData
        {
            public int totalFloorsCleared;
            public int totalEnemiesKilled;
            public int totalBossesKilled;
            public int totalCoinsCollected;
        }

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            settingsDataPath = Application.persistentDataPath + "/settingsData.json";
            scoreDataPath = Application.persistentDataPath + "/scoreData.bin";
            statisticsDataPath = Application.persistentDataPath + "/statisticsData.bin";

            LoadSettingsData();
            LoadScoreData();
            LoadStatisticsData();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnApplicationQuit()
        {
            Debug.Log("Saving and Quitting...");
            SaveSettingsData();

            Debug.Log("Done!");
        }

        public void SessionClear()
        {
            foreach (GameObject go in playerWeapons)
                Destroy(go);

            foreach (GameObject go in playerItems)
                Destroy(go);

            playerWeapons = null;
            playerItems = null;

            playerClass = null;
            playerCoins = 0;
            enemiesKilled = 0;
            timeSurvived = 0.0f;
            floorsCleared = 0;
            bossesKilled = 0;
            Progression.ProgressionManager.Instance.ChangeScoreNeededToTeleportBy(-Progression.ProgressionManager.Instance.GetScoreNeededToTeleport());
        }


        public void SaveSettingsData()
        {
            SettingsData settingsData = new SettingsData
            {
                savedSens = playerSens
            };
            string jsonString = JsonUtility.ToJson(settingsData);
            File.WriteAllText(settingsDataPath, jsonString);
            Debug.Log("Settings saved...");
        }

        public void LoadSettingsData()
        {
            if (File.Exists(settingsDataPath))
            {
                string saveString = File.ReadAllText(settingsDataPath);
                SettingsData settings = JsonUtility.FromJson<SettingsData>(saveString);
                playerSens = settings.savedSens;
            }
        }

        public void SaveScoreData()
        {
            Debug.Log("Saving Data...");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Create(scoreDataPath);
            bf.Serialize(stream, highestScore);
            Debug.Log("Data Saved At: " + stream.Name);
            stream.Close();
        }

        public void LoadScoreData()
        {
            if (File.Exists(scoreDataPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(scoreDataPath, FileMode.Open);
                highestScore = (ScoreData)bf.Deserialize(file);
                file.Close();
            }
        }

        public void SaveStatisticsData()
        {
            Debug.Log("Saving Data...");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Create(statisticsDataPath);
            bf.Serialize(stream, statistics);
            Debug.Log("Data Saved At: " + stream.Name);
            stream.Close();
        }

        public void LoadStatisticsData()
        {
            if (File.Exists(statisticsDataPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(statisticsDataPath, FileMode.Open);
                statistics = (StatisticsData)bf.Deserialize(file);
                file.Close();
            }
        }

        public void CalculateScore()
        {
            int enemyScore = enemiesKilled * Progression.ProgressionManager.Instance.GetScorePerEnemy();
            int bossScore = bossesKilled * Progression.ProgressionManager.Instance.GetScorePerBoss();
            int floorScore = floorsCleared * Progression.ProgressionManager.Instance.GetScorePerFloorClear();

            int finalScore = (enemyScore + bossScore) * floorScore;

            if (finalScore > highestScore.soPlayerScore)
            {
                ScoreData scoreObject = new ScoreData
                {
                    soChosenClassName = playerClass.GetClassName(),
                    soPlayerScore = finalScore,
                    soBossesKilled = bossesKilled,
                    soEnemiesKilled = enemiesKilled,
                    soFloorsCleared = floorsCleared
                };

                highestScore = scoreObject;
                SaveScoreData();
            }

            statistics.totalBossesKilled += bossesKilled;
            statistics.totalEnemiesKilled += enemiesKilled;
            statistics.totalFloorsCleared += floorsCleared;
            statistics.totalCoinsCollected += playerTotalCoins;

            Debug.Log("Statistics: " + statistics.totalBossesKilled);
            Debug.Log("Statistics: " + statistics.totalEnemiesKilled);
            Debug.Log("Statistics: " + statistics.totalCoinsCollected);
            SaveStatisticsData();
        }

        public void SetPlayerWeapons(GameObject[] weapons)
        {
            playerWeapons = new GameObject[weapons.Length];
            for (int i = 0; i < playerWeapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    playerWeapons[i] = Instantiate(weapons[i], this.transform);
                    Destroy(weapons[i]);
                }
            }
        }

        public void SetPlayerItems(GameObject[] items)
        {
            playerItems = new GameObject[items.Length];
            for (int i = 0; i < playerItems.Length; i++)
            {
                if (items[i] != null)
                {
                    ItemBase ogItem = items[i].GetComponent<ItemBase>();
                    ogItem.SetCurrentState(ItemBase.ItemState.Idle);
                    playerItems[i] = Instantiate(items[i], this.transform);
                    playerItems[i].GetComponent<ItemBase>().SetCurrentUses(ogItem.GetCurrentUses());
                    Destroy(items[i]);
                }
            }
        }


        //Saved During Gameplay
        public void SetPlayerClass(ClassManager.PlayerClass pClass) { playerClass = pClass; }
        public void SetPlayerCoins(int n) { playerCoins = n; }
        public void SetPlayerTotalCoins(int n) { playerTotalCoins = n; }
        public void SetEnemiesKilled(int n) { enemiesKilled = n; }
        public void SetBossesKilled(int n) { bossesKilled = n; }
        public void SetFloorsCleared(int n) { floorsCleared = n; }
        public void SetTimeSurvived(float n) { timeSurvived = n; }

        public ClassManager.PlayerClass GetPlayerClass() { return playerClass; }

        public GameObject[] GetPlayerWeapons() { return playerWeapons; }
        public GameObject[] GetPlayerItems() { return playerItems; }
        public int GetPlayerCoins() { return playerCoins; }
        public int GetPlayerTotalCoins() { return playerTotalCoins; }
        public int GetEnemiesKilled() { return enemiesKilled; }
        public int GetBossesKilled() { return bossesKilled; }
        public int GetFloorsCleared() { return floorsCleared; }
        public float GetTimeSurvived() { return timeSurvived; }


        //Saved For Game Reload
        public void SetPlayerSens(float sens) { playerSens = sens; }

        public float GetPlayerSens() { if (playerSens > 0.1f) { return playerSens; } else return 0.1f; }
        public ScoreData GetHighScoreData() { return highestScore; }
        public StatisticsData GetStatisticsData() { return statistics; }

        //Other
        public void SetupPlayerView() { playerView = Utility.UtilFunctions.instance.GetPlayerObj().GetComponentInChildren<Player.Movement.PlayerView>(); }
        public void UpdateSensitivity() { if (playerView) { playerView.UpdateSensitivity(); } }
    }
}
