using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Player;

namespace ATDungeon.Dungeon
{
    public class LockedWall : MonoBehaviour
    {
        [SerializeField]
        private int lockedWallID;
        [SerializeField]
        private DungeonRoom dungeonRoom;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                bool hasKeyRequired = other.gameObject.transform.parent.GetComponent<PlayerController>().HasKeyID(lockedWallID);
                if (hasKeyRequired)
                {
                    dungeonRoom.UnlockRoom();
                    //Remove Key From Player Here
                }
            }
        }

        public void SetLockedWallID(int n) { lockedWallID = n; }
        public int GetLockedWallID() { return lockedWallID; }
        public void SetDungeonRoom(DungeonRoom dr) { dungeonRoom = dr; }
    }
}
