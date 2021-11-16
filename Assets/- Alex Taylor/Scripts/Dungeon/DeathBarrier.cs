using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATDungeon.Dungeon
{
    public class DeathBarrier : MonoBehaviour
    {
        Player.PlayerController playerController;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                playerController = other.gameObject.transform.parent.gameObject.GetComponent<Player.PlayerController>();
                playerController.GetHitBox().GenerateHit(999999, false);
            }
        }
    }
}
