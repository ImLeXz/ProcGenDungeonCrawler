using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Dungeon.Setup;

namespace ATDungeon.Dungeon.Testing
{
    public class LoopSpawnerTester : MonoBehaviour
    {
        [SerializeField]
        private GameObject obj01;
        [SerializeField]
        private GameObject obj02;
        [SerializeField]
        private LoopSpawner ls;
        // Start is called before the first frame update
        void Start()
        {
            ls.SpawnHall(obj01, obj02, null, null, LoopSpawner.HallwayType.Lshape);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
