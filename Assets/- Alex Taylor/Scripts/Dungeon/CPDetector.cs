using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Utility;

namespace ATDungeon.Dungeon.Setup
{
    public class CPDetector : MonoBehaviour
    {
        [SerializeField]
        private ConnectionPoint cp;

        [Header("Debug Options")]
        [SerializeField]
        private GameObject testObj01;
        [SerializeField]
        private GameObject testObj02;
        [SerializeField]
        private bool debug;

        ConnectionPoint cpOther;
        bool canGenerate = false;
        // Start is called before the first frame update
        void Start()
        {
            Initialise();
        }

        // Update is called once per frame
        void Update()
        {
            if (cp.IsInitialised())
            {
                canGenerate = true;
                Destroy(this.gameObject, DungeonManager.instance.GetLoopWaitTime());
            }
        }

        private void Initialise()
        {
            this.gameObject.tag = "CPDetector";

            if (cp == null)
                cp = this.transform.parent.GetComponent<ConnectionPoint>();

            if (debug)
            {
                canGenerate = false;
                Destroy(this.gameObject, DungeonManager.instance.GetLoopWaitTime());
                SpawnLoop(testObj01, testObj02);
            }
        }

        public ConnectionPoint GetCP() { return cp; }

        public bool CanGenerate
        {
            get { return canGenerate; }
            set { canGenerate = value; }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (canGenerate && !cp.IsConnected)
            {
                if (collision.gameObject.tag == "CPDetector")
                {
                    Debug.Log("Collision Detected! - From[" + this.gameObject.name + "] to - [ " + collision.gameObject.name + "] ");
                    CPDetector other = collision.gameObject.GetComponent<CPDetector>();
                    cpOther = other.GetCP();
                    if(other.CanGenerate && !cpOther.IsConnected)
                    {
                        canGenerate = false;
                        other.CanGenerate = false;
                        SpawnLoop(cp.gameObject, cpOther.gameObject);
                    }
                }
            }
        }

        private void SpawnLoop(GameObject from, GameObject to)
        {
            Debug.Log("Spawning Loop!");
            Vector3 startPos = from.transform.position;
            Vector3 endPos = to.transform.position;
            Vector3 dist = endPos - startPos;
            float distanceX = Mathf.Abs(UtilFunctions.instance.ReturnDistance(UtilFunctions.Axis.x, dist));
            float distanceZ = Mathf.Abs(UtilFunctions.instance.ReturnDistance(UtilFunctions.Axis.z, dist));
            Debug.Log("Distance X: " + distanceX);
            Debug.Log("Distance Z: " + distanceZ);
            float minDistance = DungeonManager.instance.GetLoopMinDistance();
            if ((distanceX > minDistance) && (distanceZ > minDistance))
            {
                if (dist.magnitude < DungeonManager.instance.GetLoopMaxDistance())
                {
                    Debug.Log("Generating!");
                    LoopSpawner ls = DungeonManager.instance.GetLoopSpawner();
                    cp.IsConnected = true;
                    cpOther.IsConnected = true;
                    ls.SpawnHall(from, to, cp, cpOther, ls.ReturnDesiredHallwayType(from, to));
                }
                else
                {
                    Debug.LogWarning("Loop Couldn't Be Spawned, Distance Greater Than Max Distance: [" + dist.magnitude + "]");
                }
            }
            else
            {
                Debug.LogWarning("Loop Couldn't Be Spawned, Distance Less Than Min Distance");
            }
        }
    }
}
