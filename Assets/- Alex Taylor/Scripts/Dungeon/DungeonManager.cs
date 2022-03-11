using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;
using UnityEngine.AI;
using ATDungeon.Dungeon.Setup;
using ATDungeon.Pickups;
using System;

namespace ATDungeon.Dungeon
{
    public class DungeonManager : MonoBehaviour
    {
        public enum RoomType { SmallChamber, MediumChamber, LargeChamber, Hallway, Corner, TrapRoom, MedRoom, ShopRoom, ForgeRoom, Stairway, SpawnRoom, LoopingPath };

        [System.Serializable]
        public class ConnectionPiece
        {
            public RoomType roomType;
            public GameObject[] prefabs;
            [Range(0.0f, 1.0f)] public float frequencyPercentage;
            public int maxQuantity;
            public int curQuantity;
            public DungeonManager.RoomType[] validConnectionTypes;
        }

        [System.Serializable]
        public class ChestObject
        {
            public GameObject prefab;
            [Range(0.0f, 1.0f)] public float frequencyPercentage;
            public Transform spawnPoint;
        }

        [System.Serializable]
        public class CrateObject
        {
            public GameObject prefab;
            [Range(0.0f, 1.0f)] public float frequencyPercentage;
            public Transform spawnPoint;
        }

        public static DungeonManager instance;

        [Header("Dungeon Settings")]
        [SerializeField]
        private Transform roomsParent;
        [SerializeField]
        private int scoreNeededToTeleportOffset;
        [SerializeField]
        private NavMeshSurface navMeshComponent;
        [SerializeField]
        private int maxRooms;
        [SerializeField]
        private int maxRoomSpawnAttempts;
        [SerializeField]
        private float roomSetupDelay;
        [SerializeField]
        private float delayBetweenAttempts;
        [SerializeField]
        private float roomValidationDelay;
        [SerializeField]
        private float delayTillCompletionIsTrue;
        [SerializeField]
        private float frequencyPercentageMultiplier;
        [SerializeField]
        private int maxQuantityMultiplier;

        [Header("Locked Room Settings")]
        [SerializeField]
        private List<RoomType> lockedRoomBlackList;
        [SerializeField]
        [Range(0.0f, 1.0f)] private float chanceToLockRoom;
        [SerializeField]
        private GameObject lockedRoomKeyPrefab;

        [Header("Loop Settings")]
        [SerializeField]
        private LoopSpawner loopSpawner;
        [SerializeField]
        private float loopWaitTime;
        [SerializeField]
        [Range(1.0f, 100.0f)] private int maxLoopDistance;

        [Header("Dungeon Rooms")]
        [SerializeField]
        private ConnectionPiece[] connectionPieces;
        [SerializeField]
        private GameObject closerObj;
        [SerializeField]
        private GameObject teleportObj;
        [SerializeField]
        private GameObject lockedRoomObj;
        [SerializeField]
        private GameObject validatorObj;
        [SerializeField]
        private Transform playerSpawnTransform;
        [SerializeField]
        private Vector3 roomTriggerScale;

        [Header("Spawn Settings")]
        [SerializeField]
        private List<RoomType> bossRoomWhiteList;
        [SerializeField]
        private GameObject enableWhenComplete;
        [SerializeField]
        private Transform itemSpawnParent;
        [SerializeField]
        private ChestObject[] chestObjects;
        [SerializeField]
        private CrateObject[] crateObjects;

        [Header("Debug Util")]
        [SerializeField]
        private bool isDebugMode;
        [SerializeField]
        private bool shouldCheckValidators;
        [SerializeField]
        private bool shouldCleanupSpawnerUtils;
        [SerializeField]
        private float connectionPointOverlapRadius;

        private Dictionary<RoomType, ConnectionPiece> piecesDictionary;


        private GameObject lastSpawnedRoomObj = null; //Used To Mitigate Same Room Spawning In A Row
        private int totalMaxQuantity;
        private int currentRoomCount = 0;
        private int keysNeeded = 0;
        private List<RoomSpawner> spawnedRooms = new List<RoomSpawner>();
        private bool hasFinalised = false;
        private float timer;
        private float timeTakenToFinish;
        private bool hasMaxRoomsSpawned = false;
        private float minLoopDistance;
        const int maxLockedRooms = 4; //If this is changed, UI for keys needs changing
        private List<int> lockedRoomNumbers = new List<int>();

        private void Awake()
        {
            instance = this;
            enableWhenComplete.SetActive(false);
            SetupDictonaryEtc();
        }

        // Start is called before the first frame update
        void Start()
        {
            minLoopDistance = loopSpawner.GetWidth() / 2;
            timer = delayTillCompletionIsTrue;

            if(Progression.ProgressionManager.Instance && Persistence.PersistenceManager.Instance)
                if(Persistence.PersistenceManager.Instance.GetFloorsCleared() != 0)
                    maxRooms += Progression.ProgressionManager.Instance.GetAdditionalRooms();

            if (maxRooms > totalMaxQuantity)
            {
                maxRooms = totalMaxQuantity;
                Debug.LogWarning("Max Rooms Greater Than Max Quantity Of Rooms, New Max Rooms = " + maxRooms);
            }
            //RandomPieceDebug();
        }

        private void Update()
        {
            if (!hasFinalised)
            {
                float t = Time.deltaTime;
                timeTakenToFinish += t;
                timer -= t;
                if (timer <= 0.0f)
                {
                    hasFinalised = true;
                    FinaliseDungeon();
                }
            }
        }

        private void SetupDictonaryEtc()
        {
            //Add all pieces from array to dictionary for easier access via roomtype
            //Also calculate the quantity of all the max quantities of each room added toggether
            int totalPrefabNum = 0;
            piecesDictionary = new Dictionary<RoomType, ConnectionPiece>();
            foreach (ConnectionPiece cp in connectionPieces)
            {
                totalPrefabNum += cp.prefabs.Length;
                cp.maxQuantity *= maxQuantityMultiplier;
                piecesDictionary.Add(cp.roomType, cp);
                totalMaxQuantity += cp.maxQuantity;
                totalMaxQuantity -= cp.curQuantity;
            }
            Debug.Log("Total Max Quantity = " + totalMaxQuantity);
            Debug.Log("Total Prefab Num = " + totalPrefabNum);
        }

        private void FinaliseDungeon()
        {
            Debug.Log("Dungeon Finished Spawning...");
            if (currentRoomCount > 0)
            {
                int wallsSpawned = 0;
                int loopsFound = 0;

                Debug.Log("Time Taken: " + timeTakenToFinish);
                Debug.Log("Rooms Spawned: " + currentRoomCount);
                Debug.Log("Last Piece To Spawn: " + spawnedRooms[spawnedRooms.Count - 1]);

                RoomSpawner finalRoom = GetFinalRoom();
                RoomSpawner bossRoom = GetBossRoom();
                Debug.Log("Last Room To Spawn: " + finalRoom.gameObject.name);

                List<GameObject> trashCan = new List<GameObject>();

                //Builds Navmesh For Generated Dungeon, But Before Walls Are Placed
                navMeshComponent.BuildNavMesh();

                bool bExceptionThrown = false;
                bool isBossRoom = false;
                ConnectionPoint teleportPoint = SpawnTeleport(finalRoom); //Spawn Teleport To Next Stage In Final Room
                //Room Spawner Loop Start
                for (int i = 0; i < spawnedRooms.Count; i++)
                {
                    try
                    {
                        isBossRoom = false;
                        spawnedRooms[i].gameObject.transform.parent = roomsParent.transform;
                        DungeonRoom dr = spawnedRooms[i].gameObject.AddComponent<DungeonRoom>();
                        if (spawnedRooms[i] == bossRoom)
                            isBossRoom = true;

                        if (shouldCleanupSpawnerUtils && spawnedRooms[i].Validators != null)
                            foreach (Validator v in spawnedRooms[i].Validators)
                                trashCan.Add(v.gameObject);

                        ConnectionPoint[] connectionPoints = spawnedRooms[i].ConnectionPoints;
                        List<GameObject> lockedRoomObjects = new List<GameObject>();
                        if (connectionPoints != null)
                        {
                            bool bLockRoom = RandomLockRoom(spawnedRooms[i], dr); //Randomly Locks Room And Returns Whether Room Was Locked
                            if (bLockRoom)
                            {
                                lockedRoomNumbers.Add(i);
                            }

                            //Connection Point Loop Start
                            for (int j = 0; j < connectionPoints.Length; j++)
                            {

                                //Generate Closer Walls
                                if (!connectionPoints[j].IsConnected && connectionPoints[j].GetBlockType() == ConnectionPoint.BlockType.Unblocked)
                                {
                                    GameObject wall = connectionPoints[j].SpawnWall(closerObj);
                                    connectionPoints[j].SetBlockType(ConnectionPoint.BlockType.Wall);
                                    wallsSpawned++;
                                }
                                //Generate Closer Walls End

                                //Generate Locked Room Walls
                                else if (bLockRoom && connectionPoints[j].GetBlockType() == ConnectionPoint.BlockType.Unblocked)
                                {
                                    GameObject lockedRoomWall = connectionPoints[j].SpawnWall(lockedRoomObj);
                                    LockedWall lockedWall = lockedRoomWall.GetComponentInChildren<LockedWall>();
                                    lockedWall.SetDungeonRoom(dr);
                                    lockedWall.SetLockedWallID(keysNeeded);
                                    lockedRoomObjects.Add(lockedRoomWall);
                                    connectionPoints[j].SetBlockType(ConnectionPoint.BlockType.Locked);
                                }
                                //Generating Lock Walls End

                                //Final Validation Check To Make Sure No Gaps Are Present
                                else if (connectionPoints[j] != teleportPoint)
                                {
                                    Collider[] overlaps = Physics.OverlapSphere(connectionPoints[j].gameObject.transform.position, connectionPointOverlapRadius);
                                    bool isValid = false;
                                    foreach (Collider overlap in overlaps)
                                        if (overlap.gameObject.tag == "ConnectionPoint" && overlap.gameObject != connectionPoints[j].gameObject)
                                            isValid = true;
                                    if (!isValid)
                                    {
                                        Debug.LogWarning("Found Invalid Connection At [" + connectionPoints[j].gameObject.name + " / " + connectionPoints[j].GetRoomSpawner().gameObject.name + "] Spawning Extra Wall!");
                                        if (isDebugMode)
                                            UtilFunctions.instance.DebugDrawMessage("Found Invalid Connection At[" + connectionPoints[j].gameObject.name + " / " + connectionPoints[j].GetRoomSpawner().gameObject.name + "]", connectionPoints[j].gameObject.transform.position);
                                        GameObject wall = connectionPoints[j].SpawnWall(closerObj);
                                        wallsSpawned++;
                                    }
                                }

                                if (shouldCleanupSpawnerUtils)
                                    trashCan.Add(connectionPoints[j].gameObject);
                            }
                            //Connection Point Loop End
                            
                        } // Connection Point Null Check End
                        
                        SetupDungeonRoom(dr, spawnedRooms[i], lockedRoomObjects.ToArray(), isBossRoom);
                        
                    }
                    
                    catch(Exception e)
                    {
                        Debug.LogError("Exception Thrown Setting Up: [" + spawnedRooms[i].gameObject.name + "] - " + e);
                        bExceptionThrown = true;
                    }
                }
                //RoomSpawner Loop End

                if(bExceptionThrown)
                {
                    Debug.Break();
                }

                int maxEnemyScore = EnemyManager.Instance.GetSpawnedEnemies() * Progression.ProgressionManager.Instance.GetScorePerEnemy();
                int maxBossScore = EnemyManager.Instance.GetSpawnedBosses() * Progression.ProgressionManager.Instance.GetScorePerBoss();
                int maxScore = maxEnemyScore + maxBossScore;
                Debug.Log("Spawned Enemies: [" + EnemyManager.Instance.GetSpawnedEnemies() + "]");
                Debug.Log("Spawned Bosses: [" + EnemyManager.Instance.GetSpawnedBosses() + "]");
                Debug.Log("Max Enemy Score: [" + maxEnemyScore + "] - Max Boss Score: [" + maxBossScore + "]");
                Debug.Log("Needed Score: [" + (maxScore - scoreNeededToTeleportOffset).ToString() + "]");
                Progression.ProgressionManager.Instance.ChangeScoreNeededToTeleportBy(maxScore - scoreNeededToTeleportOffset);

                SpawnKeys(); //Generates Keys For Locked Rooms

                loopsFound = loopSpawner.GetHallwaysSpawned();
                Debug.Log("Walls Spawned: " + wallsSpawned);
                Debug.Log("Loops Found: " + loopsFound);

                foreach (GameObject g in trashCan)
                    Destroy(g);

                if (enableWhenComplete)
                    enableWhenComplete.SetActive(true);
            }
        }

        private bool RandomLockRoom(RoomSpawner rs, DungeonRoom dr)
        {
            bool bLockRoom = false;
            float[] lockRoomWeight = new float[] { chanceToLockRoom, 1.0f - chanceToLockRoom };
            int shouldLockRoom = UtilFunctions.instance.GetWeightedRandomValue(lockRoomWeight, 0);
            if (shouldLockRoom == 0 && keysNeeded < maxLockedRooms)
            {
                if (!lockedRoomBlackList.Contains(rs.RoomType))
                {
                    Debug.Log("Locking Room: " + rs.gameObject.name);
                    keysNeeded++;
                    bLockRoom = true;
                    dr.LockRoom();
                }
            }

            return bLockRoom;
        }

        private ConnectionPoint SpawnTeleport(RoomSpawner finalRoom)
        {
            ConnectionPoint[] connectionPoints = finalRoom.ConnectionPoints;
            bool bSpawnedWall = false;
            List<int> possibleValues = new List<int>();

            for (int n = 0; n < connectionPoints.Length; n++)
                possibleValues.Add(n);

            int index = 0;
            int randNum = 0;
            ConnectionPoint selectedPoint = null;

            do
            {
                index = UnityEngine.Random.Range(0, possibleValues.Count - 1);
                randNum = possibleValues[index];
                possibleValues.RemoveAt(index);
                selectedPoint = connectionPoints[randNum];
            } while (selectedPoint.IsConnected && possibleValues.Count > 0);

            if (!selectedPoint.IsConnected)
            {
                selectedPoint.SpawnWall(teleportObj);
                selectedPoint.SetBlockType(ConnectionPoint.BlockType.Teleport);
                selectedPoint.IsConnected = true;
                bSpawnedWall = true;
                Debug.Log("Spawned Teleport In: " + finalRoom.gameObject.name + " At: " + selectedPoint.gameObject.name);
            }

            else if (!bSpawnedWall)
                Debug.LogError("Failed To Spawn Teleport");

            return selectedPoint;
        }

        private void SpawnKeys()
        {
            GameObject keysParent = new GameObject("Keys Parent");
            keysParent.transform.parent = this.transform;
            Debug.Log("Keys Needed: " + keysNeeded);
            Debug.Log("Locked Rooms: " + lockedRoomNumbers.Count);
            for (int i = 0; i < keysNeeded; i++)
            {
                Debug.Log("------Spawning Key: [" + (i + 1) + " / " + keysNeeded + "]------");
                GameObject key = Instantiate(lockedRoomKeyPrefab, keysParent.transform);

                RoomSpawner chosenRoom = null;
                int randomRoomNum = 0;
                int roomChooseAttempts = 0;
                do
                {
                    randomRoomNum = UnityEngine.Random.Range(0, lockedRoomNumbers[i]);
                    Debug.Log("Attempting To Get Room At Number: " + randomRoomNum);
                    chosenRoom = spawnedRooms[randomRoomNum];
                    roomChooseAttempts++;
                } while (chosenRoom.RoomType == RoomType.LoopingPath && roomChooseAttempts < lockedRoomNumbers[i]);

                if (chosenRoom == null)
                {
                    Debug.LogError("Chosen Room Was NULL, Setting It To Default Value");
                    chosenRoom = spawnedRooms[lockedRoomNumbers[i] - 1];
                }

                Debug.Log("Positioning Key In Room [" + randomRoomNum + "]: " + chosenRoom.gameObject.name);
                Validator randValidator = chosenRoom.Validators[UnityEngine.Random.Range(0, chosenRoom.Validators.Length)];
                Bounds bounds = randValidator.ValidatorCollider.bounds;
                Vector3 center = bounds.center;
                float radius = bounds.extents.magnitude;
                Vector3 keyPosition = UtilFunctions.instance.RandomNavmeshLocation(radius / 2, center);
                Debug.Log("Setting Key Position To: " + keyPosition);
                key.transform.localPosition = new Vector3(keyPosition.x, keyPosition.y + .5f, keyPosition.z);

                LockedRoomKey lrk = key.GetComponent<LockedRoomKey>();
                lrk.SetKeyID(i + 1);
                Debug.Log("------------------------");
            }
        }

        private float[] GetProbability(int arrayLength, float[] frequencyPercentages)
        {
            float[] weights = new float[arrayLength + 1];
            float totalWeight = 0.0f;
            for (int i = 0; i < arrayLength; i++)
            {
                float weight = frequencyPercentages[i];
                weights[i] = weight;
                totalWeight += weight;
            }
            weights[weights.Length - 1] = 1.0f - totalWeight;
            return weights;
        }

        private void SpawnChestsEtc(RoomSpawner rs, DungeonRoom dr)
        {
            //Chest Probability
            float[] chestFreqPercents = new float[chestObjects.Length];
            for (int i = 0; i < chestObjects.Length; i++)
                chestFreqPercents[i] = chestObjects[i].frequencyPercentage;
            float[] chestWeights = GetProbability(chestObjects.Length, chestFreqPercents);
            //-----------------

            //Crate Probability
            float[] crateFreqPercents = new float[crateObjects.Length];
            for (int i = 0; i < crateObjects.Length; i++)
                crateFreqPercents[i] = crateObjects[i].frequencyPercentage;
            float[] crateWeights = GetProbability(crateObjects.Length, crateFreqPercents);
            //-----------------

            ChestSpawn[] chestSpawns = rs.GetChestSpawns();

            //Chest Spawning Start
            if (chestSpawns.Length > 0 && chestSpawns != null)
            {
                foreach (ChestSpawn chestSpawn in chestSpawns)
                {
                    int arrayOffset = 0;
                    if (chestSpawn.GetShouldAlwaysSpawn()) //Offset By -1 To Remove Last Entry Which Is No Chest Spawning
                        arrayOffset = -1;

                    if (chestSpawn.GetOnlyBlockedSpawn())
                    {
                        GameObject nearestCPObj = UtilFunctions.instance.FindClosestTag(chestSpawn.gameObject.transform.position, "ConnectionPoint", 2.5f);
                        ConnectionPoint nearestCP = null;
                        if (nearestCPObj != null)
                        {
                            nearestCP = nearestCPObj.GetComponent<ConnectionPoint>();
                            if (nearestCP != null)
                            {
                                if (nearestCP.GetBlockType() != ConnectionPoint.BlockType.Wall)
                                {
                                    chestSpawn.CanSpawnChest = false;
                                    chestSpawn.CanSpawnCrate = false;
                                }
                            }
                        }
                    }

                    if (chestSpawn.CanSpawnChest)
                    {
                        int randChest = UtilFunctions.instance.GetWeightedRandomValue(chestWeights, arrayOffset);
                        if (randChest != (chestWeights.Length - 1) || chestSpawn.GetShouldAlwaysSpawn())
                        {
                            ChestObject chestObj = new ChestObject();
                            chestObj = chestObjects[randChest];
                            chestObj.spawnPoint = chestSpawn.gameObject.transform;
                            dr.SpawnChest(chestObj);
                            chestSpawn.CanSpawnChest = false;
                            chestSpawn.CanSpawnCrate = false;
                        }
                    }

                    if (chestSpawn.CanSpawnCrate)
                    {
                        int randCrate = UtilFunctions.instance.GetWeightedRandomValue(crateWeights, arrayOffset);
                        if (randCrate != (crateWeights.Length - 1) || chestSpawn.GetShouldAlwaysSpawn())
                        {
                            CrateObject crateObj = crateObjects[randCrate];
                            crateObj.spawnPoint = chestSpawn.gameObject.transform;
                            dr.SpawnCrate(crateObj);
                            chestSpawn.CanSpawnChest = false;
                            chestSpawn.CanSpawnCrate = false;
                        }
                    }
                }
            }
            //Chest Spawning End
        }

        private RoomSpawner GetBossRoom()
        {
            RoomSpawner bossRoom = null;
            int i = 1;
            do
            {
                bossRoom = spawnedRooms[spawnedRooms.Count - i];
                i++;
                if (i > spawnedRooms.Count)
                {
                    bossRoom = null;
                    break;
                }
            } while (!bossRoomWhiteList.Contains(bossRoom.RoomType));
            return bossRoom;
        }

        private RoomSpawner GetFinalRoom()
        {
            //Makes Sure Final Room Isn't A Looping Pathway, And Has A Spare Connection Point
            RoomSpawner finalRoom = null;
            int i = 1;
            do
            {
                finalRoom = spawnedRooms[spawnedRooms.Count - i];
                i++;
                if (i > spawnedRooms.Count)
                {
                    finalRoom = null;
                    break;
                }
            } while (finalRoom.RoomType == RoomType.LoopingPath || !finalRoom.HasAvailableConnectionPoint());
            return finalRoom;
        }

        private void SetupDungeonRoom(DungeonRoom dr, RoomSpawner rs, GameObject[] lockedRoomObjects, bool isBossRoom)
        {
            if (rs.RoomType != RoomType.LoopingPath)
            {
                if (isBossRoom)
                    dr.SetIsBossRoom(true);

                Validator randomValidator = rs.Validators[UnityEngine.Random.Range(0, rs.Validators.Length)];
                Bounds bounds = randomValidator.ValidatorCollider.bounds;
                Vector3 center = bounds.center;
                float radius = bounds.extents.magnitude;
                int spNum = EnemyManager.Instance.GetSP(rs.RoomType);
                int timesToSpawn = EnemyManager.Instance.GetEnemiesPerSP(rs.RoomType);
                SpawnChestsEtc(rs, dr);
                dr.SetRoomType(rs.RoomType);
                dr.SetNumberOfSP(spNum);
                dr.SetTimesToSpawn(timesToSpawn);
                dr.SetSpawnPoints(radius, center);
                dr.SetLockedRoomObjects(lockedRoomObjects);
                dr.SpawnEnemies();

                if (rs.ConnectionPoints != null)
                    dr.GenerateRoomTriggers(rs.ConnectionPoints);
            }
            Destroy(rs);
        }

        //Used For Testing Randomness Of Piece Picker
        private void RandomPieceDebug()
        {
            //Needs to get a random piece from the list of valid pieces
            //If a piece is returned, add to the current quantity, then check if equal to max quantity, if so, remove that piece from valid pieces

            int timesToRun = 40;

            if (timesToRun > totalMaxQuantity)
            {
                Debug.LogWarning("Times To Run Is Greater Than Max Quantites");
                timesToRun = totalMaxQuantity;
            }

            Dictionary<RoomType, ConnectionPiece> validPieces = piecesDictionary;
            validPieces.Remove(RoomType.SpawnRoom);
            for (int i = 0; i < timesToRun; i++)
            {
                List<ConnectionPiece> piecesList = new List<ConnectionPiece>(validPieces.Values);

                float[] pieceWeights = new float[piecesList.Count];

                for (int j = 0; j < piecesList.Count; j++)
                {
                    pieceWeights[j] = piecesList[j].frequencyPercentage * frequencyPercentageMultiplier;
                }

                int randomPieceNum = UtilFunctions.instance.GetWeightedRandomValue(pieceWeights, 0);
                if (randomPieceNum != -1)
                {
                    ConnectionPiece randomPiece = piecesList[randomPieceNum];
                    randomPiece.curQuantity++;
                    if (randomPiece.curQuantity == randomPiece.maxQuantity)
                    {
                        validPieces.Remove(randomPiece.roomType);
                    }
                    Debug.Log("Random Piece Was: " + randomPiece.roomType.ToString());
                }
                else
                    Debug.LogError("Random Index Was: " + randomPieceNum);
            }
        }

        public GameObject GetRandomPiece(RoomType[] validTypes, List<GameObject> attemptedPieces, ConnectionPoint caller)
        {
            Dictionary<RoomType, ConnectionPiece> validPieces = new Dictionary<RoomType, ConnectionPiece>();
            foreach (RoomType rt in validTypes)
            {
                ConnectionPiece cp = piecesDictionary[rt];
                if (cp.curQuantity != cp.maxQuantity)
                    validPieces.Add(rt, cp);
            }

            if (validPieces.Values.Count <= 0)
            {
                Debug.LogWarning("Zero Valid Pieces Found");
                if (DungeonManager.instance.GetIsDebugMode())
                    UtilFunctions.instance.DebugDrawMessage("Zero Valid Pieces" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "] "), caller.transform.position);
                return null;
            }

            List<ConnectionPiece> piecesList = new List<ConnectionPiece>(validPieces.Values);

            float[] pieceWeights = new float[piecesList.Count];

            for (int j = 0; j < piecesList.Count; j++)
                pieceWeights[j] = piecesList[j].frequencyPercentage * frequencyPercentageMultiplier;

            int randomPieceNum = UtilFunctions.instance.GetWeightedRandomValue(pieceWeights, 0);

            if (randomPieceNum != -1)
            {
                ConnectionPiece randomPiece = piecesList[randomPieceNum];
                GameObject roomObj = null;

                if (randomPiece.prefabs.Length <= 0)
                    Debug.LogWarning("Piece [" + randomPiece.roomType.ToString() + "] Has No Prefabs!");

                List<GameObject> validPrefabs = new List<GameObject>();

                foreach (GameObject obj in randomPiece.prefabs)
                    if (!attemptedPieces.Contains(obj))
                        validPrefabs.Add(obj);

                int randAttempts = 0;
                int randNum = 0;

                /*
                do
                {
                    randNum = UnityEngine.Random.Range(0, randomPiece.prefabs.Length);
                    Debug.Log("Randum Num Was: " + randNum);
                    roomObj = randomPiece.prefabs[randNum];
                    randAttempts++;
                } while (roomObj == lastSpawnedRoomObj && randAttempts < maxRoomSpawnAttempts);

                if (roomObj == null)
                {
                    Debug.LogWarning("Room Obj For [" + randomPiece.roomType.ToString() + "] Was NULL When Attempting To Get Random Piece, Prefab Num Was: " + randNum);
                    if (DungeonManager.instance.GetIsDebugMode())
                        UtilFunctions.instance.DebugDrawMessage("Room OBJ Was NULL" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "] "), caller.transform.position);
                }
                */

                //Tries to not spawn same object as last spawned object

                int validPrefabCount = validPrefabs.Count;

                if (validPrefabCount != 0)
                {
                    //GameObject[] validPrefabArray = validPrefabs.ToArray();
                    do
                    {
                        randNum = UnityEngine.Random.Range(0, validPrefabCount);
                        roomObj = validPrefabs[randNum];
                        randAttempts++;
                    } while (roomObj == lastSpawnedRoomObj && randAttempts < maxRoomSpawnAttempts);

                    if (roomObj == null)
                    {
                        Debug.LogWarning("Room Obj For [" + randomPiece.roomType.ToString() + "] Was NULL When Attempting To Get Random Piece, Prefab Num Was: " + randNum);
                        if (DungeonManager.instance.GetIsDebugMode())
                            UtilFunctions.instance.DebugDrawMessage("Room OBJ Was NULL" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "] "), caller.transform.position);
                    }
                }

                else
                {
                    Debug.LogWarning("Already Attempted All Prefabs For This RoomType");

                    if (DungeonManager.instance.GetIsDebugMode())
                        UtilFunctions.instance.DebugDrawMessage("Attempted All Prefabs" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "] "), caller.transform.position);

                    return null;
                }

                if (randomPiece.roomType != RoomType.Corner && randomPiece.roomType != RoomType.Hallway && randomPiece.roomType != RoomType.Stairway)
                    lastSpawnedRoomObj = roomObj;

                return roomObj;
            }
            else
            {
                Debug.LogWarning("Random Piece Num Was -1");
                return null;
            }
        }

        public void UpdateCurrentRoomNum()
        {
            if (currentRoomCount >= maxRooms)
                hasMaxRoomsSpawned = true;
            currentRoomCount++;
        }

        public void AddRoomToList(RoomSpawner room) { spawnedRooms.Add(room); }
        public GameObject GetValidatorObj() { return validatorObj; }
        public Transform GetPlayerSpawnTransform() { return playerSpawnTransform; }
        public Transform GetRoomsParentTransform() { return roomsParent; }
        public Transform GetItemSpawnParent() { return itemSpawnParent; }
        public ConnectionPiece GetConnectionPieceFromDictionary(RoomType roomType) { return piecesDictionary[roomType]; }
        public RoomType[] GetValidRoomTypes(RoomType roomType) { return piecesDictionary[roomType].validConnectionTypes; }

        public int GetMaxSpawnAttemptsNum() { return maxRoomSpawnAttempts; }
        public int GetMaxRooms() { return maxRooms; }
        public int GetCurrentRoomCount() { return currentRoomCount; }
        public bool GetHasMaxRoomsSpawned() { return hasMaxRoomsSpawned; }

        public float GetRoomSetupDelay() { return roomSetupDelay; }
        public float GetDelayBetweenAttempts() { return delayBetweenAttempts; }
        public float GetRoomValidationDelay() { return roomValidationDelay; }
        public void ResetTimer() { timer = delayTillCompletionIsTrue; }

        public LoopSpawner GetLoopSpawner() { return loopSpawner; }
        public float GetLoopWaitTime() { return loopWaitTime; }
        public float GetLoopMaxDistance() { return maxLoopDistance; }
        public float GetLoopMinDistance() { return minLoopDistance; }

        public Vector3 GetRooomTriggerScale() { return roomTriggerScale; }

        public bool GetIsDebugMode() { return isDebugMode; }
        public bool GetShouldCheckValidators() { return shouldCheckValidators; }
    }
}
