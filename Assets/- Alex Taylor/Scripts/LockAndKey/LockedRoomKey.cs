using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Player;

namespace ATDungeon.Pickups
{
    public class LockedRoomKey : MonoBehaviour
    {
        [SerializeField]
        private int keyID;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                Debug.Log("Player Picked Up Key");
                other.gameObject.transform.parent.GetComponent<PlayerController>().AddKeyID(keyID);
                DestroyKey();
            }
        }

        private void DestroyKey()
        {
            //Key Death Animation
            Destroy(this.gameObject);
        }

        public int GetKeyID() { return keyID; }
        public void SetKeyID(int n) { keyID = n; }
    }
}
