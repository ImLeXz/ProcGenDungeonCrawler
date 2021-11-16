using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Dungeon.Setup;

namespace ATDungeon.Dungeon.Testing
{

    public class ObjectMoverTest : MonoBehaviour
    {
        [SerializeField]
        private GameObject obj1;

        [SerializeField]
        private GameObject obj2;

        [SerializeField]
        private float delay;

        // Start is called before the first frame update
        void Start()
        {
            Invoke("SnapObjects", delay);
        }

        private void SnapObjects()
        {
            RoomSpawner room_01 = obj1.GetComponent<RoomSpawner>();
            RoomSpawner room_02 = obj2.GetComponent<RoomSpawner>();
            ConnectionPoint[] room01Connections = room_01.ConnectionPoints;
            ConnectionPoint[] room02Connections = room_02.ConnectionPoints;
            ConnectionPoint room01Con;
            ConnectionPoint room02Con;
            int ranNum01;
            int ranNum02;

            do
            {
                ranNum01 = Random.Range(0, room01Connections.Length);
                room01Con = room01Connections[ranNum01];
            } while (room01Con.IsConnected);

            do
            {
                ranNum02 = Random.Range(0, room02Connections.Length);
                room02Con = room02Connections[ranNum02];
            } while (room02Con.IsConnected);

            Vector3 xRot = room01Con.transform.eulerAngles;
            Vector3 yRot = room02Con.transform.eulerAngles;

            Debug.Log(room01Con.gameObject.name + " Rotation = " + xRot);
            Debug.Log(room02Con.gameObject.name + " Rotation = " + yRot);

            int rotOffset = Mathf.RoundToInt(yRot.y - xRot.y);

            if (Mathf.Abs(rotOffset) == 0)
                rotOffset = 180;
            else if (Mathf.Abs(rotOffset) == 180)
                rotOffset = 0;

            Debug.Log("Rotation Offset: " + rotOffset);

            room_02.gameObject.transform.eulerAngles = new Vector3(0.0f, room_02.gameObject.transform.eulerAngles.y + rotOffset, 0.0f);

            if (room01Con != null && room02Con != null)
            {
                Vector3 offset = room01Con.gameObject.transform.position - room02Con.gameObject.transform.position;
                room_02.transform.position += offset;
                room01Con.IsConnected = true;
                room02Con.IsConnected = true;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
