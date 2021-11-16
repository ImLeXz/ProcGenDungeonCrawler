using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Dungeon
{
    public class ChestSpawn : MonoBehaviour
    {
        [SerializeField]
        private bool bSpawnChest = true;
        [SerializeField]
        private bool bSpawnCrate = true;
        [SerializeField]
        private bool bOnlySpawnOnBlockedConnection = true;
        [SerializeField]
        private bool bAlwaysSpawn = false;

        public bool CanSpawnChest
        {
            get { return bSpawnChest; }
            set { bSpawnChest = value; }
        }

        public bool CanSpawnCrate
        {
            get { return bSpawnCrate; }
            set { bSpawnCrate = value; }
        }

        public bool GetShouldAlwaysSpawn() { return bAlwaysSpawn; }
        public bool GetOnlyBlockedSpawn() { return bOnlySpawnOnBlockedConnection; }
    }
}
