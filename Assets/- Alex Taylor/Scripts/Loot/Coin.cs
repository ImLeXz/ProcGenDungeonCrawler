using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATDungeon.Player;

namespace ATDungeon.Pickups
{
    public class Coin : MonoBehaviour
    {
        [SerializeField]
        private int coinAmount;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.transform.parent.GetComponent<PlayerController>().ChangeCoinsBy(coinAmount);
                DestroyCoin();
            }
        }

        private void DestroyCoin()
        {
            //Coin Death Animation
            Destroy(this.gameObject);
        }
    }
}
