using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;
using ATDungeon.Dungeon.Setup;

namespace ATDungeon.Dungeon
{
    public class DungeonRoom : MonoBehaviour
    {
        [SerializeField]
        private int numberOfEnemySpawnPoints;
        [SerializeField]
        private DungeonManager.RoomType roomType;

        private List<Vector3> enemySpawnPoints = new List<Vector3>();
        private GameObject[] lockedRoomObjects;
        private GameObject roomTriggerParent = null;
        private GameObject spawnPointParent = null;
        private GameObject enemyParent = null;
        private GameObject bossParent = null;
        private int timesToSpawn = 0;
        bool isBossRoom = false;

        private void Awake()
        {
            roomTriggerParent = new GameObject("RoomTrigger Parent");
            roomTriggerParent.transform.parent = this.gameObject.transform;

            spawnPointParent = new GameObject("SpawnPoint Parent");
            spawnPointParent.transform.parent = this.gameObject.transform;
        }

        public void SpawnEnemies()
        {
            if (!isBossRoom)
            {
                enemyParent = new GameObject("Enemies");
                enemyParent.transform.parent = this.gameObject.transform;
                int spawnedEnemies = 0;
                foreach (Vector3 sp in enemySpawnPoints)
                {
                    for (int i = 0; i < timesToSpawn; i++)
                    {
                        GameObject randEnemy = Instantiate(EnemyManager.Instance.GetRandomEnemy(), enemyParent.transform);
                        randEnemy.transform.position = sp;
                        randEnemy.SetActive(false);
                        spawnedEnemies++;
                    }
                }
                EnemyManager.Instance.ChangeSpawnedEnemiesBy(spawnedEnemies);
            }
            else
            {
                bossParent = new GameObject("BossParent");
                bossParent.transform.parent = this.gameObject.transform;
                Vector3 sp = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
                GameObject randBoss = Instantiate(EnemyManager.Instance.GetRandomBoss(), bossParent.transform);
                randBoss.transform.position = sp;
                randBoss.SetActive(false);
                EnemyManager.Instance.ChangeSpawnedBossesBy(1);
            }
        }

        public void ActivateEnemies()
        {
            if (!isBossRoom)
            {
                if (timesToSpawn > 0)
                {
                    Destroy(roomTriggerParent);
                    int initialEnemyCount = enemyParent.transform.childCount;
                    int amountToEnable = initialEnemyCount / timesToSpawn;
                    float delay = EnemyManager.Instance.GetEnemySpawnDelay(roomType);
                    StartCoroutine(ActivateEnemiesRoutine(0, amountToEnable, initialEnemyCount, 0.0f));
                    for (int i = 1; i < timesToSpawn; i++)
                    {
                        StartCoroutine(ActivateEnemiesRoutine(amountToEnable * i, amountToEnable * (i + 1), initialEnemyCount, delay * i));
                    }
                }
            }
            else
            {
                Destroy(roomTriggerParent);
                bossParent.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        private IEnumerator ActivateEnemiesRoutine(int start, int amountToEnable, int initialEnemyCount, float delay)
        {
            yield return new WaitForSeconds(delay);
            int amountDifference = initialEnemyCount - enemyParent.transform.childCount;
            start -= amountDifference;
            amountToEnable -= amountDifference;

            for (int i = start; i < amountToEnable; i++)
                enemyParent.transform.GetChild(i).gameObject.SetActive(true);

        }

        public void SetSpawnPoints(float radius, Vector3 center)
        {
            List<Vector3> spawnPoints = new List<Vector3>();
            for (int i = 0; i < numberOfEnemySpawnPoints; i++)
            {
                Vector3 sp = Vector3.zero;
                bool badSP = false;
                int spawnAttempts = 0;
                do
                {
                    badSP = false;
                    sp = UtilFunctions.instance.RandomNavmeshLocation(radius, center);
                    foreach(Vector3 point in spawnPoints)
                    {
                        if(Vector3.Distance(point, sp) < EnemyManager.Instance.GetEnemySPThreshold())
                            badSP = true;
                    }

                    if(!badSP)
                        spawnPoints.Add(sp);

                    spawnAttempts++;

                    if (spawnAttempts >= EnemyManager.Instance.GetMaxSpawnAttempts())
                    {
                        Debug.LogWarning("Max SpawnPoint Placement Attempts Exceeded");
                        break;
                    }

                } while (badSP);
                GameObject spObj = new GameObject();
                spObj.name = "SpawnPoint_" + i;
                spObj.transform.position = sp;
                spObj.transform.parent = spawnPointParent.transform;
                enemySpawnPoints.Add(spObj.transform.position);
            }
        }


        public void SpawnChest(DungeonManager.ChestObject chest)
        {
            Instantiate(chest.prefab, chest.spawnPoint);
        }

        public void SpawnCrate(DungeonManager.CrateObject crate)
        {
            Instantiate(crate.prefab, crate.spawnPoint);
        }

        public void GenerateRoomTriggers(ConnectionPoint[] connectionPoints)
        {
            if (connectionPoints.Length > 0)
            {
                int triggerNum = 0;
                foreach (ConnectionPoint cPoint in connectionPoints)
                {
                    GameObject roomTriggerObj = new GameObject("RoomTrigger_" + triggerNum);
                    roomTriggerObj.tag = "Trigger";
                    roomTriggerObj.layer = LayerMask.NameToLayer("Trigger");
                    roomTriggerObj.transform.parent = cPoint.gameObject.transform;
                    roomTriggerObj.transform.localScale = DungeonManager.instance.GetRooomTriggerScale();
                    roomTriggerObj.transform.localPosition = new Vector3(0.0f, roomTriggerObj.transform.localScale.y / 2, 0.0f);
                    roomTriggerObj.transform.localEulerAngles = Vector3.zero;
                    roomTriggerObj.transform.localScale = DungeonManager.instance.GetRooomTriggerScale();
                    BoxCollider roomTriggerCol = roomTriggerObj.AddComponent<BoxCollider>();
                    RoomTrigger roomTrigger = roomTriggerObj.AddComponent<RoomTrigger>();
                    roomTrigger.SetDungeonRoom(this);
                    roomTriggerCol.isTrigger = true;
                    roomTriggerObj.transform.parent = roomTriggerParent.transform;
                    triggerNum++;
                }
            }
        }

        public void UnlockRoom()
        {
            roomTriggerParent.SetActive(true);
            foreach(GameObject obj in lockedRoomObjects)
            {
                Destroy(obj);
            }
        }

        public void SetRoomType(DungeonManager.RoomType rt) { roomType = rt; }
        public void LockRoom() { roomTriggerParent.SetActive(false); }
        public void SetLockedRoomObjects(GameObject[] objs) { lockedRoomObjects = objs; }
        public void SetNumberOfSP(int n) { numberOfEnemySpawnPoints = n; }
        public void SetTimesToSpawn(int n) { timesToSpawn = n; }
        public void SetIsBossRoom(bool b) { isBossRoom = b; Debug.Log("Set Boss Room To: " + this.gameObject.name); }
    }
}
