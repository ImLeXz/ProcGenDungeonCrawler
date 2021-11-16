using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Dungeon.Setup
{
    public class RoomSpawner : MonoBehaviour
    {
        [SerializeField]
        private DungeonManager.RoomType roomType;

        [SerializeField]
        private ConnectionPoint[] connectionPoints;

        [SerializeField]
        private Validator[] validators;

        [SerializeField]
        private ChestSpawn[] chestSpawns;

        [SerializeField]
        private MeshRenderer mesh;

        private int completedConnections;

        private void Awake()
        {
            Initialise();
        }

        private void Start()
        {
            //Spawn Room Auto Setup
            if (roomType == DungeonManager.RoomType.SpawnRoom)
            {
                DungeonManager.instance.AddRoomToList(this);
                SetupRoom();
            }
        }

        private void Initialise()
        {
            //Variables Setup
            chestSpawns = GetComponentsInChildren<ChestSpawn>();
            validators = GetComponentsInChildren<Validator>();
            connectionPoints = GetComponentsInChildren<ConnectionPoint>();
            mesh = GetComponentInChildren<MeshRenderer>();
        }

        private void SetupRoom()
        {
            if (connectionPoints != null)
            {
                float perPieceDelay = DungeonManager.instance.GetRoomSetupDelay() / connectionPoints.Length;
                for (int i = 0; i < connectionPoints.Length; i++)
                    if (!connectionPoints[i].IsConnected && !DungeonManager.instance.GetHasMaxRoomsSpawned())
                        connectionPoints[i].Invoke("InitialiseChecks", perPieceDelay * i + 1);
            }

        }

        public void StartValidationChecks()
        {
            foreach (Validator v in validators)
                v.StartValidationChecks();
        }

        public bool ValidateRoom()
        {
            //Runs validation on each validator collider to check for overlaps
            //if overlapping, return false, else, setup room and return true.
            int overlaps = 0;
            foreach (Validator v in validators)
            {
                if (!v.GetValidationResult())
                    overlaps++;
            }
            if (overlaps > 0)
                return false;
            else
            {
                SetupRoom();
                return true;
            }
        }

        public bool HasAvailableConnectionPoint()
        {
            if (connectionPoints != null)
            {
                int availablePoints = 0;

                foreach (ConnectionPoint cp in connectionPoints)
                    if (!cp.IsConnected)
                        availablePoints++;

                if (availablePoints > 0)
                    return true;
                else return false;
            }
            else return false;
        }

        public ConnectionPoint[] ConnectionPoints
        {
            get { return connectionPoints; }
            set { connectionPoints = value; }
        }

        public Validator[] Validators
        {
            get { return validators; }
            set { validators = value; }
        }

        public DungeonManager.RoomType RoomType
        {
            get { return roomType; }
            set { roomType = value; }
        }

        public ChestSpawn[] GetChestSpawns() { return chestSpawns; }
        public MeshRenderer GetMesh() { return mesh; }
    }
}
