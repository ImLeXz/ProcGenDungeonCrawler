using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ATDungeon.SceneManagement;

namespace ATDungeon.UI.Menus
{
    public class StatisticsUI : MonoBehaviour
    {
        [System.Serializable]
        private struct StatReferences
        {
            public TextMeshProUGUI enemiesKilledTxt;
            public TextMeshProUGUI bossesKilledTxt;
            public TextMeshProUGUI scoreTxt;
            public TextMeshProUGUI coinsTxt;
            public TextMeshProUGUI floorsClearedTxt;
            public TextMeshProUGUI playerClassTxt;
        }

        [Header("Stats References")]
        [SerializeField]
        private StatReferences highScoreStats;
        [SerializeField]
        private StatReferences allTimeStats;

        private void Start()
        {
            InitialiseHighScore();
            InitialiseStatistics();
        }

        private void InitialiseHighScore()
        {
            Persistence.PersistenceManager.ScoreData highScoreData = Persistence.PersistenceManager.Instance.GetHighScoreData();

            highScoreStats.bossesKilledTxt.text = "Bosses Killed: " + highScoreData.soBossesKilled;
            highScoreStats.enemiesKilledTxt.text = "Enemies Killed: " + highScoreData.soEnemiesKilled;
            highScoreStats.floorsClearedTxt.text = "Floors Cleared: " + highScoreData.soFloorsCleared;
            highScoreStats.scoreTxt.text = "Final Score: " + highScoreData.soPlayerScore;
            highScoreStats.playerClassTxt.text = "Chosen Class: " + highScoreData.soChosenClassName;
        }

        private void InitialiseStatistics()
        {
            Persistence.PersistenceManager.StatisticsData statistics = Persistence.PersistenceManager.Instance.GetStatisticsData();
            allTimeStats.bossesKilledTxt.text = "Bosses Killed: " + statistics.totalBossesKilled;
            allTimeStats.enemiesKilledTxt.text = "Enemies Killed: " + statistics.totalEnemiesKilled;
            allTimeStats.floorsClearedTxt.text = "Floors Cleared: " + statistics.totalFloorsCleared;
            allTimeStats.coinsTxt.text = "Coins Collected: " + statistics.totalCoinsCollected;
        }

        public void BackBtnPressed()
        {
            int buildIndex = DungeonSceneManager.Instance.GetMainMenuSceneBuildIndex();
            DungeonSceneManager.Instance.UnloadScene(this.gameObject.scene);
        }

    }
}
