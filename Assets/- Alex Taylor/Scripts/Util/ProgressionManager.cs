using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Progression
{
    public class ProgressionManager : MonoBehaviour
    {
        public enum DifficultyOption
        {
            Easy,
            Normal,
            Hard
        };
        
        public static ProgressionManager Instance;

        [Header("Difficulty Modifiers")]
        [SerializeField]
        private int additionalRoomsPerClear = 10;
        [SerializeField]
        private int enemyHealthIncrease = 10;
        [SerializeField]
        private float enemyDamageMultiplierIncrease = 0.15f;
        
        [Space(10)]
        [SerializeField] private DifficultyOption selectedDifficulty;
        [SerializeField] private float easyDamageMultiplier;
        [SerializeField] private float normalDamageMultiplier;
        [SerializeField] private float hardDamageMultiplier;

        [Header("Score Modifiers")]
        [SerializeField]
        private int scorePerEnemy = 50;
        [SerializeField]
        private int scorePerBoss = 500;
        [SerializeField]
        private int scorePerFloorClear = 1000;

        [Header("Debug")]
        [SerializeField]
        private int scoreNeededToTeleport = 0;

        private void Awake()
        {
            Instance = this;
        }

        public float CalculateEnemyDamageMultiplier()
        {
            switch (selectedDifficulty)
            {
                case DifficultyOption.Easy:
                    return easyDamageMultiplier;
                case DifficultyOption.Normal:
                    return normalDamageMultiplier;
                case DifficultyOption.Hard:
                    return hardDamageMultiplier;
                default:
                    return 0f;
            }
        }

        //Difficulty
        public int GetAdditionalRooms() { return additionalRoomsPerClear; }
        public int GetEnemyHealthIncrease() { return enemyHealthIncrease; }
        public float GetEnemyDamageMultIncrease() { return enemyDamageMultiplierIncrease; }

        //Score
        public int GetScorePerEnemy() { return scorePerEnemy; }
        public int GetScorePerBoss() { return scorePerBoss; }
        public int GetScorePerFloorClear() { return scorePerFloorClear; }

        //Other
        public void ChangeScoreNeededToTeleportBy(int n) { scoreNeededToTeleport += n; }
        public int GetScoreNeededToTeleport() { return scoreNeededToTeleport; }


    }
}
