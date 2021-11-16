using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Enemies;
using ATDungeon.Utility;

namespace ATDungeon.Dungeon
{
    public class EnemyManager : MonoBehaviour
    {
        public enum EnemyType { FlyingBasic, FlyingMagic, FlyingElite, Melee, Ranged, Elite, Brute, Sorcerer, EliteRanged }

        [System.Serializable]
        private class Enemy
        {
            public EnemyType enemyType;
            public GameObject[] prefabs;
            [Range(0.0f, 1.0f)] public float frequencyPercentage;
        }

        [System.Serializable]
        private class EnemySpawnHelper
        {
            public DungeonManager.RoomType roomType;
            public EnemyType[] enemyTypes;
            public int numOfEnemySP;
            public float enemySpawnDelay;
            [Range(0, 10)] public int enemiesPerSP;
        }

        [System.Serializable]
        private class Boss
        {
            public GameObject prefab;
            public Vector2 floorRange;
            [Range(0.0f, 1.0f)] public float frequencyPercentage;
        }

        [Header("Enemies")]
        [SerializeField]
        private Enemy[] enemies;

        [Header("Bosses")]
        [SerializeField]
        private Boss[] bosses;

        [Header("Enemy Room Settings")]
        [SerializeField]
        private EnemySpawnHelper[] enemySpawnHelpers;

        [Header("Spawn Point Settings")]
        [SerializeField]
        private float enemySPThreshold;
        [SerializeField]
        private int maxSpawnAttempts;

        private float[] enemyWeights;
        private Dictionary<EnemyType, Enemy> enemyDictionary;
        private Dictionary<DungeonManager.RoomType, EnemySpawnHelper> enemySpawnHelperDictionary;
        public static EnemyManager Instance;

        private int spawnedEnemies = 0;
        private int spawnedBosses = 0;
        

        private void Awake()
        {
            Instance = this;
            enemyDictionary = new Dictionary<EnemyType, Enemy>();
            enemySpawnHelperDictionary = new Dictionary<DungeonManager.RoomType, EnemySpawnHelper>();
        }

        // Start is called before the first frame update
        void Start()
        {
            InitialiseVariables();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void InitialiseVariables()
        {
            enemyWeights = new float[enemies.Length];
            for(int i = 0; i < enemies.Length; i++)
            {
                enemyWeights[i] = enemies[i].frequencyPercentage;
                enemyDictionary.Add(enemies[i].enemyType, enemies[i]);
            }

            for(int i = 0; i < enemySpawnHelpers.Length; i++)
            {
                enemySpawnHelperDictionary.Add(enemySpawnHelpers[i].roomType, enemySpawnHelpers[i]);
            }
        }

        public GameObject GetRandomEnemy()
        {
            int randomEnemyIndex = UtilFunctions.instance.GetWeightedRandomValue(enemyWeights, 0);
            Enemy randomEnemy = enemies[randomEnemyIndex];
            int randomPrefabIndex = Random.Range(0, randomEnemy.prefabs.Length);
            GameObject randomPrefab = randomEnemy.prefabs[randomPrefabIndex];
            if(randomPrefab)
                return randomPrefab;
            else
            {
                Debug.LogError("Random Enemy Was NULL");
                return null;
            }
        }

        public GameObject GetRandomBoss()
        {
            float[] bossWeights = new float[bosses.Length];
            for (int i = 0; i < bosses.Length; i++)
                bossWeights[i] = bosses[i].frequencyPercentage;

            int randomBossIndex = UtilFunctions.instance.GetWeightedRandomValue(bossWeights, 0);
            Boss randomBoss = bosses[randomBossIndex];

            return bosses[0].prefab; //only one boss currently
            /*
            if (randomBoss.prefab)
                return randomBoss.prefab;
            else
            {
                Debug.LogError("Random Boss Was NULL");
                return null;
            }
            */
        }

        public int GetEnemiesPerSP(DungeonManager.RoomType rt)
        {
            if (enemySpawnHelperDictionary.ContainsKey(rt))
                return enemySpawnHelperDictionary[rt].enemiesPerSP;
            else return 0;
        }

        public int GetSP(DungeonManager.RoomType rt)
        {
            if (enemySpawnHelperDictionary.ContainsKey(rt))
                return enemySpawnHelperDictionary[rt].numOfEnemySP;
            else return 0;
        }

        public float GetEnemySpawnDelay(DungeonManager.RoomType rt)
        {
            if (enemySpawnHelperDictionary.ContainsKey(rt))
                return enemySpawnHelperDictionary[rt].enemySpawnDelay;
            else return 0.0f;
        }

        public EnemyType[] GetAllowedEnemies(DungeonManager.RoomType rt)
        {
            if (enemySpawnHelperDictionary.ContainsKey(rt))
                return enemySpawnHelperDictionary[rt].enemyTypes;
            else return new EnemyType[0];
        }

        public float GetEnemySPThreshold() { return enemySPThreshold; }
        public int GetMaxSpawnAttempts() { return maxSpawnAttempts; }
        public int GetSpawnedEnemies() { return spawnedEnemies; }
        public int GetSpawnedBosses() { return spawnedBosses; }
        public void ChangeSpawnedEnemiesBy(int n) { spawnedEnemies += n; }
        public void ChangeSpawnedBossesBy(int n) { spawnedBosses += n; }
    }
}
