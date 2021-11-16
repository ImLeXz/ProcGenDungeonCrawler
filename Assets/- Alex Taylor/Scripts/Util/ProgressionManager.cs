using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Progression
{
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance;

        [Header("Difficulty Modifiers")]
        [SerializeField]
        private int additionalRoomsPerClear = 10;
        [SerializeField]
        private int enemyHealthIncrease = 10;
        [SerializeField]
        private float enemyDamageMultiplierIncrease = 0.15f;

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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
