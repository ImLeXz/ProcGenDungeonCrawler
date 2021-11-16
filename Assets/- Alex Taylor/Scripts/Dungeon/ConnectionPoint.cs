using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using ATDungeon.Utility;

namespace ATDungeon.Dungeon.Setup
{
    public class ConnectionPoint : MonoBehaviour
    {
        public enum BlockType { Unblocked, Wall, Locked, Teleport };

        [SerializeField]
        private RoomSpawner roomSpawner;

        [SerializeField]
        private CPDetector cpDetector;

        [SerializeField]
        private bool isConnected;

        [SerializeField]
        private BlockType blockType;

        [Header("Debug Options")]
        [SerializeField]
        private bool autoInitialise;

        private int numOfAttemptedPieces = 0;
        private bool hasCompletedSpawning = false;
        private List<GameObject> attemptedPieces = new List<GameObject>();
        private int totalValidPrefabNum = 0;
        private bool initialised;
        private bool doOnce;

        private DungeonManager.RoomType[] validConnectionTypes;

        private void Awake()
        {
        }

        private void Start()
        {
            if (roomSpawner == null)
                roomSpawner = this.GetComponentInParent<RoomSpawner>();

            validConnectionTypes = DungeonManager.instance.GetValidRoomTypes(roomSpawner.RoomType);
            if (validConnectionTypes == null)
                validConnectionTypes = new DungeonManager.RoomType[0];

            if (autoInitialise)
                initialised = true;
        }

        // Update is called once per frame
        void Update()
        {
            DelayedSpawn();
        }

        private void DelayedSpawn()
        {
            if (initialised && !isConnected && !doOnce)
                if (cpDetector == null)
                    SpawnConnectedPiece();
        }

        public void InitialiseChecks()
        {
            initialised = true;
        }

        private void SpawnConnectedPiece()
        {
            //loops until valid piece is spawned
            //Spawns random piece using weighted randomiser from DungeonManager
            //Calls validate function on spawned prefabs RoomSpawner component which will run validation check on validators
            //If invalid, destroy RoomSpawner GameObject, then add 1 to attempts and loop again
            //If valid, set isConnected to true and break loop, delete connection point?
            //If Attempts greater than max attempts, spawn wall

            doOnce = true;

            if (!DungeonManager.instance.GetHasMaxRoomsSpawned() && !isConnected)
            {
                if (numOfAttemptedPieces >= DungeonManager.instance.GetMaxSpawnAttemptsNum())
                {
                    Debug.LogWarning("Max Spawn Attempts Exceeded");
                    if (DungeonManager.instance.GetIsDebugMode())
                        UtilFunctions.instance.DebugDrawMessage("Max Spawn Attempts Exceeded" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "] "), this.transform.position);
                    hasCompletedSpawning = true;
                }

                DungeonManager.instance.ResetTimer();
                GameObject roomPiece = DungeonManager.instance.GetRandomPiece(validConnectionTypes, attemptedPieces, this);

                if (roomPiece != null)
                {
                    attemptedPieces.Add(roomPiece);
                    GameObject spawnedRoom = Instantiate(roomPiece);

                    //Set Each Piece to random color
                    if (DungeonManager.instance.GetIsDebugMode())
                    {
                        Color randColor = new Color(
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(0f, 1f)
                        );
                        spawnedRoom.GetComponentInChildren<MeshRenderer>().material.color = randColor;
                    }
                    //---------------

                    PositionObject(spawnedRoom, true);
                }

                else if (validConnectionTypes.Length <= 0)
                {
                    Debug.LogWarning("Zero Valid Connection Types On Connection Point");
                    if (DungeonManager.instance.GetIsDebugMode())
                        UtilFunctions.instance.DebugDrawMessage("Zero Valid Connection Types On Connection Point" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "] "), this.transform.position);
                }

                numOfAttemptedPieces++;
            }

            else if (DungeonManager.instance.GetHasMaxRoomsSpawned())
            {
                Debug.LogWarning("Max Rooms Exceeded");
                if (DungeonManager.instance.GetIsDebugMode())
                    UtilFunctions.instance.DebugDrawMessage("Max Rooms Exceeded" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "] "), this.transform.position);
                hasCompletedSpawning = true;
            }
        }

        private void PositionObject(GameObject obj, bool shouldValidate)
        {
            RoomSpawner room_01 = this.roomSpawner;
            RoomSpawner room_02 = obj.GetComponent<RoomSpawner>();
            ConnectionPoint[] connectionPoints = new ConnectionPoint[room_02.ConnectionPoints.Length];

            for(int i = 0; i < room_02.ConnectionPoints.Length; i++)
                if (!room_02.ConnectionPoints[i].isConnected)
                    connectionPoints[i] = room_02.ConnectionPoints[i];

            ConnectionPoint room02Con;
            room02Con = connectionPoints[UnityEngine.Random.Range(0, connectionPoints.Length)];

            if (room02Con != null)
            {
                Vector3 xRot = this.transform.eulerAngles;
                Vector3 yRot = room02Con.transform.eulerAngles;

                int rotOffset = Mathf.RoundToInt(yRot.y - xRot.y);

                if (Mathf.Abs(rotOffset) == 0)
                    rotOffset = 180;
                else if (Mathf.Abs(rotOffset) == 180)
                    rotOffset = 0;

                room_02.gameObject.transform.eulerAngles = new Vector3(0.0f, room_02.gameObject.transform.eulerAngles.y + rotOffset, 0.0f);
                Vector3 offset = this.gameObject.transform.position - room02Con.gameObject.transform.position;
                room_02.gameObject.transform.position += offset;

                this.IsConnected = true;
                room02Con.IsConnected = true;

                if (shouldValidate)
                {
                    room_02.StartValidationChecks();
                    room_02.gameObject.name += (" [" + DungeonManager.instance.GetCurrentRoomCount() + "]");
                    StartCoroutine(ValidateRoom(room_02, room02Con, DungeonManager.instance.GetRoomValidationDelay()));
                }

            }

            else
            {
                Debug.LogWarning("Connection Point Was NULL");
                if (DungeonManager.instance.GetIsDebugMode())
                    UtilFunctions.instance.DebugDrawMessage("Connection Point Was Null" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "]"), this.transform.position);
            }
        }

        IEnumerator ValidateRoom(RoomSpawner room, ConnectionPoint cp, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            bool isValid = room.ValidateRoom();
            if (isValid)
            {
                cp.IsConnected = true;
                this.IsConnected = true;
                hasCompletedSpawning = true;
                DungeonManager.instance.GetConnectionPieceFromDictionary(room.RoomType).curQuantity++; //Adds To Quantity
                DungeonManager.instance.AddRoomToList(room); //Adds Room To List Of All Rooms Spawn
                DungeonManager.instance.UpdateCurrentRoomNum(); //Sets Current Room Counter Num Plus One
            }
            else
            {
                isConnected = false;
                if (!hasCompletedSpawning)
                    Invoke("SpawnConnectedPiece", DungeonManager.instance.GetDelayBetweenAttempts());
                Destroy(room.gameObject);
            }
        }

        //Checking for loops was causing issues and implemented fully procedural loop generation so this isnt used anymore
        public bool CheckForLoop()
        {
            try
            {
                bool canLoop = false;
                float radius = this.GetComponent<Collider>().bounds.size.x;
                Debug.Log("Loop Check Radius: " + radius);
                Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radius);
                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.tag == "ConnectionPoint" && hitCollider.gameObject != this.gameObject)
                    {
                        //float threshold = DungeonManager.instance.GetConnectionPointLockThreshold();
                        float threshold = 0.0f;
                        Vector3 hitPos = hitCollider.transform.position;
                        Vector3 thisPos = this.transform.position;
                        float dist = Vector3.Distance(hitPos, thisPos);

                        if ((dist - -threshold) * (threshold - dist) >= 0)
                        {
                           //Debug.Log("Distance Was Less Than Threshold, Locking Connection: [" + this.gameObject.name + " / " + roomSpawner.gameObject.name + "] - [" + hitCollider.gameObject.name + " / " + hitCollider.GetComponent<ConnectionPoint>().GetRoomSpawner().gameObject.name + "]");
                            isConnected = true;
                            canLoop = true;
                            if (DungeonManager.instance.GetIsDebugMode())
                                UtilFunctions.instance.DebugDrawMessage("Locking Loop" + (" [" + DungeonManager.instance.GetCurrentRoomCount() + "] "), this.gameObject.transform.position);
                        }
                    }
                }
                return canLoop;
            }
            catch(Exception e)
            {
                Debug.LogError("Error Whilst Checking For Loop: " + e);
                return false;
            }
        }

        public GameObject SpawnWall(GameObject wallPrefab)
        {
            Debug.Log("Spawning Wall At: [" + this.gameObject.name + " / " + this.roomSpawner.gameObject.name + "]");
            GameObject closerObj = Instantiate(wallPrefab);
            PositionObject(closerObj, false);
            closerObj.transform.parent = this.transform.parent;
            return closerObj;
        }

        public RoomSpawner GetRoomSpawner() { return roomSpawner; }
        public bool IsInitialised() { return initialised; }

        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; }
        }

        public void SetBlockType(BlockType bt) { blockType = bt; }
        public BlockType GetBlockType() { return blockType; }
    }
}
