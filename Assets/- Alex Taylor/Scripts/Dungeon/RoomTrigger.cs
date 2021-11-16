using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Dungeon
{
    public class RoomTrigger : MonoBehaviour
    {
        private DungeonRoom dungeonRoom;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                dungeonRoom.ActivateEnemies();
            }
        }

        public void SetDungeonRoom(DungeonRoom dr) { dungeonRoom = dr; }
    }
}
